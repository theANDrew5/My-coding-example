using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class YandexDeliverySubmitOrderResponse
    {
        [JsonProperty("orderId")]
        public int OrderId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("errors")]
        public YandexDeliveryError[] Errors { get; set; }

        [JsonProperty("violations")]
        public YandexDeliveryViolation[] Violations { get; set; }
    }

    public class YandexDeliveryError
    {
        [JsonProperty("field")]
        public int Field { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
