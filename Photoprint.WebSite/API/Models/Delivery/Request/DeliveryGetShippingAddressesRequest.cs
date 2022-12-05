using System.Collections.Generic;
using Photoprint.Core.Models;

namespace Photoprint.WebSite.API.Models.Delivery
{
    public class DeliveryGetShippingAddressesRequest : BaseDeliveryRequest
    {
        public CityAddress City { get; set; }
        public DeliveryDisplayType Type { get; set; }
        public IReadOnlyCollection<int> ShippingIds { get; set; }
    }
}