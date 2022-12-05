using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class CDEKv2Location
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("sub_region")]
        public string SubRegion { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("address_full")]
        public string AddressFull { get; set; }

        [JsonProperty("coutry")]
        public string Country { get; set; }

        
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
        
        [JsonProperty("region_code")]
        public string RegionCode { get; set; }
        
        [JsonProperty("city_code")]
        public string CityCode { get; set; }
        
        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }
    }
}
