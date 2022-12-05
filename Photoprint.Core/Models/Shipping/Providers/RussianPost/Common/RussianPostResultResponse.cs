using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
   public class RussianPostResultResponse
    {
        [JsonProperty("errors")]
        public RussianPostError[] Errors { get; set; }

        [JsonProperty("result-ids")]
        public string[] ResultIds { get; set; }
    }
}
