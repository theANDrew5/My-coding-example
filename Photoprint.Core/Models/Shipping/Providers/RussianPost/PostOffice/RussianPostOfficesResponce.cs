using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
   public class RussianPostofficesResponce
    {
        [JsonProperty("is-matched")]
        public bool IsMatched { get; set; }

        [JsonProperty("postoffices")]
        public string[] Postoffices { get; set; }
    }
}
