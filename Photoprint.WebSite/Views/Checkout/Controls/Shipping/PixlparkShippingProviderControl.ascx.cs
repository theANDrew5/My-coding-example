using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Photoprint.Core;
using Photoprint.Core.Models;
using Photoprint.Core.Services;

namespace Photoprint.WebSite.Controls
{
	public abstract class BaseShippingProviderControl : BaseControl
	{
		protected static readonly IFrontendShippingService FrontendShippingService = Container.GetInstance<IFrontendShippingService>();
		protected static readonly IShippingCalculatorService ShippingCalculatorService = Container.GetInstance<IShippingCalculatorService>();
	    protected static readonly IFrontendShippingPriceService FrontendShippingPriceService = Container.GetInstance<IFrontendShippingPriceService>();
		protected static readonly IFrontendPaymentService FrontendPaymentService = Container.GetInstance<IFrontendPaymentService>();
		protected static readonly IFrontendDiscountService FrontendDiscountService = Container.GetInstance<IFrontendDiscountService>();
		protected static readonly IOrderAddressService OrderAddressService = Container.GetInstance<IOrderAddressService>();
        protected static readonly IFrontendMaterialTypeService FrontendMaterialTypeService = Container.GetInstance<IFrontendMaterialTypeService>();
        protected static readonly ISuppliersCategoryService SuppliersCategoryService = Container.GetInstance<ISuppliersCategoryService>();
        protected static readonly IFrontendSuppliersService FrontendSuppliersService = Container.GetInstance<IFrontendSuppliersService>();
        protected static readonly IFrontendOrderService FrontendOrderService = Container.GetInstance<IFrontendOrderService>();
        protected IEnumerable<ShoppingCartItem> CartItems { get; private set; }
		protected IEnumerable<Discount> Discounts { get; private set; }
		protected UserCompany UserCompany { get; private set; }
        public string BaseLocalizedGeneralError { get; set; } = string.Empty;
		public string AdditionalNotificationEmail { get; set; } = string.Empty;
        public abstract Shipping GetSelectedShipping();
		public abstract OrderAddress GetSelectedOrderAddress();
		public abstract bool TryGetShippingPrice(out decimal price);

		public void InitControl(IEnumerable<ShoppingCartItem> cartItems, UserCompany userCompany)
		{
			CartItems = cartItems.ToList();
			UserCompany = userCompany;
			Discounts = FrontendDiscountService.FindDiscountsForOrder(CurrentPhotolab, LoggedInUser, CartItems, filterByShipping: false);
		}
	}

	public partial class PixlparkShippingProviderControl : BaseShippingProviderControl
	{
		private IEnumerable<Shipping> _allAvailableShippings;
        protected readonly ShippingServiceProviderType[] BannedProviders = { ShippingServiceProviderType.RussianPost };
		protected IEnumerable<Shipping> AllAvailableShippings
		{
			get
			{
			    if (_allAvailableShippings != null) return _allAvailableShippings;

			    var shippings = FrontendShippingService.GetAvailableList<Shipping>(CurrentPhotolab, CartItems).OrderBy(s => s.Position).AsList();
                var allPayments = FrontendPaymentService.GetList(CurrentPhotolab).Where(p => p.IsActive);

			    if (allPayments.Any())
			    {
			        var result = new List<Shipping>();
			        foreach (var shipping in shippings)
			        {
			            var payments = FrontendPaymentService.GetAvailableList(CurrentPhotolab, CartItems, UserCompany, shipping, CurrentLanguage);
			            if (payments.Any())
			                result.Add(shipping);
			        }
			        _allAvailableShippings = result;
			    }
			    else
			    {
			        _allAvailableShippings = shippings;
			    }

			    return _allAvailableShippings;
			}
		}
        protected bool CheckoutWeightConstraint(Shipping shipping)
	    {
	        var totalWeight = CartItems.Sum(x => x.TotalWeight);

	        var prices = FrontendShippingPriceService.GetListByShipping(shipping);

            return prices.Where(price => price.PriceList.IsAvailableWeightConstrain).All(p => p.PriceList.MaximumWeight > totalWeight);
	    }

	    protected bool ShowWarningBox(bool notAvailableMaterialTypes, bool checkoutWeightConstraint, Shipping shipping)
	    {
	        if (!WebSiteGlobal.CurrentPhotolab.Properties.HideNotAvailableShippings)
	            return notAvailableMaterialTypes || !AllAvailableShippings.Contains(shipping) || !checkoutWeightConstraint;
	        return false;
	    }

