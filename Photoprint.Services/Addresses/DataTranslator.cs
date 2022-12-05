using Photoprint.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Address = Photoprint.Core.Models.Address;

namespace Photoprint.Services
{

    public static class DataTranslator
    {
        #region Common
        private const string _ignoreWords="Респ"; 
        private const string _separators=@"(-на)?"; 
		private static string _pattern = $@"(?!{_ignoreWords})"+@"(?<Title>([А-ЯЁA-ZİЇI]{1}[а-яёa-zії]+){1}"+_separators+@"([- ][А-ЯЁA-Zİ]{1}[а-яёa-z]+)?)"+$@"(?!{_ignoreWords})";
        private static Regex _regionRegex = new Regex(_pattern,RegexOptions.Compiled);
		public static string GetTitle(string fullName, bool eCase=false, bool regexEndLineCase=false)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return string.Empty;

			var title = _regionRegex.Match(fullName.Trim()).Groups["Title"].Value;
            title = eCase ? title.Replace("ё", "[её]") : title;
            title = regexEndLineCase ? string.Concat(title, @"$") : title;
            return title;	
		}

        public static bool CheckShippingAddress(ShippingAddress shippingAddress, CityAddress city,
            bool pickPoint = false)
        {
            if (shippingAddress == null || city == null)
                return false;
            var sCity = GetTitle(shippingAddress.City);
            var cCity =  GetTitle(city.Title, true, true);
            var sRegion =  GetTitle(shippingAddress.Region?? string.Empty);
            var cRegion =  GetTitle(city.Region?? string.Empty);
            var sArea = GetTitle(shippingAddress.Area?? string.Empty);
            var cArea = GetTitle(city.Area?? string.Empty);
            return Regex.IsMatch(sCity, cCity) &&
                   (Regex.IsMatch(sRegion, cRegion) && (string.IsNullOrWhiteSpace(sArea) ||
                                                        string.IsNullOrWhiteSpace(cArea) ||
                                                        Regex.IsMatch(sArea, cArea)) ||
                    (city.Bound?.IsCoordInBound(shippingAddress.Latitude, shippingAddress.Longitude) ?? false)) &&
                   (!pickPoint ||
                    (!string.IsNullOrWhiteSpace(shippingAddress.Street) &&
                     !string.IsNullOrWhiteSpace(shippingAddress.House) ||
                     !string.IsNullOrWhiteSpace(shippingAddress.AddressLine1)))
                   && (!pickPoint || (!string.IsNullOrWhiteSpace(shippingAddress.Latitude)
                                        && !string.IsNullOrWhiteSpace(shippingAddress.Longitude)));
        }

        private static Regex _phoneRegex = new Regex(@"[0-9]+", RegexOptions.Compiled);
        public static string CleanPhone(string phone, bool withoutPlus = false)
        {
            var matches = _phoneRegex.Matches(phone);
            var result = withoutPlus ? string.Empty : "+";
            foreach (Match match in matches)
                result = string.Concat(result, match.Value);
            return result;
        }

        public static string GetCountryCode(CityAddressCountry country)
        {
            switch (country)
            {
                case CityAddressCountry.Russia:
                    return "ru";
                case CityAddressCountry.Ukraine:
                    return "ua";
                case CityAddressCountry.Belarus:
                    return "by";
                case CityAddressCountry.Kazakhstan:
                    return "kz";
                case CityAddressCountry.Bulgaria:
                    return "bg";
                case CityAddressCountry.USA:
                    return "us";
                default:
                case CityAddressCountry.NoCountry:
                    return null;
            }
        }
        #endregion

        #region Google

        public static CityAddressCountry CountryConvert(string name)
        {
            return Enum.TryParse(name, out CityAddressCountry country) ? country : CityAddressCountry.NoCountry;
        }

