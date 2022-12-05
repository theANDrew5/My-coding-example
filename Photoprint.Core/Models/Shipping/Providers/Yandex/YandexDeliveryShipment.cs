using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryShipment
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("date")]
        public string Date { get; set; }
        
        [JsonProperty("warehouseFrom")]
        public string WarehouseFrom { get; set; }
        
        [JsonProperty("warehouseTo")]
        public string WarehouseTo { get; set; }
        
        [JsonProperty("partnerTo")]
        public string PartnerTo { get; set; }
    }

}
