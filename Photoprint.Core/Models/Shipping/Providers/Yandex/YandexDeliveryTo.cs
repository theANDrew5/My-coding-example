using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryTo
    {
        [JsonProperty("location")]
        public string Location { get; set; }
        /// <summary>
        /// Идентификатор населенного пункта
        /// </summary>
        [JsonProperty("geoId")]
        public string GeoId { get; set; }
        
        [JsonProperty("pickupPointIds")]
        public int[] PickupPointIds { get; set; }

        /// <summary>
        ///  Почтовый индекс адреса, обязателен при доставке почтой
        /// </summary>
        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }
    }
}
