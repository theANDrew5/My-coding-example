using System.Collections.Generic;
using Photoprint.Core.Configuration;

namespace Photoprint.Core.Models
{
    public class CDEKv2ServiceProviderSettings : IPriceCalculationProviderSettings
    {
        public CDEKv2Auth AuthSettings { get; set; } = new CDEKv2Auth();
        public IReadOnlyCollection<CDEKv2Tariff> TariffList { get; set; } = new List<CDEKv2Tariff>();

        public CDEKv2Location FromLocation => WarehouseAddress != null
            ? new CDEKv2Location
            {
                Latitude = WarehouseAddress.Latitude,
                Longitude = WarehouseAddress.Longitude,
                PostalCode = WarehouseAddress.PostalCode,
                Country = WarehouseAddress.Country,
                City = WarehouseAddress.City,
                Region = WarehouseAddress.Region,
                Address = WarehouseAddress.ToAddressString(),
                AddressFull = WarehouseAddress.ToString(true)
            }
            : null;

        public Address WarehouseAddress { get; set; }
        public CDEKv2Sender Sender { get; set; }
        public decimal AmountOfAdditionalFee { get; set; }
        public int DefaultWeightGram { get; set; }

        public string SellerAddress { get; set; }
        public string PvzCode { get; set; }

        public string ShipperName { get; set; }
        public string ShipperAddress { get; set; }
        public int WeightGross { get; set; }

        public string[] ContryCodes { get; set; }
        public bool DeliveryToOtherCountries { get; set; }

        public bool IsDeliveryPriceCalculationEnabled { get; set; } = true;
        public decimal AdditionalPrice { get; set; }
        public decimal PriceMultiplier { get; set; }
        public decimal DefaultPrice { get; set; }
        public bool GetDefaultPriceFromAddressList { get; set; }
        public int? DefaultShippingPriceId { get; set; }

        public bool UpdateShippingAddressesAutomatically { get; set; }
        public bool RegisterOrderInProviderService { get; set; }
        public bool ChangeOrderStatusToShippedAfterAutomaticRegistration { get; set; }

        public bool SupportAddresesSynchronization => true;
        public bool ShowAddressTab => true;
        public bool MuteNotificationAfterAddressesUpdated { get; set; }
    }
}
