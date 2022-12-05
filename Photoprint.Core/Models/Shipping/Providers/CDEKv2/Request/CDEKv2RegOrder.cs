using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class CDEKv2RegOrder
    {
        [JsonProperty("delivery_recipient_cost")]
        public CDEKv2Payment DeliveryRecipientCost { get; set; }

        [JsonProperty("shipment_point")]
        public string ShipmentPoint { get; set; }

        [JsonProperty("delivery_point")]
        public string DeliveryPoint { get; set; }

        [JsonProperty("from_location")]
        public CDEKv2Location FromLocation { get; set; }

        [JsonProperty("to_location")]
        public CDEKv2Location ToLocation { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }
        
        [JsonProperty("comment")]
        public string Comment { get; set; }
        
        [JsonProperty("packages")]
        public CDEKv2Package[] Packages { get; set; }
        
        [JsonProperty("recipient")]
        public CDEKv2Recipient Recipient { get; set; }
        
        [JsonProperty("sender")]
        public CDEKv2Sender Sender { get; set; }
        
        [JsonProperty("tariff_code")]
        public int TariffCode { get; set; }


        //да, если заказ - международный
        [JsonProperty("date_invoice")]
        public string DateInvoice { get; set; }

        [JsonProperty("shipper_name")]
        public string ShipperName { get; set; }

        [JsonProperty("shipper_address")]
        public string ShipperAddress { get; set; }

        [JsonProperty("seller")]
        public CDEKv2Seller Seller { get; set; }

        [JsonProperty("weight_gross")]
        public int? WeightGross { get; set; }
    }

    public class CDEKv2Seller
    {
        [JsonProperty("address")]
        public string Address { get; set; } //Адрес истинного продавца. Используется при печати инвойсов для отображения адреса настоящего продавца товара, либо торгового названия.
    }
}
