
using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class GooglePlaceDetails: GooglePlace
    {
        [JsonProperty("address_components")]
        public AddressComponents [] AddressComponents { get; set; }

        [JsonProperty("formatted_address")]
        public string FormattedAddress { get; set; }

        [JsonProperty("geometry")]
        public GoogleGeometry Geometry { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
    
}
