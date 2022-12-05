
using Newtonsoft.Json;

namespace Photoprint.Core.Models
{

    public class GooglePlace
    {
        [JsonProperty("place_id")]
        public string PlaceId { get; set; }

        [JsonProperty("types")]
        public string[] Types { get; set; }
    }

    public class GoogleGeometry
    {
        [JsonProperty("location")]
        public GoogleLocation Location { get; set; }

        [JsonProperty("viewport")]
        public GoogleViewport Viewport { get; set; }
    }

    public class GoogleLocation
    {
        [JsonProperty("lat")]
        public string Latitude { get; set; }

        [JsonProperty("lng")]
        public string Longitude { get; set; }
    }

    public class GoogleViewport
    {
        [JsonProperty("northeast")]
        public GoogleLocation NorthEast { get; set; }

        [JsonProperty("southwest")]
        public GoogleLocation SouthWest { get; set; }
    }
}
