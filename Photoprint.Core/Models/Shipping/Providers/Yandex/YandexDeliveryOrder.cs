using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryOrder
    {
        [JsonProperty("senderId")]
        public int SenderId { get; set; }

        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("recipient")]
        public YandexDeliveryRecipient Recipient { get; set; }

        [JsonProperty("cost")]
        public YandexDeliveryCost Cost { get; set; }

        [JsonProperty("contacts")]
        public YandexDeliveryContact[] Contacts { get; set; }

        [JsonProperty("deliveryType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public YandexDeliveryShippingType DeliveryType { get; set; }

        [JsonProperty("deliveryOption")]
        public YandexDeliveryDeliveryOption DeliveryOption { get; set; }

        [JsonProperty("shipment")]
        public YandexDeliveryShipment Shipment { get; set; }

        [JsonProperty("dimensions")]
        public YandexDeliveryDimensions Dimensions { get; set; }

        [JsonProperty("items")]
        public List<YandexDeliveryItem> Items { get; set; }

        public YandexDeliveryOrder(Photolab photolab, YandexDeliveryServiceProviderSettings settings, Order order, IReadOnlyList<OrderDetail> details)
        {
            var size = order.Shipping.Properties.ParcelPacking?.GetParcelSize(details);
            var weight = details.Sum(item => item.TotalWeight);
            if (weight == 0) weight = settings.DefaultShippingWeight;

            SenderId = settings.SenderId;
            DeliveryType = order.DeliveryAddress.DeliveryProperties.YandexDeliveryAddressInfo.DeliveryType ?? (YandexDeliveryShippingType)255;
            ExternalId = order.Id.ToString();
            DeliveryOption = new YandexDeliveryDeliveryOption
            {
                TariffId = order.DeliveryAddress.DeliveryProperties.YandexDeliveryAddressInfo.TariffId,
                PartnerId = order.DeliveryAddress.DeliveryProperties.YandexDeliveryAddressInfo.PartnerId,
                Delivery = order.DeliveryPrice
            };
            Recipient = new YandexDeliveryRecipient
            {
                FirstName = order.DeliveryAddress.FirstName,
                LastName = order.DeliveryAddress.LastName,
                MiddleName = order.DeliveryAddress.MiddleName,
                PickupPointId = order.DeliveryAddress.DeliveryProperties.YandexDeliveryAddressInfo.PickPointId?? string.Empty,
                Address = new YandexDeliveryAddress
                {
                    GeoId = order.DeliveryAddress.DeliveryProperties.YandexDeliveryAddressInfo.GeoId?? string.Empty,
                    Country = order.DeliveryAddress.Country,
                    Region = order.DeliveryAddress.Region,
                    Locality = order.DeliveryAddress.City,
                    Street = order.DeliveryAddress.Street,
                    House = order.DeliveryAddress.House,
                    Apartment = order.DeliveryAddress.Flat,
                },

            };
            Cost = new YandexDeliveryCost
            {
                AssessedValue = order.TotalPrice+order.DiscountPrice,
                ManualDeliveryForCustomer = order.DeliveryPrice,
                FullyPrepaid = order.PaymentStatus == OrderPaymentStatus.Paid && !order.IsShippingPricePaidSeparately
            };
            Contacts = new[]
            {
                new YandexDeliveryContact()
                {
                     Type = YandexDeliveryContactType.RECIPIENT,
                     FirstName = order.DeliveryAddress.FirstName,
                     LastName = order.DeliveryAddress.LastName,
                     MiddleName = order.DeliveryAddress.MiddleName,
                     Phone = order.DeliveryAddress.Phone
                }
            };
            Dimensions = new YandexDeliveryDimensions
            {
                Weight = weight,
                Height = Convert.ToInt32(size.HeightCm),
                Length = Convert.ToInt32(size.LengthCm),
                Width = Convert.ToInt32(size.WidthCm)
            };
            Shipment = new YandexDeliveryShipment
            {
                PartnerTo = order.DeliveryAddress.DeliveryProperties.YandexDeliveryAddressInfo.PartnerId,
                WarehouseFrom = settings.WarehouseFromId,
                WarehouseTo = settings.WarehouseToId,
                Type = settings.ShipmentType
            };
            Items = details.Select(detail =>
                new YandexDeliveryItem(detail, order.PaymentStatus == OrderPaymentStatus.Paid , photolab.Properties.VatRate)).ToList();
        }
    }
}
