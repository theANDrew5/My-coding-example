using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryAddress
    {
        [JsonProperty("geoId")]
        public string GeoId { get; set; }
        
        [JsonProperty("country")]
        public string Country { get; set; }
        
        [JsonProperty("region")]
        public string Region { get; set; }
        
        [JsonProperty("subRegion")]
        public string SubRegion { get; set; }
        
        [JsonProperty("locality")]
        public string Locality { get; set; }
        
        [JsonProperty("street")]
        public string Street { get; set; }
        
        [JsonProperty("house")]
        public string House { get; set; }
        
        [JsonProperty("housing")]
        public string Housing { get; set; }
        
        [JsonProperty("building")]
        public string Building { get; set; }
        
        [JsonProperty("apartment")]
        public string Apartment { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }
        
        [JsonProperty("locationId")]
        public string LocationId { get; set; }
        
        [JsonProperty("latitude")]
        public string Latitude { get; set; }
        
        [JsonProperty("longitude")]
        public string Longitude { get; set; }
        
        [JsonProperty("addressString")]
        public string AddressString { get; set; }

        [JsonProperty("shortAddressString")]
        public string ShortAddressString { get; set; }
        
    }
}
