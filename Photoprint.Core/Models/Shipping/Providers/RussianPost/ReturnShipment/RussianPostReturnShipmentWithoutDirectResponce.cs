using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostReturnShipmentWithoutDirectResponce
    {
        [JsonProperty("errors")]
        public RussianPostError[] Errors { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }

        [JsonProperty("return-barcode")]
        public string ReturnBarcode { get; set; }
    }
}
