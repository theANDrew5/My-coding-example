using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class EcontResponse
    {
        [JsonProperty("label")]
        public EcontShipmentStatus Label { get; set; }

        [JsonProperty("error")]
        public EcontError Error { get; set; }

        [JsonProperty("results")]
        public EcontResult[] Results { get; set; }
    }


    public class EcontResult
    {
        [JsonProperty("shipmentNum")]
        public string ShipmentNum { get; set; }
        [JsonProperty("error")]
        public EcontError Error { get; set; }
    }

    public class EcontStatusResponse
    {
        [JsonProperty("shipmentStatuses")]
        public EcontStatus[] ShipmentStatuses { get; set; }
    }

    public class EcontStatus
    {
        [JsonProperty("status")]
        public EcontShipmentStatus Status { get; set; }

        [JsonProperty("error")]
        public EcontError Error { get; set; }
    }



}
