using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class EcontPerson
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("phones")]
        public string[] Phones { get; set; }
    }

}
