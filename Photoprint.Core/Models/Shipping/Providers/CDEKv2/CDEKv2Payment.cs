using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class CDEKv2Payment
    {
        [JsonProperty("value")]
        public decimal? Value { get; set; }
        
        [JsonProperty("vat_sum")]
        public decimal? VatSum { get; set; }
    }
}
