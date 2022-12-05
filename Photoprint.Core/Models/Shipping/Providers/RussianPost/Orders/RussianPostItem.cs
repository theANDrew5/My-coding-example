using Newtonsoft.Json;
using System.Collections.Generic;

namespace Photoprint.Core.Models
{
    public class RussianPostGoods
    {
        [JsonProperty("items")]
        public RussianPostItem[] Items { get; set; }

        public RussianPostGoods (RussianPostItem[] items)
        {
            Items = items;
        }
    }

    public class RussianPostItem
    {
        [JsonProperty("code", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Code { get; set; }

        [JsonProperty("country-code", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int CountryCode { get; set; }

        [JsonProperty("customs-declaration-number", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CustomsDeclarationNumber { get; set; }

        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("excise", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Excise { get; set; }

        [JsonProperty("goods-type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string GoodsType { get; set; }

        [JsonProperty("insrvalue", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Insrvalue { get; set; }

        [JsonProperty("item-number", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ItemNumber { get; set; }

        [JsonProperty("lineattr", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Lineattr { get; set; }

        [JsonProperty("payattr", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Payattr { get; set; }

        [JsonProperty("quantity", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Quantity { get; set; }

        [JsonProperty("supplier-inn", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SupplierINN { get; set; }

        [JsonProperty("supplier-name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SupplierName { get; set; }

        [JsonProperty("supplier-phone", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SupplierPhone { get; set; }

        [JsonProperty("value", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Value { get; set; }

        [JsonProperty("vatrate", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Vatrate { get; set; }

        [JsonProperty("weight", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Weight { get; set; }

        public RussianPostItem(int quantity, string description)
        {
            Quantity = quantity;
            Description = description;
        }
    }
}
