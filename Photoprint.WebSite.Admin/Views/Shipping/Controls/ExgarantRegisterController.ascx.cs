using System;
using System.Linq;
using Photoprint.Core;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using Photoprint.WebSite.Admin.Controls;

namespace Photoprint.WebSite.Admin.Views.Shipping.Controls
{
    public partial class ExgarantRegisterController : BaseControl
    {
        protected static readonly IShippingService ShippingService = WebSiteGlobal.Container.GetInstance<IShippingService>();
        protected static readonly IOrderService OrderService = WebSiteGlobal.Container.GetInstance<IOrderService>();
        protected static readonly IOrderHistoryService OrderHistoryService = WebSiteGlobal.Container.GetInstance<IOrderHistoryService>();
        protected static readonly IShippingProviderResolverService ShippingProviderResolver = WebSiteGlobal.Container.GetInstance<IShippingProviderResolverService>();

        public Order CurrentOrder { get; set; }
        public Photolab OrderFrontend { get; set; }
        private Postal Postal => ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
        private ExgarantServiceProviderSettings Settings => Postal.ServiceProviderSettings as ExgarantServiceProviderSettings;

        protected ExgarantRegisterController()
        {
            Load += ExgarantRegisterControllerLoad;
        }

        private void ExgarantRegisterControllerLoad(object sender, EventArgs e)
        {
            var number = string.IsNullOrEmpty(CurrentOrder.CustomId) ? CurrentOrder.Id.ToString() : CurrentOrder.CustomId;
            if (Postal != null && Postal.IsUseConractNumber && CurrentOrder.ContractorStatus == ContractorStatus.Assigned)
            {
                var contractOrder = OrderService.GetListByParent(CurrentOrder).FirstOrDefault();
                if (contractOrder != null)
                    number = Postal.IsUseAdditionalContractNumber && !contractOrder.Number.Equals(contractOrder.Id.ToString())
                        ? $"{contractOrder.Id} ({contractOrder.Number})"
                        : contractOrder.Id.ToString();
            }

            var fields = CurrentOrder.Properties?.ExgarantDeliveryFields ?? new ExgarantEditableDeliveryInput
            {
                Phone = CurrentOrder.DeliveryAddress?.Phone?.Trim().TrimStart('+').Replace("-", string.Empty) ?? string.Empty,
                ExtensionNumber = number
            };
            if (Page.IsPostBack) return;
            txtDeliveryFields.Value = Newtonsoft.Json.JsonConvert.SerializeObject(fields);
        }

        private ExgarantEditableDeliveryInput DeliveryInput => 
            Newtonsoft.Json.JsonConvert.DeserializeObject<ExgarantEditableDeliveryInput>(txtDeliveryFields.Value);
        

        protected void RegisterExgarantClick(object sender, EventArgs e)
        {
            try
            {
                var exgarantService = ShippingProviderResolver.GetProvider(Postal);
                CurrentOrder.Properties.ExgarantDeliveryFields = DeliveryInput;
                OrderService.UpdateProperties(CurrentOrder);

                if (exgarantService != null)
                {
                    var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
                    exgarantService.GetCreateOrderRegistration(OrderFrontend, CurrentOrder, shipping.ServiceProviderSettings);
                }
                Response.Redirect(Request.Url.AbsolutePath + "#shipping");
            }
            catch (PhotoprintSystemException ex)
            {
                plhdExgarantError.Visible = litExgarantError.Visible = true;
                litExgarantError.Text = ex.Message;
            }
        }
        protected void UpdateExgarantStatusClick(object sender, EventArgs e)
        {
            try
            {
                var exgarantService = ShippingProviderResolver.GetProvider(Postal);
                if (exgarantService != null)
                {
                    var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
                    exgarantService.GetOrderStatus(OrderFrontend, CurrentOrder, shipping.ServiceProviderSettings);
                }
                Response.Redirect(UrlManager.GetHRefUrl(AdminLinkType.PhotolabOrderHistory));
            }
            catch (PhotoprintSystemException ex)
            {
                plhdExgarantError.Visible = litExgarantError.Visible = true;
                litExgarantError.Text = ex.Message;
            }
        }
    }
}