
using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostAddress
    {
        [JsonProperty("address-type")]
        public RussianPostAddressType? AddressType { get; set; }

        [JsonProperty("area")]
        public string Area { get; set; }

        [JsonProperty("building")]
        public string Building { get; set; }

        [JsonProperty("corpus")]
        public string Corpus { get; set; }

        [JsonProperty("hotel")]
        public string Hotel { get; set; }

        [JsonProperty("house")]
        public string House { get; set; }

        [JsonProperty("index")]
        public string Index { get; set; }

        [JsonProperty("letter")]
        public string Letter { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("num-address-type")]
        public string NumAddressType { get; set; }

        [JsonProperty("place")]
        public string Place { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("room")]
        public string Room { get; set; }

        [JsonProperty("slash")]
        public string Slash { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("manual-address-input")]
        public bool ManualAddressInput { get; set; }

        [JsonProperty("office")]
        public string Office { get; set; }

        [JsonProperty("vladenie")]
        public string Vladenie { get; set; }

        public RussianPostAddress(string index, string region, string place, string street = "", string house = "")
        {
            Index = index;
            Place = place;
            Region = region;
            Street = street;
            House = house;
        }

        public RussianPostAddress(RussianPostNormalizedAddressResponce data)
        {            
            AddressType = data.AddressType;
            Area = data.Area; 
            Building = data.Building; 
            Corpus = data.Corpus; 
            House = data.House; 
            Index = data.Index; 
            Location = data.Location; 
            Place = data.Place; 
            Region = data.Region; 
            Street = data.Street;
        }
    }
}
