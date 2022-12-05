using Photoprint.Core.Models;
using System;

namespace Photoprint.WebSite.API.Models.Delivery
{
    public class DeliveryGetAvailableShippingTypessRequest : BaseDeliveryRequest
    {
        public CityAddress City { get; set; }
    }
}