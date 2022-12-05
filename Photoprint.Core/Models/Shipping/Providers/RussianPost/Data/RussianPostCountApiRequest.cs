using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostCountApiRequest
    {
        [JsonProperty("allowed-count")]
        public int AllowedCount { get; set; }

        [JsonProperty("current-count")]
        public int CurrentCount { get; set; }

    }
}
