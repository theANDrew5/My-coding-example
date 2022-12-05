using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostReturnShipmentRequest
    {
        [JsonProperty("direct-barcode")]
        public string DirectBarcode { get; set; }

        [JsonProperty("mail-type")]
        public RussianPostMailType? MailType { get; set; }
    }
}
