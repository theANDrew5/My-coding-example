using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Photoprint.Core.Models
{


    public class YandexDeliveryErrorStatus
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("datetime")]
        public string Date { get; set; }
    }
}
