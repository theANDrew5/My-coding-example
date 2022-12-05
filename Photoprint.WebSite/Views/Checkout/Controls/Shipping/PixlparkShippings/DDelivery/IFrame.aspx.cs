using System;
using System.Web.UI;
using Photoprint.Core.Models;
using Photoprint.Core.Services;

namespace Photoprint.WebSite.Views.Checkout.Controls.Shipping.PixlparkShippings.DDelivery
{
    public partial class IFrame : Page
	{
        protected IFrame()
        {
            Load += Page_Load;
        }
        
        private string _cssUrl;
		protected void Page_Load(object sender, EventArgs e)
		{
			if (int.TryParse(Request.QueryString["sid"], out int id))
			{
				var shippingService = WebSiteGlobal.Container.GetInstance<IShippingService>();
				var shippng = shippingService.GetById<Postal>(id);
				_cssUrl = string.Empty;

				if (shippng != null)
				{
					var settings = shippng.ServiceProviderSettings as DDeliveryServiceProviderSettings;
					if (settings != null)
					{
						_cssUrl = settings.CssUrl;
					}
				}
			}
		}

        protected string GetStyleUrl()
        {
            const string dDeliveryCss = "/Views/Checkout/Controls/Shipping/PixlparkShippings/DDelivery/tems/default/css/screen.css";
            return (!string.IsNullOrWhiteSpace(_cssUrl)) ? _cssUrl : dDeliveryCss;
        }
	}
}