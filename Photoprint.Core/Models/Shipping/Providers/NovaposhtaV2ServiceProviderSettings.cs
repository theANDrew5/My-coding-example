namespace Photoprint.Core.Models
{
    public class NovaposhtaV2ServiceProviderSettings : IPriceCalculationProviderSettings
    {
        public string ApiKey { get; set; }

        public string SenderRef { get; set; }
        public string SenderContactRef { get; set; }
        public string SenderAddressRef { get; set; }
        public string SenderCompany { get; set; }
        public string SenderPhone { get; set; }
        public string ContentDescription { get; set; }
        public string SenderCityRef { get; set; }

        public bool UseAssessedValue { get; set; }
        public int OrderAssessedValue { get; set; }

        public bool UpdateShippingAddressesAutomatically { get; set; }
        public bool RegisterOrderInProviderService { get; set; }
        public bool ChangeOrderStatusToShippedAfterAutomaticRegistration { get; set; }
        public bool SupportAddresesSynchronization => true;
        public bool ShowAddressTab => true;
        public bool MuteNotificationAfterAddressesUpdated { get; set; }

        public string WarehouseCityName { get; set; }
        public bool IsDeliveryPriceCalculationEnabled { get; set; }
        public decimal AdditionalPrice { get; set; }
        public decimal PriceMultiplier { get; set; }
        public decimal DefaultPrice { get; set; }
        public bool GetDefaultPriceFromAddressList { get; set; }
        public int? DefaultShippingPriceId { get; set; }
    }
}