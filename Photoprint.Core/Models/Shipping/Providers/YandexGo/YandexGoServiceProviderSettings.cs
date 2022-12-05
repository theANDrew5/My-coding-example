namespace Photoprint.Core.Models
{
    public class YandexGoServiceProviderSettings : IPriceCalculationProviderSettings
    {
        public string AccessToken { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string CostCurrency { get; set; }
        public string ApiMapKey { get; set; }
        public Address WarehouseAddress { get; set; }
        public YandexGoContact Sender { get; set; }

        public YandexGoSize DefaultSize { get; set; }
        public double DefaultWeight { get; set; }

        public bool UpdateShippingAddressesAutomatically => false;
        public bool RegisterOrderInProviderService { get; set; }
        public bool ChangeOrderStatusToShippedAfterAutomaticRegistration { get; set; }

        public bool SupportAddresesSynchronization => false;
        public bool ShowAddressTab => true;
        public bool MuteNotificationAfterAddressesUpdated => false;

        public bool IsDeliveryPriceCalculationEnabled { get; set; }
        public decimal AdditionalPrice { get; set; }
        public decimal PriceMultiplier { get; set; }
        public decimal DefaultPrice { get; set; }
        public bool GetDefaultPriceFromAddressList { get; set; }
        public int? DefaultShippingPriceId { get; set; }
    }
}

