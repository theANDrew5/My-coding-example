using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class EcontLocation
    {
        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }
    }
}
