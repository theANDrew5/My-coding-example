namespace Photoprint.Core.Models
{
    public class PostnlServiceProviderSettings : IPriceCalculationProviderSettings
    {
        public string ApiKey { get; set; }
        public bool IsTestMode { get; set; }

        public bool UpdateShippingAddressesAutomatically { get; set; }
        public bool RegisterOrderInProviderService { get; set; }
        public bool ChangeOrderStatusToShippedAfterAutomaticRegistration { get; set; }
        public bool SupportAddresesSynchronization => false;
        public bool ShowAddressTab => false;
        public bool MuteNotificationAfterAddressesUpdated { get; set; }
        public bool IsDeliveryPriceCalculationEnabled { get; set; }
        public decimal AdditionalPrice { get; set; }
        public decimal PriceMultiplier { get; set; }
        public decimal DefaultPrice { get; set; }
        public bool GetDefaultPriceFromAddressList { get; set; }
        public int? DefaultShippingPriceId { get; set; }
    }
}
