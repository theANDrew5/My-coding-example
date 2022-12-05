using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class CDEKv2BarcodeRequest
    {
        [JsonProperty("orders")]
        public CDEKv2Order[] Orders { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }
    }
}
