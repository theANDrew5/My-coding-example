using System.Collections.Generic;
using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class CdekCity
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("cityName")]
        public string Title { get; set; }
        [JsonProperty("regionName")]
        public string Region { get; set; }
        [JsonProperty("countryName")]
        public string Country { get; set; }
    }
}
