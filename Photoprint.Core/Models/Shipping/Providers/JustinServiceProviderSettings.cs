namespace Photoprint.Core.Models
{
    public class JustinServiceProviderSettings : IPriceCalculationProviderSettings
    {
        public const double MinWeight = 1;
        public const double MaxWeight = 30000;

        public string Login { get; set; }
        public string Password { get; set; }
        public string Language { get; set; }
        public string SenderCompany { get; set; }
        public string SenderContact { get; set; }
        public string SenderPhone { get; set; }
        public bool IsTestMode { get; set; }

        public bool UpdateShippingAddressesAutomatically { get; set; }
        public bool RegisterOrderInProviderService { get; set;  }
        public bool ChangeOrderStatusToShippedAfterAutomaticRegistration { get; set; }
        public bool SupportAddresesSynchronization => true;
        public bool ShowAddressTab => true;
        public bool MuteNotificationAfterAddressesUpdated { get; set; }
        public bool IsDeliveryPriceCalculationEnabled { get; set; }
        public decimal AdditionalPrice { get; set; }
        public decimal PriceMultiplier { get; set; }
        public decimal DefaultPrice { get; set; }
        public bool GetDefaultPriceFromAddressList { get; set; }
        public int? DefaultShippingPriceId { get; set; }
    }
}
