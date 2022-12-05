namespace Photoprint.Core.Models
{
    public class PickpointServiceProviderSettings : IPriceCalculationProviderSettings
    {
        public const double MinWeight = 0.1;
        public const double MaxWeight = 29.999;
        public int OrderAssessedValue { get; set; }
        public bool UpdateShippingAddressesAutomatically => false;
        public bool RegisterOrderInProviderService { get; set; }
        public bool ChangeOrderStatusToShippedAfterAutomaticRegistration { get; set; }
        public bool UseOnePlacePerOrder { get; set; }
        public bool SpecifySellerNameInShippingDocument { get; set; }
        public bool MuteNotificationAfterAddressesUpdated => false;
        public bool SupportAddresesSynchronization => false;
        public bool ShowAddressTab => false;
        public string Login { get; set; }
        public string Password { get; set; }
        public string IKN { get; set; }
        public string EDTN { get; set; }

        public bool IsDeliveryPriceCalculationEnabled => true;
        public decimal AdditionalPrice { get; set; }
        public decimal PriceMultiplier { get; set; }
        public decimal DefaultPrice { get; set; }
        public bool GetDefaultPriceFromAddressList { get; set; }
        public int? DefaultShippingPriceId { get; set; }
        public string SenderCity { get; set; }
        public string SenderRegion { get; set; }
        public string ApiUrl { get; set; }

        public double DefaultWeight { get; set; } = 0.1;
        public PickpointServiceProviderSettings() { }
    }
}