	    public IReadOnlyCollection<string> GetNotAvailableMaterialTypesForShipping(Shipping shipping)
	    {   
            var result = new List<string>();

	        var materialTypeIds = FrontendMaterialTypeService.GetIdsListByShipping(shipping);
	        var gfCategories = SuppliersCategoryService.GetAvailableCategoriesIds(shipping);
	        foreach (var shoppingCartItem in CartItems)
	        {
                var mitem = shoppingCartItem as MaterialShoppingCartItem;
                if (mitem != null && materialTypeIds.Any())
                {
                    var materialType = FrontendMaterialTypeService.GetByMaterialId(mitem.MaterialId, CurrentLanguage);
                    if (materialType != null && !materialTypeIds.Contains(materialType.Id))
                    {
                        result.Add(materialType.Title);
                    }
                }
                else
                {
                    var gitem = shoppingCartItem as GFShoppingCartItem;
                    if (gitem != null && gfCategories.Any())
                    {
                        var gfsubproduct = FrontendSuppliersService.GetSubproductById(CurrentPhotolab, gitem.GFProductId);
                        if (gfsubproduct != null)
                        {
                            var gfproduct = FrontendSuppliersService.GetProductById(CurrentPhotolab, gfsubproduct.ParentId);
                            if (gfproduct?.CategoryIds != null && !gfproduct.CategoryIds.Intersect(gfCategories).Any())
                            {
                                result.Add(gfproduct.CurrentName);
                            }
                        }
                    }
                }
            }
	        result = result.Distinct().AsList();
	        return result;
	    }

		private List<ShippingType> _shippingTypes;
		protected IEnumerable<ShippingType> ShippingTypes
		{
			get
			{
				if (_shippingTypes == null)
				{
				    if (CurrentPhotolab.Properties.HideNotAvailableShippings)
				    {
				        _shippingTypes = AllAvailableShippings.Select(s => s.Type).Distinct().AsList();
				    }
				    else
                    {
                        _shippingTypes = FrontendShippingService.GetAvailableList<Shipping>(CurrentPhotolab, Enumerable.Empty<ShoppingCartItem>()).Select(s => s.Type).Distinct()
				            .AsList();
                    }
				}
				return _shippingTypes.OrderBy(t => CurrentPhotolab.Properties.SortedShippingTypes.IndexOf(t));
			}
		}

		protected (bool, decimal) TryGetApproximateShippingPrice(Shipping shipping, out bool isFixed)
		{
		    isFixed = true;
			var postal = shipping as Postal;
			if (postal != null)
			{
				var nova = postal.ServiceProviderSettings as NovaposhtaV2ServiceProviderSettings;
				if (nova?.IsDeliveryPriceCalculationEnabled == true)
				    return (true, 0);
			}

            if (ShippingCalculatorService.TryGetApproximateShippingPrice(shipping, CartItems.ToArray(),
                    Discounts.ToArray(), out var price, out isFixed)) return (true, price);
            Logger.Error("Checkout shipping Request: userId:{0} shippingId:{1} ex:{2}", LoggedInUser.Id, shipping.Id, DeliveryExceptionMessages.PriceCalculationError);
            return (false, 0);
        }

		public override bool TryGetShippingPrice(out decimal price)
        {
            price = 0m;
			var control = GetShippingSelectorControl();
			return control?.TryGetShippingPrice(out price) ?? false;
		}

		public override Shipping GetSelectedShipping()
		{
			if (AllAvailableShippings.Count() == 1) return AllAvailableShippings.First();

            return int.TryParse(Request.QueryString["sid"] ?? string.Empty, out var selectedId)
                ? AllAvailableShippings.FirstOrDefault(s => s.Id == selectedId)
                : null;
        }

		public override OrderAddress GetSelectedOrderAddress()
		{
			AdditionalNotificationEmail = string.Empty;
			var shippingControl = GetShippingSelectorControl();
            var selectedShipping = GetSelectedShipping();

            if (selectedShipping == null || shippingControl == null || !shippingControl.ValidateInput())
            {
                BaseLocalizedGeneralError = shippingControl?.BaseLocalizedShippingError; // Можем вызвать только после валидации
                return null;
            }

            var selectedOrderAddress = shippingControl?.GetSelectedOrderAddress();
		    if (selectedOrderAddress != null)
		    {
				selectedOrderAddress = OrderAddressService.Create(selectedOrderAddress);
		    }

            if (PhotolabSettingsService.GetSettings<DeliveryWindowSettings>(CurrentPhotolab)?.IsAdditionalNotificationEmailEnabled?? false)
				AdditionalNotificationEmail = shippingControl.AdditionalEmail;

			return selectedOrderAddress;
		}

		private BaseShippingSelectorControl GetShippingSelectorControl()
		{
		    return plhdShippingSelector.Controls.Count > 0
		        ? plhdShippingSelector.Controls[0] as BaseShippingSelectorControl
		        : null;
		}

        public PixlparkShippingProviderControl()
		{
			Load += OnLoad;
		}

