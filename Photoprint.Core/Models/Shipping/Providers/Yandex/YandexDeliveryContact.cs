using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryContact
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public YandexDeliveryContactType Type { get; set; }
        
        [JsonProperty("phone")]
        public string Phone { get; set; }
        
        [JsonProperty("additional")]
        public string Additional { get; set; }
        
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        
        [JsonProperty("middleName")]
        public string MiddleName { get; set; }
        
        [JsonProperty("lastName")]
        public string LastName { get; set; }

    }
}
