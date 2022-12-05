using System;
using System.Collections.Generic;
using Photoprint.Core.Models;
using Photoprint.Core.Services;

namespace Photoprint.WebSite.Admin.Controls
{
	public abstract class BaseShippingSelectorControl : BaseControl
    {
        protected IFrontendShippingService FrontendShippingService =>
            WebSiteGlobal.Container.GetInstance<IFrontendShippingService>();
        protected IPhotolabSettingsService PhotolabSettingsService => WebSiteGlobal.Container.GetInstance<IPhotolabSettingsService>();
        protected DeliveryWindowSettings DeliverySettings => PhotolabSettingsService.GetSettings<DeliveryWindowSettings>(CurrentFrontend);
        protected bool IsNewDelivery => DeliverySettings?.IsNewDeliveryWindowEnabled ?? false;
        public IEnumerable<IShippable> ShippableItems { get; set; }

		protected Order CurrentOrder { get; set; }

	    protected BaseShippingSelectorControl()
		{
        }
        public abstract void InitControl(Shipping shipping, Order order, IReadOnlyCollection<IPurchasedItem> items);
        public abstract OrderAddress GeOrderAddress();

        public abstract bool ValidateInput();
    }
}
