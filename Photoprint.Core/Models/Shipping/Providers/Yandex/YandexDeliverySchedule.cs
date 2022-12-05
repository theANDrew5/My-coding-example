using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class YandexDeliverySchedule
    {
        [JsonProperty("day")]
        public int Day { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }
    }

    //public class YandexDeliverySchedule
    //{
    //    public int day { get; set; }
    //    public string timeFrom { get; set; }
    //    public string timeTo { get; set; }
    //    public int id { get; set; }
    //}

}
