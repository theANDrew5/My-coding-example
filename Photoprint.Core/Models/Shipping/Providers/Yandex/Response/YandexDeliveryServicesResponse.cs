using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryServicesResponse
    {
        /// <summary>
        /// Идентификатор партнера в системе Яндекс.Доставки.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("warehouses")]
        public YandexDeliveryWarehouses[] Warehouses { get; set; }
    }

    public class YandexDeliveryWarehouses
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }
    }
