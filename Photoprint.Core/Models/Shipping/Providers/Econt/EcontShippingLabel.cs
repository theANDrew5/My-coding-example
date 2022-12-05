using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class EcontShippingLabel
    {
        [JsonProperty("label")]
        public EcontLabel Label { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }
    }
}
