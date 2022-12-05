namespace Photoprint.Core.Models
{
    public class ExgarantServiceProviderSettings : IPriceCalculationProviderSettings
    {
        private const string _testApiUrl = "http://test.ex-garant.ru/hydra/api_xml.php";
        private const string _liveApiUrl = "http://ex-garant.ru/hydra/api_xml.php";

        public bool UpdateShippingAddressesAutomatically { get; set; }
        public bool RegisterOrderInProviderService { get; set; }
        public bool ChangeOrderStatusToShippedAfterAutomaticRegistration { get; set; }
        public bool SupportAddresesSynchronization => true;
        public bool ShowAddressTab => true;
        public bool MuteNotificationAfterAddressesUpdated { get; set; }
        public bool IsTestMode { get; set; }
        public string UKey { get; set; }
        public string Uid { get; set; }
        public bool OnlyPrepaidPoints { get; set; }
        public ExgarantDeliveryType DeliveryType { get; set; }
        public string ApiUri => IsTestMode ? _testApiUrl : _liveApiUrl;
        

        public bool IsDeliveryPriceCalculationEnabled { get; set; }
        public decimal AdditionalPrice { get; set; }
        public decimal PriceMultiplier { get; set; }
        public decimal DefaultPrice { get; set; }
        public bool GetDefaultPriceFromAddressList { get; set; }
        public int? DefaultShippingPriceId { get; set; }
    }

    public enum ExgarantDeliveryType
    {
        Courier,
        Post,
        Boxberry,
        PickupPoint,
        BoxberryCourier,
        AnyCourier,
        Unknown
    }
}