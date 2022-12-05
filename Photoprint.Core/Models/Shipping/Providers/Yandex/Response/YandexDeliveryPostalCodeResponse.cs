using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryPostalCodeResponse
    {
        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }
    }
}
