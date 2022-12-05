using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostReturnShipmentResponce
    {
            [JsonProperty("errors")]
            public RussianPostError[] Errors { get; set; }

            [JsonProperty("return-barcode")]
            public string ReturnBarcode { get; set; }
    }
}
