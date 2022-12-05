namespace Photoprint.Core.Models
{
    public class DDeliveryV2ServiceProviderSettings : IShippingServiceProviderSettings, IPriceCalculationProviderSettings
    {
        public const double MinWeight = 0.01; // кг
        
        public bool UpdateShippingAddressesAutomatically => false;
        public bool RegisterOrderInProviderService { get; set; }
        public bool ChangeOrderStatusToShippedAfterAutomaticRegistration { get; set; }

        public bool SupportAddresesSynchronization => true;
        public bool ShowAddressTab => DontUseWidget;
        public bool MuteNotificationAfterAddressesUpdated { get; set; }
        
        public string ApiKey { get; set; }
        public bool IsTestMode { get; set; }
        public double DefaultWeight { get; set; }
        public string WidgetKey { get; set; }
        public string ShopId { get; set; }
        public bool DisableTrackingWidget { get; set; }
        public bool RegisterAsPaid { get; set; }
        public bool DontUseWidget { get; set; }

        public bool IsDeliveryPriceCalculationEnabled { get; set; }
        public decimal AdditionalPrice { get; set; }
        public decimal PriceMultiplier { get; set; }
        public decimal DefaultPrice { get; set; }
        public bool GetDefaultPriceFromAddressList { get; set; }
        public int? DefaultShippingPriceId { get; set; }
    }
}
