using Newtonsoft.Json.Linq;
using Photoprint.Core.Models;

namespace Photoprint.WebSite.API.Models.Delivery
{
    public class DeliveryGetPriceModelPriceReponse
    {
        public int ShippingId { get; set; }
        public int? ShippingAddressId { get; set; }
        public JObject CalculationResult { get; set; }
    }

}
