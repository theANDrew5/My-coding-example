using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryDeleteOrderResponse
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

    }


}
