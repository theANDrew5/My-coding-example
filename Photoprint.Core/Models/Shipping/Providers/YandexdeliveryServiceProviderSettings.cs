namespace Photoprint.Core.Models
{
    public class YandexDeliveryServiceProviderSettings : IPriceCalculationProviderSettings
    {
        public bool UpdateShippingAddressesAutomatically => false;

        public bool RegisterOrderInProviderService { get; set; }
        public bool ChangeOrderStatusToShippedAfterAutomaticRegistration { get; set; }

        public bool SupportAddresesSynchronization => false;
        public bool ShowAddressTab => false;
        public bool MuteNotificationAfterAddressesUpdated => false;

        public string OAuthId { get; set; }
        public string Token { get; set; }

        public int SenderId { get; set; }
        public string WarehouseFromId { get; set; }
        public string WarehouseToId { get; set; }
        public string ShipmentType{ get; set; }
        public double DefaultShippingWeight { get; set; }

        public bool OverWeightPostalTariffEnabled => false;

        public bool IsDeliveryPriceCalculationEnabled => true;

        public decimal AdditionalPrice { get; set; }

        public decimal PriceMultiplier { get; set; } = 1;
        public decimal DefaultPrice { get; set; }

        public bool GetDefaultPriceFromAddressList => false;

        public int? DefaultShippingPriceId { get; set; }
    }
}