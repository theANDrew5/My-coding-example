using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostCustomsEntries
    {
        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("country-code")]
        public int CountryCode { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("tnved-code")]
        public string Tnvedcode { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }

        [JsonProperty("weight")]
        public int Weight { get; set; }

        public RussianPostCustomsEntries(int amount, int countryCode, string description, string tnvedcode)
        {
            Amount = amount;
            CountryCode = countryCode;
            Description = description;
            Tnvedcode = tnvedcode;
        }

    }
}