        public static IReadOnlyList<CityAddress> GetCitiesFromGeoObj(ToponymType toponymType,
            IReadOnlyList<GoogleGeocodeResult> data)
        {
            switch (toponymType)
            {
                case ToponymType.Country:
                    return data.Where(d => d.Types[0] == "country")
                        .Select(GetCityFromGeoObj).ToList();
                case ToponymType.Region:
                    return data.Where(d => d.Types[0] == "administrative_area_level_1")
                        .Select(GetCityFromGeoObj).ToList();
                case ToponymType.City:
                    return data.Where(d => d.Types[0] == "locality")
                        .Select(GetCityFromGeoObj).ToList();
                default:
                    throw new ArgumentOutOfRangeException(nameof(toponymType), toponymType, null);
            }
        }
        public static CityAddress GetCityFromGeoObj(GoogleGeocodeResult data)
        {
            var title = data.AddressComponents.FirstOrDefault(c => c.Types[0] == "locality")?.LongName;
            var area = data.AddressComponents.FirstOrDefault(c => c.Types[0] == "administrative_area_level_2")?.LongName ?? string.Empty;
            var result = new CityAddress
            {
                Country = data.AddressComponents.FirstOrDefault(c => c.Types[0] == "country")?.LongName ?? "",
                Region = data.AddressComponents.FirstOrDefault(c => c.Types[0] == "administrative_area_level_1")?.LongName ?? title,
                Area = !Regex.IsMatch(area, $@"{title}$")? area: string.Empty,
                Title = title,
                Latitude = data.Geometry.Location.Latitude,
                Longitude = data.Geometry.Location.Longitude,
                Bound = GetBounds(data.Geometry)
            };
            return result;
        }

        public static BoundedAddress GetAddressFromGeoObj(GoogleGeocodeResult data)
        {
            var city = data.AddressComponents.FirstOrDefault(c => c.Types[0] == "locality")?.LongName;
            var house = data.AddressComponents.FirstOrDefault(c => c.Types[0] == "street_number")?.LongName;
            if (city == null) return null;
            var area = data.AddressComponents.FirstOrDefault(c => c.Types[0] == "administrative_area_level_2")?.LongName ?? string.Empty;
            var result = new BoundedAddress
            {
                Country = data.AddressComponents.FirstOrDefault(c => c.Types[0] == "country")?.LongName ?? "",
                Region = data.AddressComponents.FirstOrDefault(c => c.Types[0] == "administrative_area_level_1")?.LongName ?? city,
                Area = !Regex.IsMatch(area, $@"{city}$")? area: string.Empty,
                City = city,
                Street =  data.AddressComponents.FirstOrDefault(c => c.Types[0] == "route")?.LongName?? string.Empty,
                House = house,
                Latitude = data.Geometry.Location.Latitude,
                Longitude = data.Geometry.Location.Longitude,
                PostalCode = data.AddressComponents.FirstOrDefault(c => c.Types[0] == "postal_code")?.LongName?? string.Empty,
                Bounds = GetBounds(data.Geometry)
            };
            return result;
        }

        private static MapBounds GetBounds(GoogleGeometry geometry)
        {
            if (geometry == null) return null;
            return new MapBounds
            {
                UpperLatitude = geometry.Viewport.NorthEast.Latitude,
                UpperLongitude = geometry.Viewport.NorthEast.Longitude,
                LowerLatitude = geometry.Viewport.SouthWest.Latitude,
                LowerLongitude = geometry.Viewport.SouthWest.Longitude
            };
        }

        #endregion

