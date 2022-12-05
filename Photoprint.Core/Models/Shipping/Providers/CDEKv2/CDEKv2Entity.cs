using Newtonsoft.Json;
using System.Collections.Generic;

namespace Photoprint.Core.Models
{
    public class CDEKv2Entity
    {
        [JsonProperty("uuid")]
        public string Uuid { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("is_return")]
        public bool IsReturn { get; set; }
        
        [JsonProperty("number")]
        public string Number { get; set; }
        
        [JsonProperty("cdek_number")]
        public string CdekNumber { get; set; }
        
        [JsonProperty("tariff_code")]
        public int TariffCode { get; set; }
        
        [JsonProperty("sender")]
        public CDEKv2Sender Sender { get; set; }
        
        [JsonProperty("recipient")]
        public CDEKv2Recipient Recipient { get; set; }
        
        [JsonProperty("from_location")]
        public CDEKv2Location FromLocation { get; set; }
        
        [JsonProperty("to_location")]
        public CDEKv2Location ToLocation { get; set; }
        
        [JsonProperty("packages")]
        public CDEKv2Package[] Packages { get; set; }
        
        [JsonProperty("recipient_currency")]
        public string RecipientCurrency { get; set; }
        
        [JsonProperty("items_cost_currency")]
        public string ItemsCostCurrency { get; set; }
        
        [JsonProperty("comment")]
        public string Comment { get; set; }
        
        [JsonProperty("shop_seller_name")]
        public string ShopSellerName { get; set; }
        
        [JsonProperty("statuses")]
        public IReadOnlyCollection<CDEKv2Status> Statuses { get; set; }
        
        [JsonProperty("errors")]
        public object[] Errors { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("copy_count")]
        public string CopyCount { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }
    }
}
