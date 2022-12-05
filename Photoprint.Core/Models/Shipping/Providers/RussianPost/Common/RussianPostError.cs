using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostError
    {
        [JsonProperty("error-codes")]
        public RussianPostErrorCode[] ErrorCodes { get; set; }
        [JsonProperty("position")]
        public int? Position { get; set; }// Индекс ошибочной записи в массиве Заказов(Отправлений)
    }

    public class RussianPostErrorCode
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("details")]
        public string Details { get; set; }
        [JsonProperty("position")]
        public int? Position { get; set; }
    }

    public class RussianPostGetPriceError
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("desc")]
        public string Description { get; set; }
        [JsonProperty("sub-code")]
        public string SubCode { get; set; }
    }
}
