using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class CDEKv2Response
    {
        [JsonProperty("entity")]
        public CDEKv2Entity Entity { get; set; }

        [JsonProperty("requests")]
        public CDEKv2Request[] Requests { get; set; }
    }
}


