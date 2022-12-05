using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class CDEKv2Recipient
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("phones")]
        public CDEKv2Phone[] Phones { get; set; }
    }
}
