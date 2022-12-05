using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class YandexGoErrorInfo
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}

