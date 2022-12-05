
namespace Photoprint.Core.Models
{
    public class EvropochtaServiceProviderSettings : IPriceCalculationProviderSettings
    {
        public EvropochtaServiceProviderSettings() { }

        public string LoginName { get; set; }
        public string Password { get; set; }
        public string ServiceNumber { get; set; }
        public string ApiUrl { get; set; }

        public int DeliveryTypeId { get; set; } = 1;
        public int GoodsTypeId { get; set; }
        public int WeightTypeId { get; set; }
        
        public bool IsValid => !string.IsNullOrEmpty(LoginName) && !string.IsNullOrEmpty(Password) && 
            !string.IsNullOrEmpty(ServiceNumber) && !string.IsNullOrEmpty(ApiUrl) && GoodsTypeId > 0 && WeightTypeId > 0;
        public bool SendStandartDeclareSum { get; set; }
        public bool SendCustomDeclareSum { get; set; }
        public bool UpdateShippingAddressesAutomatically { get; set; }
        public bool RegisterOrderInProviderService { get; set; }
        public bool ChangeOrderStatusToShippedAfterAutomaticRegistration { get; set; }
        public bool SupportAddresesSynchronization => true;
        public bool ShowAddressTab => true;
        public bool MuteNotificationAfterAddressesUpdated { get; set; }
        public bool IsFullAmountToPay { get; set; } = true;
        public int WarehouseIdStart { get; set; }
        public bool IsDeliveryPriceCalculationEnabled { get; set; }
        public decimal AdditionalPrice { get; set; }
        public decimal PriceMultiplier { get; set; }
        public decimal DefaultPrice { get; set; }
        public bool GetDefaultPriceFromAddressList { get; set; }
        public int? DefaultShippingPriceId { get; set; }
    }
}
