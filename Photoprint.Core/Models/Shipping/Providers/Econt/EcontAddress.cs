using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class EcontAddress
    {
        [JsonProperty("city")]
        public EcontCity City { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("num")]
        public string House { get; set; }

        [JsonProperty("other")]
        public string Other { get; set; }

        [JsonProperty("fullAddress")]
        public string FullAddress { get; set; }

        [JsonProperty("quarter")]
        public string Quarter { get; set; }

        [JsonProperty("location")]
        public EcontLocation Location { get; set; }
    }
}
