using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryViolation
    {
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }
        
        [JsonProperty("extraService")]
        public string ExtraService { get; set; }
        
        [JsonProperty("serviceType")]
        public string ServiceType { get; set; }
        
        [JsonProperty("currentValue")]
        public object CurrentValue { get; set; }
        
        [JsonProperty("actualValue")]
        public object ActualValue { get; set; }
        
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }
}
