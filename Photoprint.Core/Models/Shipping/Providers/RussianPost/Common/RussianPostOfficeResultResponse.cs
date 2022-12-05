using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostofficeResultResponse
    {
        [JsonProperty("error-code")]
        public string ErrorCode { get; set; }

        [JsonProperty("f103-sent")]
        public bool F103Sent { get; set; }

    }
}
