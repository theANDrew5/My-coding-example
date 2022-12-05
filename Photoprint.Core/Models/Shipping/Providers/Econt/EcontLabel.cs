using Newtonsoft.Json;
using System;

namespace Photoprint.Core.Models
{

    public class EcontLabel
    {
        [JsonProperty("senderClient")]
        public EcontPerson SenderClient { get; set; }

        [JsonProperty("senderAddress")]
        public EcontAddress SenderAddress { get; set; }

        [JsonProperty("senderOfficeCode")]
        public string SenderOfficeCode { get; set; }

        [JsonProperty("receiverClient")]
        public EcontPerson ReceiverClient { get; set; }

        [JsonProperty("receiverAddress")]
        public EcontAddress ReceiverAddress { get; set; }

        [JsonProperty("packCount")]
        public int PackCount { get; set; }

        [JsonProperty("shipmentType")]
        public string ShipmentType { get; set; }

        [JsonProperty("weight")]
        public double Weight { get; set; }

        [JsonProperty("shipmentDescription")]
        public string ShipmentDescription { get; set; }

        [JsonProperty("receiverOfficeCode")]
        public string ReceiverOfficeCode { get; set; }

        [JsonProperty("emailOnDelivery")]
        public string EmailOnDelivery { get; set; }

        [JsonProperty("shipmentDimensionsL")]
        public double? ShipmentDimensionsL { get; set; }

        [JsonProperty("shipmentDimensionsW")]
        public double? ShipmentDimensionsW { get; set; }

        [JsonProperty("shipmentDimensionsH")]
        public double? ShipmentDimensionsH { get; set; }

        [JsonProperty("orderNumber")]
        public int? OrderNumber { get; set; }

        [JsonProperty("sendData")]
        public string SendData { get; set; }

        [JsonProperty("holidayDeliveryDay")]
        public string HolidayDeliveryDay { get; set; }

        [JsonProperty("services")]
        public ShippingLabelServices[] Services { get; set; }
    }

    public class EcontShippingLabelBuilder
    {
        private EcontLabel _label;

        public EcontShippingLabelBuilder(EcontServiceProviderSettings settings, int orderId, int packCount, double weight, string email)
        {
            _label = new EcontLabel
            {
                OrderNumber = orderId,
                Weight = weight,
                PackCount = packCount,
                SenderClient = settings.SenderClient,
                ShipmentType = "PACK",
                SendData = DateTime.Now.Date.ToString("yyyy-MM-dd"),
                HolidayDeliveryDay = "workday",
                EmailOnDelivery = email,
            };

            if (settings.UseOfficeCode)
            {
                _label.SenderOfficeCode = settings.SenderOfficeCode;
            }
            else
            {
                _label.SenderAddress = settings.SenderAddress;
            }
        }

        public static EcontShippingLabelBuilder Init(EcontServiceProviderSettings settings, int orderId, int packCount, double weight, string email = "") => new EcontShippingLabelBuilder(settings, orderId, packCount, weight, email);
        public EcontShippingLabel Build(string mode) => new EcontShippingLabel { Label = _label, Mode = mode };

        public EcontShippingLabelBuilder SetShipmentDimensions(ParcelSize size)
        {
            _label.ShipmentDimensionsL = size.LengthCm;
            _label.ShipmentDimensionsW = size.WidthCm;
            _label.ShipmentDimensionsH = size.HeightCm;
            return this;
        }

        public EcontShippingLabelBuilder SetShipmentDescription(string description)
        {
            _label.ShipmentDescription = description;
            return this;
        }

        public EcontShippingLabelBuilder SetReceiverClient(string fullName, string phone)
        {
            _label.ReceiverClient = new EcontPerson
            {
                Name = fullName.Replace(" ", string.Empty),
                Phones = new string[] { phone }
            };
            return this;
        }

        public EcontShippingLabelBuilder SetReceiverAddress(OrderAddress receiverAddress)
        {
            _label.ReceiverOfficeCode = receiverAddress.DeliveryProperties?.EcontAddressInfo?.ReceiverOfficeCode;
            if (_label.ReceiverOfficeCode.IsNotEmpty()) return this;

            _label.ReceiverAddress = new EcontAddress
            {
                City = new EcontCity
                {
                    Country = new EcontCountry
                    {
                        Code3 = receiverAddress.Country
                    },
                    Name = receiverAddress.City,
                    RegionName = receiverAddress.Region,
                    PostCode = receiverAddress.PostalCode
                },
                Street = receiverAddress.Street,
                House = receiverAddress.House,
                Location = new EcontLocation
                {
                    Latitude = receiverAddress.Latitude,
                    Longitude = receiverAddress.Longitude
                }
            };

            return this;
        }

        public EcontShippingLabelBuilder SetCashOnDelivery(decimal amount, string currency, string payOptionsTemplate)
        {
            _label.Services = new ShippingLabelServices[]
            {
                new ShippingLabelServices
                {
                    cdAmount = amount.ToString(),
                    cdCurrency = currency,
                    cdPayOptionsTemplate = payOptionsTemplate
                }
            };

            return this;
        }
    }
}
