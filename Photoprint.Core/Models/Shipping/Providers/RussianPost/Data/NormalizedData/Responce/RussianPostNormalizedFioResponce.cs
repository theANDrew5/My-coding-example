using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostNormalizedFioResponce : RussianPostNormalizedDataResponce<RussianPostQualityNameCode>
    {
        [JsonProperty("original-fio")]
        public string OriginalFio { get; set; }

        [JsonProperty("middle-name")]
        public string MiddleName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("valid")]
        public bool? Valid { get; set; }
    }
}
