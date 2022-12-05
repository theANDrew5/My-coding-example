using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostFioRequest: RussianPostDataRequest
    {
        [JsonProperty("original-fio")]
        public string OriginalFio { get; set; }
    }
}
