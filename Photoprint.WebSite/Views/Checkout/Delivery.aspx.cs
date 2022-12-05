using PhoneConverter;
using Photoprint.Core;
using Photoprint.Core.InputModels;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using Photoprint.WebSite.Controls;
using Photoprint.WebSite.Modules;
using Photoprint.WebSite.Views.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Photoprint.WebSite.Shared;

namespace Photoprint.WebSite
{
    public partial class Delivery : BaseCheckoutModulePage, IGoogleAnalyticsDataProvider
    {
        protected static readonly IOrderAddressService OrderAddressService = Container.GetInstance<IOrderAddressService>();
        protected static readonly IFrontendOrderService FrontendOrderService = Container.GetInstance<IFrontendOrderService>();
        protected static readonly IOrderCommentService OrderCommentService = Container.GetInstance<IOrderCommentService>();
        protected static readonly IFrontendShippingService FrontendShippingService = Container.GetInstance<IFrontendShippingService>();
        protected static readonly IPhoneRulesService PhoneRulesService = Container.GetInstance<IPhoneRulesService>();
        protected Delivery()
        {
            Init += DeliveryPageInit;
            Load += DeliveryPageLoad;
        }

        private UserCompanyInfo _userCompanyInfo { get; set; }
        protected UserCompanyInfo UserCompanyInfo
        {
            get
            {
                if (LoggedInUser == null) return null;
                if (_userCompanyInfo != null) return _userCompanyInfo;

                _userCompanyInfo = FrontendUserCompanyService.GetByUser(LoggedInUser);
                return _userCompanyInfo;
            }
        }

        private BaseShippingProviderControl GetShippingProviderControl()
        {
            if (plhdShippingProvider?.Controls == null || plhdShippingProvider.Controls.Count != 1) return null;

            return plhdShippingProvider.Controls[0] as BaseShippingProviderControl;
        }

        private void DeliveryPageInit(object sender, EventArgs e)
        {
            //
            // TODO: временная мера, после отмены старой доставки убрать
            //
            var settings = PhotolabSettingsService.GetSettings<DeliveryWindowSettings>(CurrentPhotolab);
            if (settings != null && settings.IsNewDeliveryWindowEnabled)
            {
                var url = new UrlManager { CurrentLanguageCulture = UrlManager.CurrentLanguageCulture };
                Response.Redirect(url.GetHRefUrl(SiteLinkType.CheckoutShippingNew));
                return;
            }
            //

            ValidateEmptyUser();
            if (ShoppingCart.Items.Count == 0 || !UserAuthService.IsUserMeetsRequirements(CurrentPhotolab, LoggedInUser)) Response.Redirect(new UrlManager().GetHRefUrl(SiteLinkType.ShoppingCart));

            var shippingProviderControl = (BaseShippingProviderControl)LoadControl("/Views/Checkout/Controls/Shipping/PixlparkShippingProviderControl.ascx");
            if (shippingProviderControl != null)
            {
                plhdShippingProvider.Controls.Add(shippingProviderControl);
                shippingProviderControl.InitControl(ShoppingCart.Items, UserCompany);
            }
        }

        private void DeliveryPageLoad(object sender, EventArgs e)
        {
            butNext.Text = butNext.Text.Replace("{0}", RM.GetString(RS.Common.Buttons.Next, false));
            //butNext.TrackingCode = "if(typeof orderTracker == 'function') orderTracker(); ";
            var shipping = GetShippingProviderControl();
            mgOrderSummary.InitControl(ShoppingCart.Items, shipping?.GetSelectedShipping());

            if (RenderGoogleAnalytics)
            {
                butNext.TrackingCode = "selectShippingGoogleTrack();";
            }

            var suppliersMinPrice = FrontendSuppliersSettingsService.GetSuppliersSettings(CurrentPhotolab).MinPrice ?? 0;
            var ordinaryItemsPrice = ShoppingCart.Items.Where(x => x is MaterialShoppingCartItem || x is ProductShoppingCartItem).Sum(shoppingCart => shoppingCart.Price);
            var suppliersItems = ShoppingCart.Items.Where(x => x is GFShoppingCartItem);
            if (suppliersItems.Any())
            {
                var suppliersItemsPrice = suppliersItems.Sum(shoppingCart => shoppingCart.Price);
                if (suppliersMinPrice - suppliersItemsPrice > 0.1m)
                {
                    plhdCheckoutValidation.Visible = true;
                    plhdMinimumShoppingCartItemsPrice.Visible = false;
                    litCheckoutValidation.Text = string.Format(
                        RM.GetString(RS.Suppliers.MinimumSupplierItemsPriceWarningFormatString),
                        Utility.GetPrice(suppliersMinPrice, CurrentPhotolab));
                }
            }

            if (ordinaryItemsPrice > 0 && CurrentPhotolab.Properties.MinimumShoppingCartItemsPrice - ordinaryItemsPrice > 0.1m)
            {
                if (CurrentPhotolab.Properties.IsWarningOnlyMinimumShoppingCartItemsPrice)
                {
                    plhdCheckoutValidation.Visible = false;
                    plhdMinimumShoppingCartItemsPrice.Visible = true;
                    litMinimumShoppingCartItemsPrice.Text = string.Format(RM.GetString(RS.Shop.ShoppingCart.MinimumShoppingCartItemPriceInformationFormatString),
                        Utility.GetPrice(CurrentPhotolab.Properties.MinimumShoppingCartItemsPrice, CurrentPhotolab));
                }
                else
                {
                    plhdCheckoutValidation.Visible = true;
                    plhdMinimumShoppingCartItemsPrice.Visible = false;
                    litCheckoutValidation.Text = string.Format(RM.GetString(RS.Shop.ShoppingCart.MinimumShoppingCartItemPriceWarningFormatString),
                        Utility.GetPrice(CurrentPhotolab.Properties.MinimumShoppingCartItemsPrice, CurrentPhotolab));
                }
            }
        }


