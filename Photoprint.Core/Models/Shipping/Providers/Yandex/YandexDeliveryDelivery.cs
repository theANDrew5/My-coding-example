using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryDelivery
    {
        [JsonProperty("partner")]
        public YandexDeliveryPartner Partner { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public YandexDeliveryShippingType Type { get; set; }

        [JsonProperty("courierSchedule")]
        public YandexDeliveryCourierSchedule CourierSchedule { get; set; }

        [JsonProperty("calculatedDeliveryDateMin")]
        public string CalculatedDeliveryDateMin { get; set; }

        [JsonProperty("calculatedDeliveryDateMax")]
        public string CalculatedDeliveryDateMax { get; set; }
    }
    public class YandexDeliveryCourierSchedule
    {

        [JsonProperty("locationId")]
        public int? LocationId { get; set; }

        [JsonProperty("schedule")]
        public YandexDeliverySchedule[] Schedule { get; set; }
    }
}
