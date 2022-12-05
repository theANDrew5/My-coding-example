using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostofficeServiceResponce
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("group-id")]
        public int GroupId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
