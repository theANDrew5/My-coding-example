using System;

namespace Photoprint.Core.Models
{
    public enum OmnivaLanguage : byte
    {
       Russian = 0,
       English = 1,
       Estonian = 2,
       Latvian = 3,
       Lithuanian = 4
    }
    public enum OmnivaCountry : byte {
        All = 0,
        Estonia = 1,
        Latvia = 2,
        Lithuania = 3
    }
    public class OmnivaSenderAddress
    {
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string SenderMobile { get; set; }
        public string SenderEmail { get; set; }
        public string SenderCountry { get; set; }
        public string SenderCity { get; set; }
        public string SenderPostcode { get; set; }
        public string SenderStreet { get; set; }
    }
    public class OmnivaServiceProviderSettings : IPriceCalculationProviderSettings
    {
        public bool IsDeliveryPriceCalculationEnabled { get; set; }
        public decimal AdditionalPrice { get; set; }
        public decimal PriceMultiplier { get; set; }
        public decimal DefaultPrice { get; set; }
        public bool GetDefaultPriceFromAddressList { get; set; }
        public int? DefaultShippingPriceId { get; set; }
        public bool SupportAddresesSynchronization => true;
        public bool ShowAddressTab => true;
        public bool MuteNotificationAfterAddressesUpdated { get; set; }
        public bool UpdateShippingAddressesAutomatically { get; set; }
        public OmnivaLanguage OmnivaLanguage { get; set; }
        public OmnivaCountry OmnivaCountry { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public OmnivaSenderAddress SenderAddress { get; set; }
        public string PartnerCode { get; set; }
        public string SenderEmail { get; set; }
        public bool RegisterOrderInProviderService { get; set; }

        public bool ChangeOrderStatusToShippedAfterAutomaticRegistration { get; set; }
    }
}
