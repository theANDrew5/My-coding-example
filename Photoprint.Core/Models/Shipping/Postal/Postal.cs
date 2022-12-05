using System;
using System.Collections.Generic;

namespace Photoprint.Core.Models
{
	public class Postal : Shipping, IEquatable<Postal>, ITrackable
    {
		public override ShippingType Type => ShippingType.Postal;

        public virtual IReadOnlyCollection<ShippingAddressPrice> ShippingAddressPrices { get; }
        public virtual IReadOnlyCollection<ShippingAddress> ShippingAddresses { get; } = new List<ShippingAddress>();

        public bool IsTrackingSet => !string.IsNullOrWhiteSpace(TrackingUrlFormatString);
        public bool IsGetRequest => IsTrackingSet && TrackingUrlFormatString.Contains("{0}");

        private const string _postalTypeKey = "PostalType";
        public PostalType PostalType
        {
            get => Properties.DeserializeItem(_postalTypeKey, PostalType.ToClientDelivery);
            set => Properties.SerializeItem(_postalTypeKey, value, PostalType.ToClientDelivery);
        }

        private const string _trackingUrlFormatStringKey = "trackingUrlFormatString";
        public string TrackingUrlFormatString
		{
			get
			{
                switch (ShippingServiceProviderType)
				{
                    case ShippingServiceProviderType.Boxberry:
                        return "https://boxberry.ru/tracking/?id={0}";
				    case ShippingServiceProviderType.Cdek:
                    case ShippingServiceProviderType.CDEKv2:
				        return "https://www.cdek.ru/track.html?order_id={0}";
				    //  case ShippingServiceProviderType.DDelivery:
                    //  case ShippingServiceProviderType.DDeliveryV2:
				    //      return "https://saferoute.ru/#order-info/{0}";
                    case ShippingServiceProviderType.Postnl:
                        return "https://postnl.post/details";
                    case ShippingServiceProviderType.Justin:
                        return "https://justin.ua/tracking-ttn/?ttn_number={0}";
                    case ShippingServiceProviderType.Omniva:
                        return "https://www.omniva.lv/private/track_and_trace";
                    case ShippingServiceProviderType.Pickpoint:
                        return "https://pickpoint.ru/monitoring/";
                    case ShippingServiceProviderType.RussianPost:
                        return "https://www.pochta.ru/tracking#{0}";
                    default:
                        return Properties.DeserializeItem<string>(_trackingUrlFormatStringKey);
                }
			}
			set => Properties.SerializeItem(_trackingUrlFormatStringKey, value);
        }

        private const string _trackingServiceUrlKey = "trackingServiceUrl";
        public string TrackingServiceUrl
		{
			get
			{
                switch (ShippingServiceProviderType)
				{
                    case ShippingServiceProviderType.Boxberry:
                        return "https://boxberry.ru/";
                    case ShippingServiceProviderType.Cdek:
                    case ShippingServiceProviderType.CDEKv2:
                        return "http://www.cdek.ru";
				    case ShippingServiceProviderType.DDelivery:
				    case ShippingServiceProviderType.DDeliveryV2:
                        return "https://saferoute.ru/";
                    case ShippingServiceProviderType.Postnl:
                        return "https://postnl.nl";
                    case ShippingServiceProviderType.Justin:
                        return "https://justin.ua/";
                    case ShippingServiceProviderType.RussianPost:
                        return "https://www.pochta.ru/";
                    default:
                        return Properties.DeserializeItem<string>(_trackingServiceUrlKey);
                }
			}
			set => Properties.SerializeItem(_trackingServiceUrlKey, value);
		}

        private const string _trackingFieldNameKey = "TrackingFieldName";
        public string TrackingFieldName
		{
            get
            {
                switch (ShippingServiceProviderType)
                {
                    case ShippingServiceProviderType.Postnl:
                        return "barcodes";
                    default:
                        return Properties.DeserializeItem<string>(_trackingFieldNameKey);
                }
            }
            set => Properties.SerializeItem(_trackingFieldNameKey, value);
		}