		private void OnLoad(object sender, EventArgs eventArgs)
		{
			plhdShippingSelector.Controls.Clear();
			var selectedShipping = GetSelectedShipping();
			if (selectedShipping == null) return;

			var path = "/Views/Checkout/Controls/Shipping/PixlparkShippings/PointSelector.ascx";

			if (selectedShipping.Type == ShippingType.Courier)
            {
                path = "/Views/Checkout/Controls/Shipping/PixlparkShippings/CourierSelector.ascx";
            }
            else if (selectedShipping.Type == ShippingType.Postal)
			{
			    switch (selectedShipping.ShippingServiceProviderType)
			    {
			        case ShippingServiceProviderType.Cdek:
			            var postal = selectedShipping as Postal;

			            if (postal?.PostalType == PostalType.ToStorageDelivery)
			                path = ((postal?.ServiceProviderSettings as CdekServiceProviderSettings)?.UseCdekPvzWidget ?? false)
			                    ? "/Views/Checkout/Controls/Shipping/PixlparkShippings/CdekDeliverySelector.ascx"
			                    : "/Views/Checkout/Controls/Shipping/PixlparkShippings/ToStorageDeliveryMailSelector.ascx";
			            else
                            path =  "/Views/Checkout/Controls/Shipping/PixlparkShippings/ToClientDeliveryMailSelector.ascx";
			            break;
			        case ShippingServiceProviderType.DDelivery:
                        path = "/Views/Checkout/Controls/Shipping/PixlparkShippings/DDeliverySelector.ascx";
			            break;
			        case ShippingServiceProviderType.DDeliveryV2:
                        var saferoute = selectedShipping as Postal;

                        if ((saferoute?.ServiceProviderSettings as DDeliveryV2ServiceProviderSettings)?.DontUseWidget ?? false)
                        {
                            path = saferoute?.PostalType == PostalType.ToStorageDelivery
                                ? "/Views/Checkout/Controls/Shipping/PixlparkShippings/ToStorageDeliveryMailSelector.ascx"
                                : "/Views/Checkout/Controls/Shipping/PixlparkShippings/ToClientDeliveryMailSelector.ascx";
                        }
                        else
                        {
                            path = "/Views/Checkout/Controls/Shipping/PixlparkShippings/DDeliveryV2Selector.ascx";
                        }
                        break;
			        case ShippingServiceProviderType.YandexDelivery:
			            path = "/Views/Checkout/Controls/Shipping/PixlparkShippings/YandexDeliverySelector.ascx";
			            break;
                    case ShippingServiceProviderType.NovaposhtaV2:
                        path = "/Views/Checkout/Controls/Shipping/PixlparkShippings/NovaposhtaDeliverySelector.ascx";
                        break;
                    case ShippingServiceProviderType.Postnl:
                        path = "/Views/Checkout/Controls/Shipping/PixlparkShippings/PostnlDeliverySelector.ascx";
                        break;
                    case ShippingServiceProviderType.Ukrposhta:
                        path = "/Views/Checkout/Controls/Shipping/PixlparkShippings/UkrposhtaDeliverySelector.ascx";
                        break;
                    case ShippingServiceProviderType.Boxberry:
                        var boxberry = selectedShipping as Postal;
			            path = boxberry?.PostalType != PostalType.ToStorageDelivery
			                    ? "/Views/Checkout/Controls/Shipping/PixlparkShippings/BoxberryDeliverySelector.ascx"
			                    : "/Views/Checkout/Controls/Shipping/PixlparkShippings/ToStorageDeliveryMailSelector.ascx";
                        break;
                    case ShippingServiceProviderType.Pickpoint:
						path = "/Views/Checkout/Controls/Shipping/PixlparkShippings/PickpointDeliverySelector.ascx";
						break;
					case ShippingServiceProviderType.YandexGo:
						path = "/Views/Checkout/Controls/Shipping/PixlparkShippings/YandexGoSelector.ascx";
						break;
					default:
			            path = $"/Views/Checkout/Controls/Shipping/PixlparkShippings/{((Postal)selectedShipping).PostalType}MailSelector.ascx";
			            break;
			    }
			}

			var shippingSelectorControl = (BaseShippingSelectorControl)Page.LoadControl(path);
            var previousOrders = FrontendOrderService.GetListByUser(LoggedInUser, CurrentPhotolab.Id, OrderFilter.None, 0, 10);
			shippingSelectorControl.InitControl(selectedShipping, CartItems.ToArray(), Discounts.ToArray(), previousOrders, cstPrice);
			plhdShippingSelector.Controls.Add(shippingSelectorControl);
		}

	    protected int? GetSingleDeliveryId()
	    {
            var user = LoggedInUser;
            var lab = CurrentPhotolab;
            if (!(user.IsOperator(lab) || user.IsManager(lab))) return null;
            var bindings = FrontendShippingService.GetShippingBindings(CurrentCompanyAccount, user);
	        return bindings.Bindings[lab.Id]?.ShippingIds?.Count == 1 
                ? bindings.Bindings[lab.Id]?.ShippingIds?.FirstOrDefault()
                : null;
	    }

	    protected bool IsSingleDeliveryOperator => GetSingleDeliveryId() != null;

        protected void cstPrice_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = TryGetShippingPrice(out _);
        }
    }
}