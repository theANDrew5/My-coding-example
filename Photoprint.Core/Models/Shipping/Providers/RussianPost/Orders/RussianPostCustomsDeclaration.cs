using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostCustomsDeclaration
    {
        [JsonProperty("certificate-number")]
        public string CertificateNumber { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; } // https://otpravka.pochta.ru/specification#/dictionary-currencies

        [JsonProperty("customs-entries")]
        public RussianPostCustomsEntries[] CustomsEntries { get; set; }

        [JsonProperty("entries-type")]
        public RussianPostEntriesType? EntriesType { get; set; }

        [JsonProperty("invoice-number")]
        public string InvoiceNumber { get; set; }

        [JsonProperty("license-number")]
        public string LicenseNumber { get; set; }

        [JsonProperty("with-certificate")]
        public bool WithCertificate { get; set; }

        [JsonProperty("with-invoice")]
        public bool WithInvoice { get; set; }

        [JsonProperty("with-license")]
        public bool WithLicense { get; set; }


        public RussianPostCustomsDeclaration(string currency, RussianPostCustomsEntries[] customsEntries)
        {
            Currency = currency;
            CustomsEntries = customsEntries;
        }
    }
}
