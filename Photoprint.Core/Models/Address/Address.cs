using System;
using System.Collections.Generic;

namespace Photoprint.Core.Models
{
    public static class AddressDBConstraints
    {
        public static class Max
        {
            public const int CountryLength = 100;
            public const int RegionLength = 100;
            public const int AreaLength = 100;
            public const int CityLength = 100;
            public const int DistrictLength = 200;
            public const int StreetLength = 500;
            public const int HouseLength = 500;
            public const int FlatLength = 500;
            public const int AddressLine1Length = 1000;
            public const int AddressLine2Length = 1000;
            public const int PostalCodeLength = 50;
            public const int LatitudeLength = 50;
            public const int LongitudeLength = 50;
            public const int PhoneLength = 200;
        }
    }

    public class Address
    {
        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set => _description = value ?? string.Empty;
        }

        private string _country = string.Empty;
        public string Country
        {
            get => _country;
            set => _country = value.Cut(AddressDBConstraints.Max.CountryLength);
        }

        private string _region = string.Empty;
        public string Region
        {
            get => _region;
            set => _region = value.Cut(AddressDBConstraints.Max.RegionLength);
        }

        private string _area = string.Empty;
        public string Area
        {
            get => _area;
            set => _area = value.Cut(AddressDBConstraints.Max.AreaLength);
        }

        private string _city = string.Empty;
        public string City
        {
            get => _city;
            set => _city = value.Cut(AddressDBConstraints.Max.CityLength);
        }

        private string _district = string.Empty;
        public string District
        {
            get => _district;
            set => _district = value.Cut(AddressDBConstraints.Max.DistrictLength);
        }

        private string _street = string.Empty;
        public string Street
        {
            get => _street;
            set => _street = value.Cut(AddressDBConstraints.Max.StreetLength);
        }

        private string _house = string.Empty;
        public string House
        {
            get => _house;
            set => _house = value.Cut(AddressDBConstraints.Max.HouseLength);
        }

        private string _flat = string.Empty;
        public string Flat
        {
            get => _flat;
            set => _flat = value.Cut(AddressDBConstraints.Max.FlatLength);
        }

        private string _addressLine1 = string.Empty;
        public string AddressLine1
        {
            get => _addressLine1;
            set => _addressLine1 = value.Cut(AddressDBConstraints.Max.AddressLine1Length);
        }

        private string _addressLine2 = string.Empty;
        public string AddressLine2
        {
            get => _addressLine2;
            set => _addressLine2 = value.Cut(AddressDBConstraints.Max.AddressLine2Length);
        }

        private string _postalCode = string.Empty;
        public string PostalCode
        {
            get => _postalCode;
            set => _postalCode = value.Cut(AddressDBConstraints.Max.PostalCodeLength);
        }

        private string _latitude = string.Empty;
        public string Latitude
        {
            get => _latitude;
            set => _latitude = value.Cut(AddressDBConstraints.Max.LatitudeLength);
        }

        private string _longitude = string.Empty;
        public string Longitude
        {
            get => _longitude;
            set => _longitude = value.Cut(AddressDBConstraints.Max.LongitudeLength);
        }

        private string _phone = string.Empty;
        public string Phone
        {
            get => _phone;
            set => _phone = value.Cut(AddressDBConstraints.Max.PhoneLength);
        }

        public string FullRegion => City + ", " + Region + ", " + Country;
        
        public override string ToString() => ToString(true);
        public string ToString(bool includeZipCode)
		{
			var address = string.Empty;
            if (includeZipCode) address = Utility.AddressMaker(address, PostalCode);
            address = Utility.AddressMaker(address, Country);
            if (City != Region) address = Utility.AddressMaker(address, Region);
            address = Utility.AddressMaker(address, City);
            address = Utility.AddressMaker(address, District);
            if (!string.IsNullOrWhiteSpace(AddressLine1) || !string.IsNullOrWhiteSpace(AddressLine2))
			{
				address = Utility.AddressMaker(address, AddressLine1);
				address = Utility.AddressMaker(address, AddressLine2);
			}
			else
			{
				address = Utility.AddressMaker(address, Street);
				address = Utility.AddressMaker(address, House);
				address = Utility.AddressMaker(address, Flat);
			}

			if (!string.IsNullOrWhiteSpace(Description))
			{
				address = $"{address} ({Description})";
			}

            return address;
		}        
        public string ToString(bool includeZipCode, bool includeCountry)
        {
            var address = string.Empty;
            if (includeZipCode) address = Utility.AddressMaker(address, PostalCode);
            if (includeCountry) address = Utility.AddressMaker(address, Country);

            address = Utility.AddressMaker(address, Region);
            address = Utility.AddressMaker(address, City);
            address = Utility.AddressMaker(address, District);
            if (!string.IsNullOrWhiteSpace(AddressLine1) || !string.IsNullOrWhiteSpace(AddressLine2))
            {
                address = Utility.AddressMaker(address, AddressLine1);
                address = Utility.AddressMaker(address, AddressLine2);
            }
            else
            {
                address = Utility.AddressMaker(address, Street);
                address = Utility.AddressMaker(address, House);
                address = Utility.AddressMaker(address, Flat);
            }

			return address;
		}
        public string ToAddressString()
        {
            if (!string.IsNullOrWhiteSpace(AddressLine1))
            {
                return Utility.AddressMaker(string.Empty, AddressLine1);
            }

            var address = Utility.AddressMaker(string.Empty, Street);
            address = Utility.AddressMaker(address, House);
            address = Utility.AddressMaker(address, Flat);

            return address;
        }

