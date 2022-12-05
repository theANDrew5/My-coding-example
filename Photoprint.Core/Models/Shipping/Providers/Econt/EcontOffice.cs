using Newtonsoft.Json;
using System.Collections.Generic;

namespace Photoprint.Core.Models
{
    public class EcontOffices
    {
        [JsonProperty("offices")]
        public List<EcontOffice> Offices { get; set; }
    }

    public class EcontOffice
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("code")]
        public string Code { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("nameEn")]
        public string NameEn { get; set; }
        
        [JsonProperty("phones")]
        public string[] Phones { get; set; }
        
        [JsonProperty("address")]
        public EcontAddress Address { get; set; }
        
        [JsonProperty("info")]
        public string Info { get; set; }
        
        [JsonProperty("currency")]
        public string Currency { get; set; }
        
        [JsonProperty("language")]
        public string Language { get; set; }
        
        [JsonProperty("shipmentTypes")]
        public string[] ShipmentTypes { get; set; }
        
        [JsonProperty("partnerCode")]
        public string PartnerCode { get; set; }
        
        [JsonProperty("hubCode")]
        public string HubCode { get; set; }
        
        [JsonProperty("hubName")]
        public string HubName { get; set; }
        
        [JsonProperty("hubNameEn")]
        public string HubNameEn { get; set; }
    }
}
