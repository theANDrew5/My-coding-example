using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
   public  class RussianPostDataRequest
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
