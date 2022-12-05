using System;
using System.Collections.Generic;
using System.Linq;
using Photoprint.Core;
using Photoprint.Core.Configuration;
using Photoprint.Core.Models;

namespace Photoprint.WebSite.Admin.Controls
{
    public partial class DistributionPointSelector : BaseShippingSelectorControl
    {
		protected string GMapKey => Settings.GMapAdminKey;
		protected DistributionPoint SelectedPoint { get; set; }

        protected DistributionPointSelector()
    	{
    		Load += PageLoad;
    	}

    	private void PageLoad(object sender, EventArgs e)
        {
            imgMap.Attributes.Add("title", RM.GetString(RS.Order.Delivery.ViewMap));

			if (!IsPostBack) 
            {
                var language = WebSiteGlobal.UILanguage;

                refFullMap.Attributes.Add("onclick", "return false;");
                imgMap.Src = string.Format("{0}?{1}", SelectedPoint.ThumbnailImageUrl, DateTime.Now.Second);
                imgMap.Attributes.Add("data-zoom-src", SelectedPoint.ImageUrl);
                imgMap.Alt = SelectedPoint.AdminTitle;

                litOfficeHours.Text = SelectedPoint.OfficeHours;
                litPhone.Text = SelectedPoint.Phone;
                litTitle.Text = SelectedPoint.AdminTitle;
                litAddress.Text = SelectedPoint.Address.ToString();
                litDescription.Text = TextManager.GetInstance()
                    .GetRenderedText(SelectedPoint.DescriptionLocalized[language], true);

                var address = CurrentOrder != null ? CurrentOrder.DeliveryAddress : null;
                if (address is null) return;

            }

        }
        
    	public override void InitControl(Shipping shipping, Order order, IReadOnlyCollection<IPurchasedItem> items)
        {
            if (!(shipping is DistributionPoint point)) throw new ArgumentException(nameof(shipping));

            SelectedPoint = point;
			CurrentOrder = order ?? throw new ArgumentNullException(nameof(order));

    	}

        public override OrderAddress GeOrderAddress()
        {
            return new OrderAddress(SelectedPoint.Address);
        }


        public override bool ValidateInput()
		{
			return true;
		}
    }
}