using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class CDEKv2Order
    {
        [JsonProperty("order_uuid")]
        public string Uuid { get; set; }
    }
}
