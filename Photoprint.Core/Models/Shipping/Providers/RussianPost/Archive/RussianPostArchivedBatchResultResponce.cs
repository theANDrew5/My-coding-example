using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostArchivedBatchResultResponce
    {
        [JsonProperty("batch-name")]
        public string BatchName { get; set; }

        [JsonProperty("error-code")]
        public string ErrorCode { get; set; }
    }
}
