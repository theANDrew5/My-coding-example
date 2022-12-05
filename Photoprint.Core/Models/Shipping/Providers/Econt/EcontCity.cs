using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class EcontCity
    {
        [JsonProperty("country")]
        public EcontCountry Country { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("postCode")]
        public string PostCode { get; set; }
        
        [JsonProperty("nameEn")]
        public string NameEn { get; set; }
        
        [JsonProperty("regionName")]
        public string RegionName { get; set; }
        
        [JsonProperty("regionNameEn")]
        public string RegionNameEn { get; set; }
        
        [JsonProperty("phoneCode")]
        public string PhoneCode { get; set; }
        
        [JsonProperty("location")]
        public EcontLocation Location { get; set; }
        
    }
}
