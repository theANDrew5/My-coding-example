using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryService
    {
        [JsonProperty("code")]
        [JsonConverter(typeof(StringEnumConverter))]
        public YandexDeliveryServiceCode Code { get; set; }

        [JsonProperty("cost")]
        public double Cost { get; set; }

        [JsonProperty("customerPay")]
        public bool CustomerPay { get; set; }
    }
}
