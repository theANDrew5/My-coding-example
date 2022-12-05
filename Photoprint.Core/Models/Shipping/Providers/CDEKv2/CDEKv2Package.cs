using Newtonsoft.Json;
using System.Collections.Generic;

namespace Photoprint.Core.Models
{
    public class CDEKv2Package
    {
        [JsonProperty("number")]
        public string Number { get; set; }
        
        [JsonProperty("comment")]
        public string Comment { get; set; }
        
        [JsonProperty("items")]
        public IReadOnlyCollection<CDEKv2Item> Items { get; set; }

        [JsonProperty("height")]
        public int? HeightCm { get; set; }

        [JsonProperty("length")]
        public int? LengthCm { get; set; }
        
        [JsonProperty("width")]
        public int? WidthCm { get; set; }

        [JsonProperty("weight")]
        public int? WeightGram { get; set; }
    }
}
