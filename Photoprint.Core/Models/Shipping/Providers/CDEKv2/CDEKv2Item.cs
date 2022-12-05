using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class CDEKv2Item
    {
        [JsonProperty("ware_key")]
        public string WareKey { get; set; }
        
        [JsonProperty("payment")]
        public CDEKv2Payment Payment { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("cost")]
        public decimal? Cost { get; set; }
        
        [JsonProperty("amount")]
        public int? Amount { get; set; }
        
        [JsonProperty("weight")]
        public int Weight { get; set; }
        
        [JsonProperty("url")]
        public string Url { get; set; }
        
        [JsonProperty("weight_gross")]
        public int? WeightGross { get; set; }
    }
}
