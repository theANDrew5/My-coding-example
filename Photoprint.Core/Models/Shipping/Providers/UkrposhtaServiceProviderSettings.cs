namespace Photoprint.Core.Models
{
    public class UkrposhtaServiceProviderSettings : IPriceCalculationProviderSettings
    {
        public const double MinWeight = 1;
        public const double MaxWeight = 30000;

        public string Bearer { get; set; }
        public string CounterpartyToken { get; set; }
        public string PostCode { get; set; }

        public bool UpdateShippingAddressesAutomatically => false;
        public bool RegisterOrderInProviderService { get; set;  }
        public bool ChangeOrderStatusToShippedAfterAutomaticRegistration { get; set; }
        public bool SupportAddresesSynchronization => false;
        public bool ShowAddressTab => false;
        public bool MuteNotificationAfterAddressesUpdated => false;
        public bool IsDeliveryPriceCalculationEnabled { get; set; }
        public decimal AdditionalPrice { get; set; }
        public decimal PriceMultiplier { get; set; }
        public decimal DefaultPrice { get; set; }
        public bool GetDefaultPriceFromAddressList { get; set; }
        public int? DefaultShippingPriceId { get; set; }
        public string SenderUuid { get; set; }
    }
}
