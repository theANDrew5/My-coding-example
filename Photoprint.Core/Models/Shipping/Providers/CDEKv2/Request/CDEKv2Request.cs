using Newtonsoft.Json;
using System;

namespace Photoprint.Core.Models
{
    public class CDEKv2Request
    {
        [JsonProperty("request_uuid")]
        public string RequestUuid { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("state")]
        public CDEKv2StatusCode State { get; set; }
        
        [JsonProperty("date_time")]
        public DateTime DateTime { get; set; }
        
        [JsonProperty("errors")]
        public CDEKv2Error[] Errors { get; set; }
        
        [JsonProperty("warnings")]
        public object[] Warnings { get; set; }
    }
}
