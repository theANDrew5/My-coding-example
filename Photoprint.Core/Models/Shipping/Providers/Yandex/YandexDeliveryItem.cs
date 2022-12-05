using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryItem
    {
        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("assessedValue")]
        public decimal AssessedValue { get; set; }

        [JsonProperty("tax")]
        [JsonConverter(typeof(StringEnumConverter))]
        public YandexDeliveryTaxType? Tax;

        [JsonProperty("dimensions")]
        public YandexDeliveryDimensions Dimensions { get; set; }

        public YandexDeliveryItem(OrderDetail detail, bool itemsPaid, VatRate vat)
        {
            ExternalId = detail.ItemId.ToString();
            Name = detail.Name;
            Count = detail.Quantity;
            Price = itemsPaid ? 0m : Math.Round((detail.Price - detail.DiscountsPriceTotal)/detail.Quantity, 2);
            AssessedValue = detail.ItemPriceExport;
            switch (vat)
            {
                case VatRate.None:
                    Tax = YandexDeliveryTaxType.NO_VAT;
                    break;
                case VatRate.Vat0:
                    Tax = YandexDeliveryTaxType.VAT_0;
                    break;
                case VatRate.Vat10:
                    Tax = YandexDeliveryTaxType.VAT_10;
                    break;
                case VatRate.Vat20:
                    Tax = YandexDeliveryTaxType.VAT_20;
                    break;
                default:
                    Tax = null;
                    break;
            }
        }
    }
}
