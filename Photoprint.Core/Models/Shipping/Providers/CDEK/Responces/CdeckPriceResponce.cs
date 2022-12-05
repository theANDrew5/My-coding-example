using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class CdeckPriceResponce
    {
        [JsonProperty("result")]
        public CdekPrice Result { get; set; }
        [JsonProperty("error")]
        public CdekError[] Errors { get; set; }
    }

    public class CdekError
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("text")]
        public string Message { get; set; }
    }
}
