namespace Photoprint.Core.Models
{
    public class ImlV2ServiceProviderSettings : IPriceCalculationProviderSettings
    {
        public bool UpdateShippingAddressesAutomatically { get; set; }
        public bool RegisterOrderInProviderService { get; set; }
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

        public bool IsTestMode { get; set; }
        public string Login { get; set; }

        public string ServiceCode { get; set; }
        public string ServiceCodeWithPayment { get; set; }

        public string Password { get; set; }
        public string Departure { get; set; }
    }
}