namespace Photoprint.Core.Models
{
    public class EcontServiceProviderSettings : IPriceCalculationProviderSettings
    {
        public EcontAuth AuthSettings { get; set; } = new EcontAuth();

        public EcontPerson SenderClient { get; set; }
        public EcontAddress SenderAddress { get; set; }
        public string SenderOfficeCode { get; set; }
        public bool UseOfficeCode { get; set; }
        public string PayOptionsTemplate { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsCashOnDeliveryAvailable { get; set; }

        public string CountryCode { get; set; }
        public double DefaultWeight { get; set; } = 0.1;
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
