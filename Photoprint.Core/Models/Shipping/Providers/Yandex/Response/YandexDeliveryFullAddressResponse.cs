using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryFullAddressResponse
    {
        [JsonProperty("geoId")]
        public string GeoId { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("addressComponents")]
        public YandexDeliveryAddressComponents[] AddressComponents { get; set; }
    }

    public class YandexDeliveryAddressComponents
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("kind")]
        public YandexDeliveryAddressComponentType Kind { get; set; }
    }

}
