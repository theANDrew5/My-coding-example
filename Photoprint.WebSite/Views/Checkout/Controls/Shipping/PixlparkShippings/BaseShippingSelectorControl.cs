using System;
using System.Collections.Generic;
using System.Linq;
using Photoprint.Core;
using Photoprint.Core.Models;
using Photoprint.Core.Services;

namespace Photoprint.WebSite.Controls
{
	public abstract class BaseShippingSelectorControl : BaseControl
	{
		protected static readonly IFrontendShippingService FrontendShippingService = Container.GetInstance<IFrontendShippingService>();
	    protected static readonly IFrontendShippingPriceService FrontendShippingPriceService = Container.GetInstance<IFrontendShippingPriceService>();
        protected static readonly IShippingProviderResolverService ShippingProviderResolverService = Container.GetInstance<IShippingProviderResolverService>();
        protected static readonly IShippingCalculatorService ShippingCalculatorService = Container.GetInstance<IShippingCalculatorService>();
        protected static readonly ISmsService SmsService = Container.GetInstance<ISmsService>();

	    private IGeoIPService _geoIpService;
	    protected IGeoIPService GeoIpService => _geoIpService ?? (_geoIpService = Container.GetInstance<IGeoIPService>());

        protected IReadOnlyCollection<IPurchasedItem> ShippableItems { get; private set; }
		protected Shipping Shipping { get; private set; }
		protected IReadOnlyCollection<Discount> Discounts { get; private set; }
        public string BaseLocalizedShippingError { get; protected set; } = string.Empty;
        protected IEnumerable<Order> PreviousOrders { get; private set; }
        protected bool UseMiddleName => Shipping?.UseMiddleName ?? false;
		public string AdditionalEmail { get; set; }
		protected bool UseAdditionalEmail => PhotolabSettingsService.GetSettings<DeliveryWindowSettings>(CurrentPhotolab)?.IsAdditionalNotificationEmailEnabled ?? false;

		protected OrderAddress OrderAddress { get; set; }

        private System.Web.UI.WebControls.CustomValidator _priceValidator;

		public void InitControl(Shipping shipping, IReadOnlyCollection<IPurchasedItem> items, IReadOnlyCollection<Discount> discounts,
            IEnumerable<Order> orders = null, System.Web.UI.WebControls.CustomValidator priceValidator = null)
		{
			Shipping = shipping;
			ShippableItems = items;
			Discounts = discounts;
            PreviousOrders = orders;
            _priceValidator = priceValidator;
        }

		public bool TryGetShippingPrice(out decimal price)
        {
            var address = GetSelectedOrderAddress();
			if(!(address is null))
            {
                var calculationResult =
                    ShippingCalculatorService.GetShippingPrice(Shipping, address, ShippableItems, Discounts);
                if (calculationResult != null)
                {
                    if (!(_priceValidator is null))
                        _priceValidator.IsValid = calculationResult.Success;
                    if (!(calculationResult.Properties is null))
                        OrderAddress.DeliveryProperties = calculationResult.Properties;
                    price = calculationResult.Price;
                    return calculationResult.Success;
                }
            }
			price = 0;
			return false;
        }

		public abstract OrderAddress GetSelectedOrderAddress();

		protected enum AddressItem
		{
			Country,
			Region,
			City,
			Storage,
            Street,
            AddressLine1
		}
		
		protected BaseShippingSelectorControl()
		{
			Init += BaseShippingSelectorControlInit;
		}

	    protected bool IsValidationDisabled => (!CurrentPhotolab.Properties.IsCyrillicNamesEnabled || IsCompanyTeamMember) && Shipping.ShippingServiceProviderType != ShippingServiceProviderType.Photomax;

	    protected bool IsCompanyTeamMember
	    {
	        get
	        {
	            var currentUser = LoggedInUser;
	            var isTeamMember =currentUser != null && currentUser.IsInFrontendTeam(CurrentPhotolab);
                return isTeamMember;
	        }
	    }

	    private void BaseShippingSelectorControlInit(object sender, EventArgs e)
		{
			InitSelectedShipping(Shipping);
		}
		
		protected abstract void InitSelectedShipping(Shipping shipping);
	    public abstract bool ValidateInput();

        protected virtual void SetLocalizedCustomErrorString(string input)
        {
            BaseLocalizedShippingError = input;
        }
    }
}
