using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryCost
    {
        
        [JsonProperty("paymentMethod")]
        [JsonConverter(typeof(StringEnumConverter))]
        public YandexDeliveryPaymentMethod PaymentMethod { get; set; }
        
        [JsonProperty("assessedValue")]
        public decimal AssessedValue { get; set; }
        
        [JsonProperty("fullyPrepaid")]
        public bool FullyPrepaid { get; set; }
        
        [JsonProperty("manualDeliveryForCustomer")]
        public decimal ManualDeliveryForCustomer { get; set; }
    }

}
