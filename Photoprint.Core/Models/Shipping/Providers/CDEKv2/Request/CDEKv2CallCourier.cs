using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class CDEKv2CallCourier
    {
        [JsonProperty("order_uuid")]
        public string OrderUuid { get; set; }

        [JsonProperty("intake_date")]
        public string IntakeDate { get; set; }
        
        [JsonProperty("intake_time_from")]
        public string IntakeTimeFrom { get; set; }
        
        [JsonProperty("intake_time_to")]
        public string IntakeTimeTo { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("weight")]
        public int? Weight { get; set; }
        
        [JsonProperty("length")]
        public int? Length { get; set; }
        
        [JsonProperty("width")]
        public int? Width { get; set; }
        
        [JsonProperty("height")]
        public int? Height { get; set; }
        
        [JsonProperty("comment")]
        public string Comment { get; set; }
        
        [JsonProperty("sender")]
        public CDEKv2Sender Sender { get; set; }
        
        [JsonProperty("from_location")]
        public CDEKv2Location FromLocation { get; set; }
        
        [JsonProperty("need_call")]
        public bool NeedCall { get; set; }
    }
}
