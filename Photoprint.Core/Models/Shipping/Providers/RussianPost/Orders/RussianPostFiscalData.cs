using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostFiscalData
    {
        [JsonProperty("customere-mail")]
        public string CustomereMail { get; set; }

        [JsonProperty("customer-inn")]
        public string CustomerINN { get; set; }

        [JsonProperty("customer-name")]
        public string CustomerName { get; set; }

        [JsonProperty("customer-phone")]
        public int CustomerPhone { get; set; }

        [JsonProperty("payment-amount")]
        public int PaymentAmount { get; set; }
    }
}
