using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryOptionsRequest
    {
        [JsonProperty("senderId")]
        public int SenderId { get; set; }
        
        [JsonProperty("from")]
        public YandexDeliveryFrom From { get; set; }
        
        [JsonProperty("to")]
        public YandexDeliveryTo To { get; set; }
        
        [JsonProperty("dimensions")]
        public YandexDeliveryDimensions Dimensions { get; set; }
        
        [JsonProperty("deliveryType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public YandexDeliveryShippingType DeliveryType { get; set; }
        
        [JsonProperty("shipment")]
        public YandexDeliveryOptionShipment Shipment { get; set; }
        
        [JsonProperty("cost")]
        public YandexDeliveryCost Cost { get; set; }
        
        [JsonProperty("tariffId")]
        public int TariffId { get; set; }
        
        [JsonProperty("desiredDeliveryDate")]
        public string DesiredDeliveryDate { get; set; }
        
        [JsonProperty("settings")]
        public (bool useHandlingTime, bool useWarehouseSchedule, bool showDisabledOptions) Settings { get; set; }

        //public YandexDeliveryOptionsRequest(int senderId, YandexDeliveryTo to)
        //{
        //    SenderId = senderId;
        //    To = to;
        //}
    }

    public class YandexDeliveryOptionShipment
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("partnerId")]
        public string PartnerId { get; set; }

        [JsonProperty("warehouseId")]
        public long WarehouseId { get; set; }

        [JsonProperty("includeNonDefault")]
        public string IncludeNonDefault { get; set; }

    }


    public class YandexDeliveryFrom
    {
        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("geoId")]
        public int GeoId { get; set; }
    }
}
