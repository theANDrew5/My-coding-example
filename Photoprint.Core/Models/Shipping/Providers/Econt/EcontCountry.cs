using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class EcontCountry
    {
        [JsonProperty("code2")]
        public string Code2 { get; set; }

        [JsonProperty("code3")]
        public string Code3 { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("nameEn")]
        public string NameEn { get; set; }

    }

}
