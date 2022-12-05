namespace Photoprint.Core.Models
{
    public class UspsServiceProviderSettings : IPriceCalculationProviderSettings
    {
        public double DefaultWeight { get; set; }
        public UspsAuth AuthSettings { get; set; } = new UspsAuth();
        public UspsSenderAddress SenderAddress { get; set; } = new UspsSenderAddress();

        public bool RegisterOrderInProviderService { get; set; }
        public bool ChangeOrderStatusToShippedAfterAutomaticRegistration { get; set; }

        public bool SupportAddresesSynchronization => false;
        public bool ShowAddressTab => true;
        public bool UpdateShippingAddressesAutomatically => false;
        public bool MuteNotificationAfterAddressesUpdated => false;

        public bool IsDeliveryPriceCalculationEnabled { get; set; }
        public decimal AdditionalPrice { get; set; }
        public decimal PriceMultiplier { get; set; }
        public decimal DefaultPrice { get; set; }
        public bool GetDefaultPriceFromAddressList { get; set; }
        public int? DefaultShippingPriceId { get; set; }
    }

    public class UspsSenderAddress
    {
        public string FromName { get; set; }
        public string FromFirm { get; set; }
        public string FromAddress2 { get; set; }
        public string FromCity { get; set; }
        public string FromState { get; set; }
        public string FromZip5 { get; set; }
    }

}
