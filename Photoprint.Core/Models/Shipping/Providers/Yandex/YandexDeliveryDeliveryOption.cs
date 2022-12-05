using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryDeliveryOption
    {
        [JsonProperty("tariffId")]
        public string TariffId { get; set; }
        
        [JsonProperty("delivery")]
        public decimal Delivery { get; set; }
        
        [JsonProperty("deliveryForCustomer")]
        public string DeliveryForCustomer { get; set; }
        
        [JsonProperty("partnerId")]
        public string PartnerId { get; set; }
        
        [JsonProperty("deliveryIntervalId")]
        public string DeliveryIntervalId { get; set; }
        
        [JsonProperty("calculatedDeliveryDateMin")]
        public string CalculatedDeliveryDateMin { get; set; }
        
        [JsonProperty("calculatedDeliveryDateMax")]
        public string CalculatedDeliveryDateMax { get; set; }
        
        [JsonProperty("services")]
        public YandexDeliveryService[] Services { get; set; }
    }
}
