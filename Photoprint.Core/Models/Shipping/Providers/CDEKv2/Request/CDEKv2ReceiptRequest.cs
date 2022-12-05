using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class CDEKv2ReceiptRequest
    {
        [JsonProperty("orders")]
        public CDEKv2Order[] Orders { get; set; }

        [JsonProperty("copy_count")]
        public int? CopyCount { get; set; }
    }
}
