using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryPoint
    {
        [JsonProperty("from")]
        public double From { get; set; }

        [JsonProperty("to")]
        public double To { get; set; }
    }
}
