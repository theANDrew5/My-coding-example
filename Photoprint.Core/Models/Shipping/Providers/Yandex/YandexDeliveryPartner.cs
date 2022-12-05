using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryPartner
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("code")]
        public string Code { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("partnerType")]
        public string PartnerType { get; set; }
        
        [JsonProperty("logoUrl")]
        public string LogoUrl { get; set; }
    }
}