        #region Yandex
        public static IReadOnlyList<CityAddress> GetCitiesFromGeoObj(ToponymType type,
            IReadOnlyCollection<YandexGeoObject> data)
        {
            switch (type)
            {
                case ToponymType.Country:
                    return data.Where(obj => obj.metaDataProperty.GeocoderMetaData.kind == "country")
                        .Select(GetCityFromGeoObj).Where(ca => ca != null).ToList();
                case ToponymType.Region:
                    return data.Where(obj => obj.metaDataProperty.GeocoderMetaData.kind == "province")
                        .Select(GetCityFromGeoObj).Where(ca => ca != null).ToList();
                case ToponymType.City:
                    return data.Where(obj => obj.metaDataProperty.GeocoderMetaData.kind == "locality")
                        .Select(GetCityFromGeoObj).Where(ca => ca != null).ToList();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static CityAddress GetCityFromGeoObj(YandexGeoObject data)
		{
			try
			{
				var latlon = data.Point.pos.Split(' ');
				var addressData = data.metaDataProperty.GeocoderMetaData.Address;
				var components = data.metaDataProperty.GeocoderMetaData.Address.Components.ToList();
				var dict = new Dictionary<string, string>(components.Count);
				components.ForEach(comp => { 
					if (dict.ContainsKey(comp.kind))
						dict[comp.kind] = comp.name;
					else
						dict.Add(comp.kind, comp.name);});
                return new CityAddress
                {
                    Title = dict.ContainsKey("locality") ? dict["locality"] : string.Empty,
                    Country = dict.ContainsKey("country") ? dict["country"] : string.Empty,
                    Region = dict.ContainsKey("province") ? dict["province"] : string.Empty,
                    Area = dict.ContainsKey("area") ? dict["area"] : string.Empty,
                    Description = addressData.formatted,
                    Latitude = latlon[1],
                    Longitude = latlon[0],
                    Bound = GetBounds(data.boundedBy)
                };
			} catch { }
			return null;
		}
        public static BoundedAddress GetAddressFromGeoObj(YandexGeoObject data)
        {
			var latlon = data.Point.pos.Split(' ');
			var addressData = data.metaDataProperty.GeocoderMetaData.Address;
			var components = data.metaDataProperty.GeocoderMetaData.Address.Components.ToList();
			var dict = new Dictionary<string, string>(components.Count);
			components.ForEach(comp => { 
				if (dict.ContainsKey(comp.kind))
					dict[comp.kind] = comp.name;
				else
					dict.Add(comp.kind, comp.name);});
			return new BoundedAddress()
			{
				Country = dict.ContainsKey("country") ? dict["country"] : string.Empty,
				Region = dict.ContainsKey("province") ? dict["province"] : string.Empty,
				Area = dict.ContainsKey("area") ? dict["area"] : string.Empty,
				City = dict.ContainsKey("locality") ? dict["locality"] : string.Empty,
				Street = dict.ContainsKey("street")? dict["street"] : string.Empty,
				House = dict.ContainsKey("house")? dict["house"] : string.Empty,
				Latitude = latlon[1],
				Longitude = latlon[0],
				PostalCode = addressData.postal_code,
				Description = data.description,
                Bounds = GetBounds(data.boundedBy)
			};
        }

        public static YandexCountry CountryConvert(CityAddressCountry country)
        {
			switch (country)
			{
				case CityAddressCountry.Russia:
					return YandexCountry.Russia;
				case CityAddressCountry.Ukraine:
					return YandexCountry.Ukraine;
				case CityAddressCountry.Belarus:
					return YandexCountry.Belarus;
				case CityAddressCountry.Kazakhstan:
					return YandexCountry.Kazakhstan;
                case CityAddressCountry.Bulgaria:
                    return YandexCountry.Bulgaria;
                case CityAddressCountry.USA:
                    return YandexCountry.USA;
                default:
					return YandexCountry.Everywhere;
            }
		}
        public static CityAddressCountry CountryConvert(YandexCountry yaCountry)
        {
            switch (yaCountry)
            {
                case YandexCountry.Russia:
                    return CityAddressCountry.Russia;
				case YandexCountry.Ukraine:
                    return CityAddressCountry.Ukraine;
                case YandexCountry.Belarus:
                    return CityAddressCountry.Belarus;
                case YandexCountry.Kazakhstan:
                    return CityAddressCountry.Kazakhstan;
                case YandexCountry.Bulgaria:
                    return CityAddressCountry.Bulgaria;
                case YandexCountry.USA:
                    return CityAddressCountry.USA;
                default:
                    return CityAddressCountry.NoCountry;
            }
        }
        public static string LanguageConvert(ILanguage language)
        {
			switch (language.LanguageCode)
            {
				default:
				case "ru":
					return "ru_RU";
				case "uk":
					return "uk_UA";
				case "tr":
					return "tr_TR";
				case "en":
					return "en_RU";
				case "be":
					return "be_BY";
            }
        }
        public static bool CheckSuggestData(string description, string nameStreet, string city, string street)
        {
            if (string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(nameStreet) ||
                string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(street))
                return false;

            var descs = Regex.Split(description, @", ");

            if (!GetTitle(street).Equals(GetTitle(nameStreet), StringComparison.InvariantCultureIgnoreCase)) return false;
            var cityTitle = GetTitle(city);
            return descs.Any(desc => GetTitle(desc).Equals(cityTitle, StringComparison.InvariantCultureIgnoreCase));
        }

        private static MapBounds GetBounds(YandexBound yBound)
        {
            if (string.IsNullOrWhiteSpace(yBound?.Envelope.lowerCorner) ||
                string.IsNullOrWhiteSpace(yBound.Envelope.upperCorner))
                return null;

            var lowerLongLat = yBound.Envelope.lowerCorner.Split(' ');
            var upperLongLat = yBound.Envelope.upperCorner.Split(' ');

            return new MapBounds
            {
                LowerLongitude = lowerLongLat[0],
                LowerLatitude = lowerLongLat[1],
                UpperLongitude = upperLongLat[0],
                UpperLatitude = upperLongLat[1]
            };
        }

    #endregion
		
    }
}

public enum YadexGeoDataTransformType
{
	Locality,
	Province
}