namespace Photoprint.Core.Models
{
    public class PhotomaxServiceProviderSettings : IShippingServiceProviderSettings, IPriceCalculationProviderSettings
    {
        public bool UpdateShippingAddressesAutomatically { get; set; }
        public bool RegisterOrderInProviderService => false;
        public bool ChangeOrderStatusToShippedAfterAutomaticRegistration => false;
        public bool SupportAddresesSynchronization => true;
        public bool ShowAddressTab => true;
        public bool MuteNotificationAfterAddressesUpdated { get; set; }
        public string ShopGuid { get; set; }
        public bool IsTestMode { get; set; }
        public string ApiUri => IsTestMode ? "http://ws01.proto.photoholding.com/ru" : "http://ws01.photoholding.com/ru";

        public bool AlowRefund { get; set; }

        public const double MinWeight = 0.1;
        public const double MaxWeight = 29.999;
        public bool IsDeliveryPriceCalculationEnabled { get; set; }
        public decimal AdditionalPrice { get; set; }
        public decimal PriceMultiplier { get; set; } = 1;
       
        public decimal DefaultPrice { get; set; }
        public bool GetDefaultPriceFromAddressList { get; set; }

        public int? DefaultShippingPriceId { get; set; }
    }

    public enum PhotomaxDeliveryType
    {
        Post,
        Courier,
        PickupPoint
    }
}