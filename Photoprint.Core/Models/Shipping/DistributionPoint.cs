using System;
using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public enum MapType
	{
		None,
		Static,
		GMap,
		Custom
	}

	public class DistributionPoint : Shipping, IEquatable<DistributionPoint>
	{
		public override ShippingType Type => ShippingType.Point;

        public override ShippingAddress Address { get; set; } = new ShippingAddress();
        public virtual ShippingPrices PriceList { get; set; } = new ShippingPrices();

        public override double MaximumWeight => Address.MaxWeight ?? 0.0;
        public override bool IsAvailableWeightConstrain => Address.MaxWeight.HasValue;

        public string ImagePath { get { return string.Format(@"{0}{1}.jpg", Path.Photolab.GetDPointsDirectoryPath(PhotolabId), Id); } }
		public string ImageUrl { get { return string.Format(@"{0}{1}.jpg", Path.Photolab.GetDPointsDirectoryUrl(PhotolabId), Id); } }
		public string ThumbnailImagePath { get { return string.Format(@"{0}{1}-s.jpg", Path.Photolab.GetDPointsDirectoryPath(PhotolabId), Id); } }
		public string ThumbnailImageUrl { get { return string.Format(@"{0}{1}-s.jpg", Path.Photolab.GetDPointsDirectoryUrl(PhotolabId), Id); } }

		public string FullAddress => Address.ToString();
        public string GMapStaticSize => $"{StaticMapWidth}x{StaticMapHeight}";

        private MapType? _mapType;
		public MapType MapType
		{
			get
			{
				if (_mapType == null)
				{
					if (System.IO.File.Exists(ImagePath))
						_mapType = MapType.Static;
					else if (!string.IsNullOrWhiteSpace(Longitude) && !string.IsNullOrWhiteSpace(Latitude))
						_mapType = MapType.GMap;
					else if (!string.IsNullOrWhiteSpace(CustomMapCode))
						_mapType = MapType.Custom;
					else
						_mapType = MapType.None;
				}
				return _mapType.Value;
			}
		}

        private const string _printOnDemandSolutionPointIdKey = "podsPointId";
        public string PrintOnDemandSolutionPointId
        {
            get => Properties.DeserializeItem<string>(_printOnDemandSolutionPointIdKey);
            set => Properties.SerializeItem(_printOnDemandSolutionPointIdKey, value);
        }

        private const string _longitudeKey = "longitude";
        public string Longitude
        {
            get => Properties.DeserializeItem<string>(_longitudeKey);
            set => Properties.SerializeItem(_longitudeKey, value);
        }

        private const string _latitudeKey = "latitude";
        public string Latitude
        {
            get => Properties.DeserializeItem<string>(_latitudeKey);
            set => Properties.SerializeItem(_latitudeKey, value);
        }

        private const string _mapZoomKey = "mapZoom";
        public string MapZoom
        {
            get => Properties.DeserializeItem<string>(_mapZoomKey);
            set => Properties.SerializeItem(_mapZoomKey, value);
        }

        private const string _sitePageUrlKey = "sitePageUrl";
        public string SitePageUrl
        {
            get => Properties.DeserializeItem<string>(_sitePageUrlKey);
            set => Properties.SerializeItem(_sitePageUrlKey, value);
        }

        private const string _staticMapWidthKey = "staticMapWidth";
        public string StaticMapWidth
        {
            get => Properties.DeserializeItem<string>(_staticMapWidthKey);
            set => Properties.SerializeItem(_staticMapWidthKey, value);
        }

        private const string _staticMapHeightKey = "staticMapHeight";
        public string StaticMapHeight
        {
            get => Properties.DeserializeItem<string>(_staticMapHeightKey);
            set => Properties.SerializeItem(_staticMapHeightKey, value);
        }

        private const string _customMapCodeKey = "customMapCode";
        /// <summary>
        /// Не гугл и не картинка, а собственная разметка.
        /// </summary>
        public string CustomMapCode
        {
            get => Properties.DeserializeItem<string>(_customMapCodeKey);
            set => Properties.SerializeItem(_customMapCodeKey, value);
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

        private const string _netprintSettingsKeyKey = "netprintSettingsKey";
        public PhotomaxAddressInfo NetprintSettings
        {
            get => Properties.DeserializeItem<PhotomaxAddressInfo>(_netprintSettingsKeyKey);
            set => Properties.SerializeItem(_netprintSettingsKeyKey, value);
        }

        public DistributionPoint() { }

        public bool Equals(DistributionPoint point)
        {
            return point != null && Id == point.Id;
        }
    }
}
