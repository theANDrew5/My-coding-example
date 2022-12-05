using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
	public class ShippingProperties : EntityProperties
	{
	    public ShippingProperties() : base() { }
        public ShippingProperties(string xml) : base(xml) { }

        private const string _shippingServiceProviderSettingsKey = "shippingServiceProviderSettings";
        public string ShippingServiceProviderSettingsText
		{
            get => DeserializeItem<string>(_shippingServiceProviderSettingsKey);
            set => SerializeItem(_shippingServiceProviderSettingsKey, value);
        }

		private const string _includeOriginalFilesKey = "includeOriginalFiles";
		public bool? IncludeOriginalFiles
	    {
	        get => DeserializeItem<bool?>(_includeOriginalFilesKey);
	        set => SerializeItem(_includeOriginalFilesKey, value);
	    }

		private const string _parcelPackingKey = "ParcelPacking";
		public IParcelPacking ParcelPacking
	    {
	        get => !string.IsNullOrWhiteSpace(this[_parcelPackingKey])
	            ? JsonConvert.DeserializeObject<IParcelPacking>(this[_parcelPackingKey], new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All})
	            : new ParcelFixedPacking(100, 100, 100);
	        set => this[_parcelPackingKey] = JsonConvert.SerializeObject(value, new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All});
		}

		private const string _photomaxAddressInfoKey = "photomaxAddressKey";
		public PhotomaxAddressInfo PhotomaxAddressInfo
        {
            get => DeserializeItem<PhotomaxAddressInfo>(_photomaxAddressInfoKey);
            set => SerializeItem(_photomaxAddressInfoKey, value);
        }

        private const string _isContractorAddressNotEqualKey = "isContractorAddressNotEqual";
        public bool IsContractorAddressNotEqual
        {
            get => DeserializeItem<bool>(_isContractorAddressNotEqualKey);
            set => SerializeItem(_isContractorAddressNotEqualKey, value);
        }

        private const string _sendSpecialRecieverToDistributedOrderKey = "sendSpecialRecieverToDistributedOrder";
        public bool? SendSpecialRecieverToDistributedOrder
        {
            get => DeserializeItem<bool?>(_sendSpecialRecieverToDistributedOrderKey);
            set => SerializeItem(_sendSpecialRecieverToDistributedOrderKey, value);
        }

        private const string _contractorRecieverPhoneKey = "contractorRecieverPhone";
        public string ContractorRecieverPhone
        {
            get => DeserializeItem<string>(_contractorRecieverPhoneKey);
            set => SerializeItem(_contractorRecieverPhoneKey, value);
        }

        private const string _contractorRecieverNameKey = "contractorRecieverName";
        public string ContractorRecieverName
        {
            get => DeserializeItem<string>(_contractorRecieverNameKey);
            set => SerializeItem(_contractorRecieverNameKey, value);
        }
		
		private const string _emailsKey = "emails";
		public IReadOnlyCollection<string> Emails
		{
			get => DeserializeItem(_emailsKey, Array.Empty<string>());
			set => SerializeItem(_emailsKey, value);
		}

		private const string _useShippingAssistParamsKey = "UseShippingAssistParams";
		public bool UseShippingAssistParams
		{
			get => DeserializeItem<bool>(_useShippingAssistParamsKey);
			set => SerializeItem(_useShippingAssistParamsKey, value);
		}

		private const string _assistShopIdKey = "AssistShopId";
		public string AssistShopId
		{
			get => DeserializeItem<string>(_assistShopIdKey);
			set => SerializeItem(_assistShopIdKey, value);
		}

		private const string _assistLoginKey = "AssistLogin";
		public string AssistLogin
		{
			get => DeserializeItem<string>(_assistLoginKey);
			set => SerializeItem(_assistLoginKey, value);
		}

		private const string _assistPasswordKey = "AssistPassword";
		public string AssistPassword
		{
			get => DeserializeItem<string>(_assistPasswordKey);
			set => SerializeItem(_assistPasswordKey, value);
		}

		private const string _yandexKassaShippingParamsKey = "yandexKassaShippingParams";
		public YandexKassaShippingParams YandexKassaShippingParams
		{
			get => DeserializeItem<YandexKassaShippingParams>(_yandexKassaShippingParamsKey);
			set => SerializeItem(_yandexKassaShippingParamsKey, value);
		}

		private const string _yandexKassaApiShippingParamsKey = "yandexKassaApiShippingParams";
		public YandexKassaApiShippingParams YandexKassaApiShippingParams
		{
			get => DeserializeItem<YandexKassaApiShippingParams>(_yandexKassaApiShippingParamsKey);
			set => SerializeItem(_yandexKassaApiShippingParamsKey, value);
		}

		#region WayForPay
		private const string _useShippingWayForPayParamsKey = "UseShippingWayForPayParams";
		public bool UseShippingWayForPayParams
		{
			get => DeserializeItem<bool>(_useShippingWayForPayParamsKey);
			set => SerializeItem(_useShippingWayForPayParamsKey, value);
		}
		private const string _wayForPayLogin = "WayForPayLogin";
		public string WayForPayLogin
		{
			get => DeserializeItem<string>(_wayForPayLogin);
			set => SerializeItem(_wayForPayLogin, value);
		}
		private const string _wayForPaySecretKey = "WayForPaySecretKey";
		public string WayForPaySecretKey
		{
			get => DeserializeItem<string>(_wayForPaySecretKey);
			set => SerializeItem(_wayForPaySecretKey, value);
		}
		#endregion

		#region LiqPay
		private const string _useShippingLiqPayParamsKey = "UseShippingLiqPayParams";
		public bool UseShippingLiqPayParams
		{
			get => DeserializeItem<bool>(_useShippingLiqPayParamsKey);
			set => SerializeItem(_useShippingLiqPayParamsKey, value);
		}
		private const string _liqPayPublicKey = "LiqPayPublicKey";
		public string LiqPayPublicKey
		{
			get => DeserializeItem<string>(_liqPayPublicKey);
			set => SerializeItem(_liqPayPublicKey, value);
		}
		private const string _liqPayPrivateKey = "LiqPayPrivateKey";
		public string LiqPayPrivateKey
		{
			get => DeserializeItem<string>(_liqPayPrivateKey);
			set => SerializeItem(_liqPayPrivateKey, value);
		}
		#endregion

		public override string SerializeToString()
		{
			return base.SerializeToString();
		}
	}
}