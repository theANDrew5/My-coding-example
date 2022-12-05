using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostUserSettings
    {
        [JsonProperty("account-admin")]
        public bool IsAccountAdmin { get; set; }

        [JsonProperty("accounts")]
        public RussianPostAccount[] Accounts { get; set; }

        [JsonProperty("address")]
        public RussianPostAddress Address { get; set; }

        [JsonProperty("admin-hid")]
        public string AdminHid { get; set; }

        [JsonProperty("agreement-number")]
        public string AgreementNumber { get; set; }

        [JsonProperty("api_enabled")]
        public bool ApiEnabled { get; set; }

        [JsonProperty("apig_access_token")]
        public string ApiAccessToken { get; set; }

        [JsonProperty("blocked")]
        public bool Blocked { get; set; }

        [JsonProperty("brand-name")]
        public string BrandName { get; set; }

        [JsonProperty("contact-email")]
        public string ContactEmail { get; set; }

        [JsonProperty("contact-phone")]
        public int ContactPhone { get; set; }

        [JsonProperty("def-envelope-type")]
        public string DefEnvelopeType { get; set; }

        [JsonProperty("def-payment-method")]
        public string DefPaymentMethod { get; set; }

        [JsonProperty("espp-code")]
        public string EsppCode { get; set; }

        [JsonProperty("hid")]
        public string Hid { get; set; }

        [JsonProperty("legal-hid")]
        public string LegalHid { get; set; }

        [JsonProperty("mail-rank")]
        public string MailRank { get; set; }

        [JsonProperty("mailing-option")]
        public MailingOption[] MailingOption { get; set; }

        [JsonProperty("org-inn")]
        public string OrgINN { get; set; }

        [JsonProperty("org-kpp")]
        public string OrgKPP { get; set; }

        [JsonProperty("org-name")]
        public string OrgName { get; set; }

        [JsonProperty("planned-monthly-number")]
        public int PlannedMonthlyNumber { get; set; }

        [JsonProperty("print-type")]
        public string PrintType { get; set; }

        [JsonProperty("regions")]
        public string[] Regions { get; set; }

        [JsonProperty("sender-name")]
        public string SenderName { get; set; }
    }

    public class MailingOption
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public (string key, string value)[] Value { get; set; }
    }

}
