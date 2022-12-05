using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostReturnShipmentWithoutDirectRequest
    {
        [JsonProperty("address-from")]
        public RussianPostAddress AddressFrom { get; set; }

        [JsonProperty("address-to")]
        public RussianPostAddress AddressTo { get; set; }

        [JsonProperty("insrvalue")]
        public int Insrvalue { get; set; }

        [JsonProperty("mail-type")]
        public RussianPostMailType? MailType { get; set; }

        [JsonProperty("order-num")]
        public string OrderNum { get; set; }

        [JsonProperty("postoffice-code")]
        public string PostofficeCode { get; set; }

        [JsonProperty("recipient-name")]
        public string RecipientName { get; set; }

        [JsonProperty("sender-name")]
        public string SenderName { get; set; }
    }
}

