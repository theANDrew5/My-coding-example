using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Photoprint.Core.Models
{
    public class AddressHierarchyDto
    {
        public class CountryInfo
        {
            public string Country { get; }
            public List<RegionsInfo> Regions { get; }
            public CountryInfo(string country)
            {
                Country = country;
                Regions = new List<RegionsInfo>();
            }
        }

        public class RegionsInfo
        {
            public string Region { get; set; }
            public List<CityInfo> Cities { get; set; }

            public RegionsInfo(string region)
            {
                Region = region;
                Cities = new List<CityInfo>();
            }
        }

        public class CityInfo
        {
            public string City { get; }
            public List<AddressInfo> Addresses { get; }

            public CityInfo(string city)
            {
                City = city;
                Addresses = new List<AddressInfo>();
            }
        }

        public class AddressInfo
        {
            public int Id { get; }
            public string AddressName { get; }
            public string AddressLine { get; }
            public string Street { get; }
            public string House { get; }
            public string PostalCode { get; }
            public string Latitude { get; }
            public string Longitude { get; }
            public string Phone { get; }
            public string Worktime { get; }
            [CanBeNull] public JObject DeliveryProperties { get; }

            public AddressInfo(ShippingAddress address, bool addProps) {
                Id = address.Id;
                AddressName = address.AddressName;
                AddressLine = !string.IsNullOrWhiteSpace(address.AddressLine1)?
                    address.AddressLine1:
                    !string.IsNullOrWhiteSpace(address.Street) && !string.IsNullOrWhiteSpace(address.House)?
                        $"{address.Street}, {address.House}" : 
                        string.Empty;
                Street = address.Street;
                House = address.House;
                PostalCode = address.PostalCode;
                Latitude = address.Latitude;
                Longitude = address.Longitude;
                Phone = address.Phone;
                Worktime = address.WorkTime;
                DeliveryProperties = addProps ? JObject.FromObject(address.DeliveryProperties) : null;
            }
        }

        public List<CountryInfo> Countries { get; }

        public AddressHierarchyDto(IEnumerable<ShippingAddress> addresses, bool skipSort = false, bool addProps = false)
        {
            var sc = StringComparison.OrdinalIgnoreCase;
            Countries = new List<CountryInfo>();
            foreach (var address in addresses)
            {
                var countryInfo = Countries.FirstOrDefault(c => c.Country.Equals(address.Country, sc));
                if (countryInfo == null)
                {
                    countryInfo = new CountryInfo(address.Country);
                    Countries.Add(countryInfo);
                }
                var statesInfo = countryInfo.Regions.FirstOrDefault(c => c.Region.Equals(address.Region, sc));
                if (statesInfo == null)
                {
                    statesInfo = new RegionsInfo(address.Region);
                    countryInfo.Regions.Add(statesInfo);
                }

                var citiesInfo = statesInfo.Cities.FirstOrDefault(c => c.City.Equals(address.City, sc));
                if (citiesInfo == null)
                {
                    citiesInfo = new CityInfo(address.City);
                    statesInfo.Cities.Add(citiesInfo);
                }

                var addressInfo = citiesInfo.Addresses.FirstOrDefault(c => c.Id.Equals(address.Id));
                if (addressInfo != null) continue;
                addressInfo = new AddressInfo(address, addProps);
                citiesInfo.Addresses.Add(addressInfo);
            }

            if (skipSort) return;
            Countries.Sort((a, b) => string.Compare(a.Country, b.Country, StringComparison.OrdinalIgnoreCase));
            foreach (var country in Countries)
            {
                country.Regions.Sort((a, b) => string.Compare(a.Region, b.Region, StringComparison.OrdinalIgnoreCase));
                foreach (var state in country.Regions)
                {
                    state.Cities.Sort((a, b) => string.Compare(a.City, b.City, StringComparison.OrdinalIgnoreCase));
                    foreach (var city in state.Cities)
                    {
                        city.Addresses.Sort((a, b) => string.Compare(a.AddressLine, b.AddressLine, StringComparison.OrdinalIgnoreCase));
                    }
                }
            }
        }
    }
}