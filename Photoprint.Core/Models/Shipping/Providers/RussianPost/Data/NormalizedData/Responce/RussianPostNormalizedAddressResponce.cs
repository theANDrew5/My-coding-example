using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostNormalizedAddressResponce : RussianPostNormalizedDataResponce <RussianPostQualityAddressCode>
    {
        [JsonProperty("original-address")]
        public string OriginalAddress { get; set; }

        [JsonProperty("address-type")]
        public RussianPostAddressType? AddressType { get; set; }

        [JsonProperty("area")]
        public string Area { get; set; }

        [JsonProperty("building")]
        public string Building { get; set; }

        [JsonProperty("corpus")]
        public string Corpus { get; set; }

        [JsonProperty("house")]
        public string House { get; set; }

        [JsonProperty("index")]
        public string Index { get; set; }

        [JsonProperty("location ")]
        public string Location { get; set; }

        [JsonProperty("place")]
        public string Place { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("validation-code")]
        public RussianPostValidationCode? ValidationCode { get; set; }
    }
}
