using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Photoprint.Core.Models
{
    public sealed class CityAddressCountryClass
    {
        public CityAddressCountry County { get; set; }
    }
	public enum CityAddressCountry
	{
		NoCountry = 0,
		Russia = 1,
		Ukraine = 2,
		Belarus = 3,
		Kazakhstan = 4,
        Bulgaria = 5,
        USA = 6
	}

	public sealed class CityAddress
	{
		public string Title { get; set; }
		public string Area { get; set; }
		public string Region { get; set; }
		public string Country { get; set; }
        private string _description = string.Empty;

        public CityAddress()
        {
            DoubleLatitude = 0.0;
            DoubleLongitude = 0.0;
        }

        public string Description
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_description))
                    return _description;
                var result = string.IsNullOrWhiteSpace(Country)? "": Country;
                if (!string.IsNullOrWhiteSpace(Region) &&
                    !string.Equals(Region, Title, StringComparison.InvariantCultureIgnoreCase))
                    result = string.Concat($"{Region}, ", result);
                if (!string.IsNullOrWhiteSpace(Area))
                    result = string.Concat($"{Area}, ", result);
                return result;
            }
            set => _description=value;
        }

        public double DoubleLatitude { get; private set; }
        public string Latitude 
        {
            get => DoubleLatitude.ToString("0.000000", CultureInfo.InvariantCulture);
            set => DoubleLatitude=double.TryParse(value,NumberStyles.Any, CultureInfo.InvariantCulture, out var inDouble)? inDouble: 0;
        }
        public double DoubleLongitude { get; private set; }
        public string Longitude
        {
            get => DoubleLongitude.ToString("0.000000", CultureInfo.InvariantCulture);
            set => DoubleLongitude=double.TryParse(value,NumberStyles.Any, CultureInfo.InvariantCulture, out var inDouble)? inDouble: 0;
        }
		public bool IsCoordsAvailable => DoubleLatitude!=0 && DoubleLongitude!=0;
        public MapBounds Bound { get; set; }
        
        public CityAddress(Address fullAddress)
        {
            Country = fullAddress.Country;
            Region = fullAddress.Region;
            Area = fullAddress.Area;
            Title = fullAddress.City;
        }

        public override string ToString()
        {
            var result = string.Empty;
            result = Utility.AddressMaker(result, Country);
            result = Utility.AddressMaker(result, Region);
            result = Utility.AddressMaker(result, Area);
            result = Utility.AddressMaker(result, Title);
            return result;
        }

        public override int GetHashCode()
        {
            var hashCode = (int)DoubleLatitude ^ (int)DoubleLongitude;
            hashCode = (hashCode * 397) ^ Title.GetHashCode();
            hashCode = (hashCode * 397) ^ Region.GetHashCode();
            hashCode = (hashCode * 397) ^ Country.GetHashCode();
            hashCode = (hashCode * 397) ^ Title.GetHashCode();
            return hashCode;
        }
    }
    
}