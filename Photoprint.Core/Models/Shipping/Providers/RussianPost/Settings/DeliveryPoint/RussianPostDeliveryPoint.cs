using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostDeliveryPoint
    {
       [JsonProperty("enabled")] 
        public bool Enabled { get; set; }

       [JsonProperty("operator-postcode")] 
        public string OperatorPostcode { get; set; }

        [JsonProperty("ops-address")]
        public string OpsAddress { get; set; }
    }

}
