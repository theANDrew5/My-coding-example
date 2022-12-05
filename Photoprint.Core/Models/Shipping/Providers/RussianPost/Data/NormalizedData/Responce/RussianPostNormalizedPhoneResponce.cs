using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostNormalizedPhoneResponce : RussianPostNormalizedDataResponce<RussianPostQualityPhoneCode>
    {
        [JsonProperty("original-phone")]
        public string OriginalPhone { get; set; }

        [JsonProperty("phone-city-code")]
        public string PhoneCityCode { get; set; }

        [JsonProperty("phone-country-code")]
        public string PhoneCountryCode { get; set; }

        [JsonProperty("phone-extension")]
        public string PhoneExtension { get; set; }

        [JsonProperty("phone-number")]
        public string PhoneNumber { get; set; }
    }
}
