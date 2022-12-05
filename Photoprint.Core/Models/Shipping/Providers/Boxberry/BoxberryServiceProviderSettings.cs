namespace Photoprint.Core.Models
{
    public class BoxberryServiceProviderSettings : IPriceCalculationProviderSettings
    {
        public BoxberryServiceProviderSettings() { }

        /// <summary>
        /// максимальный вес в граммах
        /// </summary>
        public const double MaxWeight = 30999;

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

        public string APIToken { get; set; }
        public bool IsTestMode { get; set; }
        public bool SendUserCompanyInfo { get; set; }
        public bool DeclareValue { get; set; }

        public string[] ContryCodes { get; set; }
        public bool DeliveryToOtherCountries { get; set; }
        /// <summary>
        /// Код точки куда продавец привезет заказ для отправки
        /// </summary>
        public string SendPlaceCode { get; set; }
        public double DefaultWeight { get; set; }
    }
    }
