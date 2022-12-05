using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostPhoneRequest: RussianPostDataRequest
    {
        [JsonProperty("original-phone")]
        public string OriginalPhone { get; set; }

        [JsonProperty("area")]
        public string Area { get; set; } 

        [JsonProperty("place")]
        public string Place { get; set; }  // Город телефонного номера 

        [JsonProperty("region")]
        public string Region { get; set; } 
    }
}
