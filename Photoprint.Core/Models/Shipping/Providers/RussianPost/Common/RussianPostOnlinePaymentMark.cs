using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostOnlinePaymentMark
    {
        [JsonProperty("indexoper")]
        public string Indexoper { get; set; }

        [JsonProperty("online-payment-mark-id")]
        public string OnlinePaymentMarkId { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }
    }
}
