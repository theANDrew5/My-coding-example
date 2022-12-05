using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostAddressRequest: RussianPostDataRequest
    {
        [JsonProperty("original-address")]
        public string OriginalAddress { get; set; }
    }
}
