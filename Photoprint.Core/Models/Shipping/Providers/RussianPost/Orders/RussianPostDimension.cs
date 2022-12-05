using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostDimension
    {
        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }
    }
}
