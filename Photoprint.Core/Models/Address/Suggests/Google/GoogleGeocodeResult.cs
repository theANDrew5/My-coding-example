using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class GoogleGeocodeResult: GooglePlace
    {
        [JsonProperty("address_components")]
        public AddressComponents [] AddressComponents { get; set; }

        [JsonProperty("formatted_address")]
        public string FormattedAddress { get; set; }

        [JsonProperty("geometry")]
        public GoogleGeometry Geometry { get; set; }

    }

    public class AddressComponents
    {
        [JsonProperty("long_name")]
        public string LongName { get; set; }
        [JsonProperty("short_name")]
        public string ShortName { get; set; }
        [JsonProperty("types")]
        public string[] Types { get; set; }
    }
}
