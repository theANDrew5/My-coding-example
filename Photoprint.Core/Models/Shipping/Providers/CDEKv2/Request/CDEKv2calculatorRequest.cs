using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class CDEKv2CalculatorRequest
    {
        [JsonProperty("tariff_code")]
        public int? TariffCode { get; set; }
        
        [JsonProperty("from_location")]
        public CDEKv2Location FromLocation { get; set; }
        
        [JsonProperty("to_location")]
        public CDEKv2Location ToLocation { get; set; }
        
        [JsonProperty("packages")]
        public CDEKv2Package[] Packages { get; set; }
    }
}
