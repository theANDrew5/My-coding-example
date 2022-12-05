using System.Collections.Generic;
using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryOptionsResponse
    {
        [JsonProperty("tariffId")]
        public string TariffId { get; set; }

        [JsonProperty("tariffName")]
        public string TariffName { get; set; }

        [JsonProperty("cost")]
        public YandexDeliveryOptionsCost Cost { get; set; }

        [JsonProperty("delivery")]
        public YandexDeliveryDelivery Delivery { get; set; }

        [JsonProperty("pickupPointIds")]
        public List<string> PickupPointIds { get; set; }

        [JsonProperty("shipments")]
        public YandexDeliveryShipment[] Shipments { get; set; }

        [JsonProperty("services")]
        public YandexDeliveryService[] Services { get; set; }

        [JsonProperty("disabledReasons")]
        public YandexDeliveryDisabledReasons[] DisabledReasons { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }
    }

    public class YandexDeliveryOptionsCost
    {
        [JsonProperty("total")]
        public decimal? Total { get; set; }

        [JsonProperty("assessedValue")]
        public decimal? AssessedValue { get; set; }

        [JsonProperty("delivery")]
        public decimal? Delivery { get; set; }

        [JsonProperty("deliveryForSender")]
        public decimal DeliveryForSender { get; set; }

        [JsonProperty("deliveryForCustomer")]
        public decimal DeliveryForCustomer { get; set; }
    }

    public class YandexDeliveryDisabledReasons
    {
        [JsonProperty("code")]
        public string[] Code { get; set; }

        [JsonProperty("description")]
        public string[] Description { get; set; }
    }
}
