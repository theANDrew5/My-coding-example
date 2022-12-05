using System.Collections.Generic;
using Newtonsoft.Json;

namespace Photoprint.Core.Models.Postnl
{
    public class PostnlResponse
    {
        [JsonProperty(PropertyName = "code")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "GetLocationsResult")]
        public Dictionary<string, string> Result { get; set; }

        [JsonProperty(PropertyName = "ResponseLocation")]
        public List<PostnlLocation> Data { get; set; }
    }
}