        protected void NextStepClick(object sender, EventArgs e)
        {
            var shippingProvider = GetShippingProviderControl();

            if (shippingProvider == null)
            {
                plhdError.Visible = true;
                return;
            }

            var selectedShipping = shippingProvider.GetSelectedShipping();
            var selectedAddress = shippingProvider.GetSelectedOrderAddress();

            Page.Validate();
            if (!string.IsNullOrWhiteSpace(shippingProvider.BaseLocalizedGeneralError))
            {
                txtErrMessage.Text = shippingProvider.BaseLocalizedGeneralError;
            }

            if (!Page.IsValid) return;
            if (selectedShipping == null || selectedAddress == null)
            {
                plhdError.Visible = true;
                return;
            }
            UserService.UpdateUserFromRecipient(LoggedInUser, CurrentPhotolab, selectedAddress, selectedShipping.ShippingServiceProviderType, true,
                out bool isSpamNumber);
            
            if (isSpamNumber)
            {
                plhdError.Visible = true;
                txtErrMessage.Text = RM.GetString(RS.General.Error.ShippingPhoneIncorrect);
                return;
            }           
            
            try
            {
                var allItems = ShoppingCart.Items;
                var affiliateUser = AffiliateUserService.GetAffiliateForOrder(CurrentCompanyAccount, LoggedInUser);

                string orderNumber = null;
                var items = ShoppingCart.Items.Where(i => i.Properties?.JobId != null); // DIGILABS
                if (items.Any())
                {
                    orderNumber = items.First().Properties.JobId;
                }
                var company = chkByCompany.Checked ? FrontendUserCompanyService.GetByUser(LoggedInUser)?.UserCompany : null;

                var orderCommentInputList = new List<OrderCommentInput>();
                foreach (var item in ShoppingCart.Items)
                {
                    if (!string.IsNullOrWhiteSpace(item.SystemDescription))
                    {
                        orderCommentInputList.Add(new OrderCommentInput(null, null, null, item.SystemDescription, false, true));
                    }

                    if (!string.IsNullOrWhiteSpace(item.UserDescription))
                    {
                        orderCommentInputList.Add(new OrderCommentInput(null, null, null, item.UserDescription, false));
                    }
                }

                if (!string.IsNullOrWhiteSpace(txtOrderComment.Text))
                {
                    var ip = SiteUtils.GetIpAddress(new HttpRequestWrapper(HttpContext.Current.Request));
                    orderCommentInputList.Add(new OrderCommentInput(null, null, ip, txtOrderComment.Text, true));
                }

                var order = FrontendOrderService.Create(CurrentPhotolab, LoggedInUser, ShoppingCart.Items,
                    selectedShipping, selectedAddress, OrderPaymentStatus.NotPaid, CurrentLanguage, affiliateUser, orderNumber, WebSiteGlobal.IsMobile, company,
                    properties =>
                    {
                        var gacid = Request.Cookies["_ga"]?.Value ?? string.Empty;
                        if (!string.IsNullOrEmpty(gacid))
                        {
                            var gaClientId = Regex.Match(gacid, "([0-9]+.[0-9]+)$").Value;
                            if (!string.IsNullOrEmpty(gaClientId))
                            {
                                properties.GoogleAnalyticsClientId = gaClientId;
                            }
                        }

                        var yaClientId = Request.Cookies["_ym_uid"]?.Value ?? string.Empty;
                        if (!string.IsNullOrEmpty(yaClientId))
                        {
                            properties.YandexMetrikaClientId = yaClientId;
                        }

                        if (!string.IsNullOrEmpty(shippingProvider.AdditionalNotificationEmail))
                        {
                            properties.AdditionalNotificationEmail = shippingProvider.AdditionalNotificationEmail;
                        }
                        return properties;
                    }, OrderSource.Site, orderCommentInputList);

                Response.Redirect(new UrlManager { CurrentOrderId = order.Id }.GetHRefUrl(SiteLinkType.UserOrderPayment));
            }
            catch (PhotoprintValidationException ex)
            {
                Logger.Error("Ошибка: {0}", ex.Message);
                plhdCheckoutValidation.Visible = true;
                var suppliersMinPrice = FrontendSuppliersSettingsService.GetSuppliersSettings(CurrentPhotolab).MinPrice ?? 0;
                var validationPrice = Math.Max(CurrentPhotolab.Properties.MinimumShoppingCartItemsPrice, suppliersMinPrice);
                litCheckoutValidation.Text = string.Format(RM.GetString(ex.ResourceKey),
                                                           Utility.GetPrice(validationPrice, CurrentPhotolab));
            }

        }

        public IEnumerable<WebAnalyticsProductFieldObject> GetGoogleProducts()
        {
            return CartToGoogleProducts(ShoppingCart.Items);
        }

        public WebAnalyticsActionFieldObject GetGoogleActionFieldObject()
        {
            return new WebAnalyticsActionFieldObject { Step = 1 };
        }

        public IEnumerable<WebAnalyticsActionFieldObject> GetGoogleActions()
        {
            return null;
        }
    }
}