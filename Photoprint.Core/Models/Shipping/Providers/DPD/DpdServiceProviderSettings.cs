namespace Photoprint.Core.Models
{

    public class DpdServiceProviderSettings : IPriceCalculationProviderSettings
    {
        public double DefaultWeight {  get; set; }

        public DpdServiceCode ServiceCode {  get; set; }
        public DpdServiceVariant ServiceVariant {  get; set; }
        public string CompanyName {  get; set; }
        public string PickupTimePeriod {  get; set; }
        public string PaymentOptions { get; set; }
        public Address SenderAddress { get; set; } 
        public string EmployeeFio { get; set; }
        public string EmployeePhone { get; set; }
        public DpdAuth AuthSettings { get; set; } = new DpdAuth();

        public (DpdAddressFilter filter, string query) AddressFilter { get; set; }

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
    }

    public class DpdAuth
    {
        public string ClientKey { get; set; }
        public int ClientNumber { get; set; }
        public bool IsTestMode { get; set; } 
    }

    public class DpdSenderAddress
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string HouseKorpus { get; set; }
    }
}
