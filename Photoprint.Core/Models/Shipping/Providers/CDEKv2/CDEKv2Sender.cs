using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class CDEKv2Sender
    {
        [JsonProperty("company")]
        public string Company { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("phones")]
        public CDEKv2Phone[] Phones { get; set; }
    }
}
