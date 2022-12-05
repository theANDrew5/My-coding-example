using System;
using Photoprint.Core.Configuration;

namespace Photoprint.Core.Models
{
	public class CdekServiceProviderSettings : IPriceCalculationProviderSettings
    {
        /// <summary>
        /// минимальный вес в кг
        /// </summary>
        public const double MinWeight = 0.100;
        /// <summary>
        /// максимальный вес в кг
        /// </summary>
        public const double MaxWeight = 29.999;

        public const string CdekDateTimeFormatString = "yyyy-MM-ddTHH:mm:ss";
		public const string CdekDateFormatString = "yyyy-MM-dd";
		public const string CdekTimeFormatString = "HH:mm:ss";

        public bool UseAssessedValue { get; set; }
        public int OrderAssessedValue { get; set; }
        public bool UpdateShippingAddressesAutomatically { get; set; }
		public bool RegisterOrderInProviderService { get; set; }
		public bool ChangeOrderStatusToShippedAfterAutomaticRegistration { get; set; }
        public bool SpecifySellerNameInShippingDocument { get; set; }
        public string SellerName { get; set; }
        public string SellerAddress { get; set; }
        public string SellerPhone { get; set; }
        public bool IsTestMode { get; set; }

        public bool SupportAddresesSynchronization => true;
        public bool ShowAddressTab => true;
        public bool MuteNotificationAfterAddressesUpdated { get; set; }
        public bool UseCdekPvzWidget { get; set; }

        public string Account { get; set; }
		public string SecurePassword { get; set; }

		public string TariffTypeCode { get; set; }
        public bool TariffCodesMoreThanOne { get; set; }
		public string SendCityCode { get; set; }
        public string SendCityPostCode { get; set; }

        public bool IsDeliveryPriceCalculationEnabled { get; set; }
        public decimal AdditionalPrice { get; set; }
        public decimal PriceMultiplier { get; set; }
        public decimal DefaultPrice { get; set; }
        public bool GetDefaultPriceFromAddressList { get; set; }
        public int? DefaultShippingPriceId { get; set; }

        public double DefaultWeight { get; set; } = 0.1;

        public bool UseProxy => Settings.UseProxyForCdek;
        public CdekServiceProviderSettings() { }

        public string GetSecure(DateTime date)
		{
			//0000-00-00T00:00:00, secure = md5(date.'&'.secure_password)
			var dateString = date.ToString(CdekDateTimeFormatString);
			return GetSecure(dateString);
		}

	    public string GetSecure(string dateTime)
	    {
            return Utility.Hash.ToMD5HexString($"{dateTime}&{SecurePassword}").ToLowerInvariant();
        }
    }
}