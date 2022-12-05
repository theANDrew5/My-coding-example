using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryDimensions
    {
        [JsonProperty("length")]
        public int Length { get; set; }
        
        [JsonProperty("width")]
        public int Width { get; set; }
        
        [JsonProperty("height")]
        public int Height { get; set; }
        
        [JsonProperty("weight")]
        public double Weight { get; set; }
    }
}
