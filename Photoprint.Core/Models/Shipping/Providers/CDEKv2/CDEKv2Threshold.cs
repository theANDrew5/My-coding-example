using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class CDEKv2Threshold
    {
        [JsonProperty("threshold")]
        public int? Threshold { get; set; }

        [JsonProperty("sum")]
        public decimal? Sum { get; set; }
    }
}