        private const string _isIndexRequiredKey = "IsIndexRequired";
        public bool IsIndexRequired
        {
            get => ShippingServiceProviderType == ShippingServiceProviderType.RussianPost || Properties.DeserializeItem<bool>(_isIndexRequiredKey);
            set => Properties.SerializeItem(_isIndexRequiredKey, value);
        }

        private const string _isRegionRequiredKey = "IsRegionRequired";
        public bool IsRegionRequired
        {
            get => Properties.DeserializeItem<bool>(_isRegionRequiredKey);
            set => Properties.SerializeItem(_isRegionRequiredKey, value);
		}

        private const string _isMultipleAddressLinesKey = "isMultipleAddressLines";
        public bool IsMultipleAddressLines
		{
            get => Properties.DeserializeItem<bool>(_isMultipleAddressLinesKey);
            set => Properties.SerializeItem(_isMultipleAddressLinesKey, value);
        }

        private const string _isUseContractNumberKey = "isUseContractNumber";
        public bool IsUseConractNumber
        {
            get => Properties.DeserializeItem<bool>(_isUseContractNumberKey);
            set => Properties.SerializeItem(_isUseContractNumberKey, value);
        }

        private const string _isUseAdditionalContractNumberKey = "isUseAdditionalContractNumber";
        public bool IsUseAdditionalContractNumber
        {
            get => Properties.DeserializeItem<bool>(_isUseAdditionalContractNumberKey);
            set => Properties.SerializeItem(_isUseAdditionalContractNumberKey, value);
        }

        private const string _postalSalesTaxesKey = "PostalSalesTaxes";
        // используется BinaryFormatter
        public PostalSalesTaxes PostalSalesTaxes
		{
			get => !string.IsNullOrWhiteSpace(Properties[_postalSalesTaxesKey]) ? Utility.DeserializeFromString<PostalSalesTaxes>(Properties[_postalSalesTaxesKey]) : new PostalSalesTaxes();
		    set => Properties[_postalSalesTaxesKey] = Utility.SerializeToString(value);
		}

        private const string _autoCourierCallKey = "AutoCourierCall";
        public bool AutoCourierCall
        {
            get => Properties.DeserializeItem(_autoCourierCallKey, false);
            set => Properties.SerializeItem(_autoCourierCallKey, value);
        }

        private const string _callForCertainTariffTypesKey = "CallForCertainTariffTypes";
        public bool AutoCourierCallForCertainTariffTypes
        {
            get => Properties.DeserializeItem(_callForCertainTariffTypesKey, false);
            set => Properties.SerializeItem(_callForCertainTariffTypesKey, value);
        }

        private const string _courierCallDPKey = "CourierCallDP";
        public int CourierCallDistributionPoint
        {
            get => int.TryParse(Properties[_courierCallDPKey], out var ccd) ? ccd : 0;
            set => Properties[_courierCallDPKey] = value.ToString();
        }

        private const string _sendToPostalByEqualTrackKey = "SendToPostalByEqualTrack";
        public bool SendToPostalByEqualTrack
        {
            get => Properties.DeserializeItem(_sendToPostalByEqualTrackKey, false);
            set => Properties.SerializeItem(_sendToPostalByEqualTrackKey, value);
        }

        public bool IsTrackingChangeAllowed => true;
        #region оставлю на память
        //{
        //	get
        //	{
        //		if (ShippingServiceProviderType == ShippingServiceProviderType.Cdek)
        //		{
        //			return true;
        //		}
        //		if (ShippingServiceProviderType == ShippingServiceProviderType.ImLogistics)
        //		{
        //			return true;
        //		}
        //		return true;
        //	}
        //}
        #endregion

        public bool Equals(Postal postal)
		{
			return postal != null && Id == postal.Id;
		}
    }
}
