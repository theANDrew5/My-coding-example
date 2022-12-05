using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostAccount
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("blocked")]
        public bool Blocked { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("is-admin")]
        public bool IsAdmin { get; set; }

        [JsonProperty("legal-hid")]
        public string LegalHid { get; set; }

        [JsonProperty("org-inn")]
        public string OrgINN { get; set; }

        [JsonProperty("org-name")]
        public string OrgName { get; set; }
    }
}
