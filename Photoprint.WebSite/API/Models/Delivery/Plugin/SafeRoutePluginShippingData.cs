using System.Collections.Generic;

namespace Photoprint.WebSite.API.Models.Delivery
{
    public class SafeRoutePluginShippingData : BasePluginShippingData
    {
        public IReadOnlyCollection<object> Products { get; set; }
        public double TotalWeight { get; set; }
        public bool IsShippingPricePaidSeparately { get; set; }
    }
}
