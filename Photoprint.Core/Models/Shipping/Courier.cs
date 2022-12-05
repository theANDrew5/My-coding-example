using System;
using System.Collections.Generic;

namespace Photoprint.Core.Models
{
	public class Courier : Shipping, IEquatable<Courier>, ITrackable
    {
		public override ShippingType Type => ShippingType.Courier;

        public bool IsTrackingSet => !string.IsNullOrWhiteSpace(TrackingServiceUrl);
		public bool IsGetRequest => IsTrackingSet && TrackingUrlFormatString != null && TrackingUrlFormatString.Contains("{0}");
		public bool IsTrackingChangeAllowed => true;

        public virtual IReadOnlyCollection<ShippingAddress> ShippingAddresses { get; } = null;
        public virtual IReadOnlyCollection<ShippingAddressPrice> ShippingAddressPrices { get; }


        private const string _trackingUrlFormatStringKey = "trackingUrlFormatString";
		public string TrackingUrlFormatString
		{
			get => Properties.DeserializeItem<string>(_trackingUrlFormatStringKey);
			set => Properties.SerializeItem(_trackingUrlFormatStringKey, value);
        }

		private const string _trackingServiceUrlKey = "trackingServiceUrl";
		public string TrackingServiceUrl
		{
			get => Properties.DeserializeItem<string>(_trackingServiceUrlKey);
			set => Properties.SerializeItem(_trackingServiceUrlKey, value);
		}

		private const string _trackingFieldNameKey = "TrackingFieldName";
		public string TrackingFieldName
		{
			get => Properties.DeserializeItem<string>(_trackingFieldNameKey);
			set => Properties.SerializeItem(_trackingFieldNameKey, value);
		}

		private const string _isIndexRequiredKey = "IsIndexRequired";
		public bool IsIndexRequired
		{
			get => Properties.DeserializeItem<bool>(_isIndexRequiredKey);
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

		private const string _postalSalesTaxesKey = "PostalSalesTaxes";
		public PostalSalesTaxes PostalSalesTaxes
		{
			get => Properties.DeserializeItem(_postalSalesTaxesKey, new PostalSalesTaxes());
			set => Properties.SerializeItem(_postalSalesTaxesKey, value);
		}

		private const string _callSeparateCourierKey = "callSeparateCourier";
		public bool? CallSeparateCourier
		{
			get => Properties.DeserializeItem<bool?>(_callSeparateCourierKey);
			set => Properties.SerializeItem(_callSeparateCourierKey, value);
        }

		private const string _sendOrderOwnerToDistributedOrderKey = "sendOrderOwnerToDistributedOrder";
		public bool? SendOrderOwnerToDistributedOrder
        {
			get => Properties.DeserializeItem<bool?>(_sendOrderOwnerToDistributedOrderKey);
			set => Properties.SerializeItem(_sendOrderOwnerToDistributedOrderKey, value);
		}

		public Courier() { }

		public bool Equals(Courier courier)
		{
			return courier != null && Id == courier.Id;
		}
	}
}
