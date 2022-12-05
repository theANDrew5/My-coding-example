using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photoprint.Core.Models;
using Photoprint.Core.Services;

namespace Photoprint.WebSite.Admin.Controls
{
    public partial class PicpointSelector : BaseShippingSelectorControl
    {
        private static readonly IFrontendShippingService _shippingService =
            WebSiteGlobal.Container.GetInstance<IFrontendShippingService>();

        protected Postal CurrentPostal { get; set; }
        protected PickpointServiceProviderSettings Settings { get; set; }
        protected Order CurrentOrder { get; set; }
        public override void InitControl(Shipping shipping, Order order, IReadOnlyCollection<IPurchasedItem> items)
        {
            if (!(shipping is Postal postal)) throw new ArgumentException(nameof(shipping));
            if (!(postal.ServiceProviderSettings is PickpointServiceProviderSettings settings)) throw new ArgumentException(nameof(settings));

            CurrentPostal = postal;
            Settings = settings;
            CurrentOrder = order ?? throw new ArgumentNullException(nameof(order));
        }

        public override OrderAddress GeOrderAddress()
        {
            if (string.IsNullOrWhiteSpace(selectedAddress.Value)) return null;

            var finalState = new DeliveryFinalState
            {
                ShippingData = JsonConvert.DeserializeObject<ShippingAddressDTO>(selectedAddress.Value)
            };

            var newAddress = _shippingService.GetSelectedOrderAddress(finalState, CurrentPostal);

            return newAddress;
        }

        public override bool ValidateInput()
        {
            return !string.IsNullOrWhiteSpace(selectedAddress.Value);
        }

        protected OrderAddress OldAddress => CurrentOrder?.DeliveryAddress ?? new OrderAddress();
        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}