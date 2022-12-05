using System.Collections.Generic;
using Photoprint.Core.Models;

namespace Photoprint.WebSite.API.Models.Delivery
{
    public class DeliveryGetPriceRequest : BaseDeliveryRequest
    {
        public IReadOnlyList<ShippingAddressDTO> Addresses { get; set; }
    }
}