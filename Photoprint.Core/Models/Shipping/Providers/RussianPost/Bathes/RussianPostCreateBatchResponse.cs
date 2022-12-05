using Newtonsoft.Json;

namespace Photoprint.Core.Models
{ 
    public class RussianPostCreateBatchResponse
    {
        [JsonProperty("batches")]
        public RussianPostBatch[] Batches { get; set; }

        [JsonProperty("errors")]
        public RussianPostError[] Errors { get; set; }

        [JsonProperty("result-ids")]
        public int[] ResultIds { get; set; }
    }
}
