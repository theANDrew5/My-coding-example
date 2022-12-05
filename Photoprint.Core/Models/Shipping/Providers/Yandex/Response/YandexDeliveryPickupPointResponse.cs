using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryPickupPointResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("partnerId")]
        public string PartnerId { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("instruction")]
        public string Instruction { get; set; }
        
        [JsonProperty("address")]
        public YandexDeliveryAddress Address { get; set; }

        [JsonProperty("schedule")]
        public YandexDeliverySchedule[] Schedule { get; set; }

        [JsonProperty("phones")]
        public YandexDeliveryPhone[] Phones { get; set; }
        
        [JsonProperty("contact")]
        public YandexDeliveryContact Contact { get; set; }

    }

    public class YandexDeliveryPhone
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("internalNumber")]
        public string InternalNumber { get; set; }
    }

}