        public void Merge(Address address)
        {
            AddressLine1 = address.AddressLine1;
            AddressLine2 = address.AddressLine2;
            Country = address.Country;
            Region = address.Region;
            Area = address.Area;
            City = address.City;
            Street = address.Street;
            House = address.House;
            Flat = address.Flat;
            Description = address.Description;
            Phone = address.Phone;
            Longitude = address.Longitude;
            Latitude = address.Latitude;
            PostalCode = address.PostalCode;
        }

        public void Merge(AddressDTO dto)
        {
            Country = dto?.Country ?? string.Empty;
            City = dto?.City ?? string.Empty;
            Region = dto?.Region ?? string.Empty;
            AddressLine1 = dto?.AddressLine ?? string.Empty;
            Street = dto?.Street ?? string.Empty;   
            House = dto?.House ?? string.Empty;
            Flat = dto?.Flat ?? string.Empty;
            Latitude = dto?.Latitude ?? string.Empty;
            Longitude = dto?.Longitude ?? string.Empty;
            Description = dto?.Description ?? string.Empty;
            PostalCode = dto?.PostalCode ?? string.Empty;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Address()
        {
            Country = string.Empty;
            Region = string.Empty;
            City = string.Empty;
            District = string.Empty;
            Street = string.Empty;
            House = string.Empty;
            Flat = string.Empty;
            AddressLine1 = string.Empty;
            AddressLine2 = string.Empty;
            Latitude = string.Empty;
            Longitude = string.Empty;
            Phone = string.Empty;
        }

        public Address(Address address)
        {
            City = address?.City ?? string.Empty;
            Country = address?.Country ?? string.Empty;
            District = address?.District ?? string.Empty;
            Region = address?.Region ?? string.Empty;
            PostalCode = address?.PostalCode ?? string.Empty;
            AddressLine1 = address?.AddressLine1 ?? string.Empty;
            AddressLine2 = address?.AddressLine2 ?? string.Empty;
            Street = address?.Street ?? string.Empty;
            House = address?.House ?? string.Empty;
            Flat = address?.Flat ?? string.Empty;
            Latitude = address?.Latitude ?? string.Empty;
            Longitude = address?.Longitude ?? string.Empty;
            Description = address?.Description ?? string.Empty;
            Phone = address?.Phone ?? string.Empty;
        }

        public Address(AddressDTO dto)
        {
            Country = dto?.Country ?? string.Empty;
            City = dto?.City ?? string.Empty;
            Region = dto?.Region ?? string.Empty;
            AddressLine1 = dto?.AddressLine ?? string.Empty;
            Street = dto?.Street ?? string.Empty;   
            House = dto?.House ?? string.Empty;
            Flat = dto?.Flat ?? string.Empty;
            Latitude = dto?.Latitude ?? string.Empty;
            Longitude = dto?.Longitude ?? string.Empty;
            Description = dto?.Description ?? string.Empty;
            PostalCode = dto?.PostalCode ?? string.Empty;
        }

    }

    public class AddressContentComparer<TAddress>: IEqualityComparer<TAddress>
    where TAddress : Address
    {
        public bool Equals(TAddress x, TAddress y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;

            return
                x.AddressLine1.Trim().Equals(y.AddressLine1.Trim(),
                    StringComparison.InvariantCultureIgnoreCase) &&
                x.AddressLine2.Trim().Equals(y.AddressLine2.Trim(),
                    StringComparison.InvariantCultureIgnoreCase) &&
                x.Country.Trim().Equals(y.Country.Trim(),
                    StringComparison.InvariantCultureIgnoreCase) &&
                x.Region.Trim().Equals(y.Region.Trim(),
                    StringComparison.InvariantCultureIgnoreCase) &&
                x.Area.Trim().Equals(y.Area.Trim(),
                    StringComparison.InvariantCultureIgnoreCase) &&
                x.City.Trim().Equals(y.City.Trim(),
                    StringComparison.InvariantCultureIgnoreCase) &&
                x.Street.Trim().Equals(y.Street.Trim(),
                    StringComparison.InvariantCultureIgnoreCase) &&
                x.House.Trim().Equals(y.House.Trim(),
                    StringComparison.InvariantCultureIgnoreCase) &&
                x.Flat.Trim().Equals(y.Flat.Trim(),
                    StringComparison.InvariantCultureIgnoreCase) &&
                x.Description.Trim().Equals(y.Description.Trim(),
                    StringComparison.InvariantCultureIgnoreCase) &&
                x.Phone.Trim().Equals(y.Phone.Trim(),
                    StringComparison.InvariantCultureIgnoreCase) &&
                x.Longitude.Trim().Equals(y.Longitude.Trim(),
                    StringComparison.InvariantCultureIgnoreCase) &&
                x.Latitude.Trim().Equals(y.Latitude.Trim(),
                    StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(TAddress obj)
        {

            unchecked
            {
                var hashCode = obj.AddressLine1?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ obj.AddressLine2?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ obj.Country?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ obj.Region?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ obj.Area?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ obj.City?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ obj.Street?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ obj.House?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ obj.Flat?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ obj.Description?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ obj.Phone?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ obj.Longitude?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ obj.Latitude?.GetHashCode() ?? 0;
                return hashCode;
            }
        }
    }

    public sealed class BoundedAddress : Address
    {
        public MapBounds Bounds { get; set; }
    }
}
