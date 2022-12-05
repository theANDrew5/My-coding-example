using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostNormalizedDataResponce <T>
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("quality-code")]
        public T QualityCode { get; set; }
    }

}
