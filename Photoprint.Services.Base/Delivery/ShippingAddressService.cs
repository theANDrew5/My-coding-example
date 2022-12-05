using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Photoprint.Core;
using Photoprint.Core.Models;
using Photoprint.Core.Repositories;
using Photoprint.Core.Services;

namespace Photoprint.Services
{
    public class ShippingAddressService: BaseService, IShippingAddressService, IUserManualShippingAddressService
    {
        private readonly ICacheService _cacheService;
        private readonly IShippingAddressRepository _shippingAddressRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPhotolabService _photolabService;
        public ShippingAddressService(
            ICacheService cacheService,
            IShippingAddressRepository shippingAddressRepository,
            IAuthenticationService authenticationService,
            IPhotolabService photolabService)
        {
            _cacheService = cacheService;
            _shippingAddressRepository = shippingAddressRepository;
            _authenticationService = authenticationService;
            _photolabService = photolabService;
        }

        #region KEYS

        private string GetAddressKey(int id)
        {
            return $"shipping address (aid:{id})";
        }

        #endregion

        #region UserManual

        private void ValidatePermissionsAndThrow(Shipping shipping)
        {
            var user = _authenticationService.LoggedInUser;
            var lab = _photolabService.GetById(shipping.PhotolabId);
            if (lab is null || user is null || (!user.IsFrontendAdministrator(lab) && !user.IsManager(lab) && !user.IsAdministrator))
                throw new PhotoprintValidationException("You don't have access to shipping settings", string.Empty);
        }

        public ShippingAddress AddCourierAddress(ShippingAddress address, Courier courier)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));
            if (courier == null) throw new ArgumentNullException(nameof(courier));


            ValidatePermissionsAndThrow(courier);

            address.ShippingId = courier.Id;

            return Create(address, courier);
        }

        public ShippingAddress AddPostalAddress(ShippingAddress address, Postal postal)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));
            if (postal == null) throw new ArgumentNullException(nameof(postal));


            ValidatePermissionsAndThrow(postal);

            address.ShippingId = postal.Id;
            if (!address.PriceId.HasValue)
            {
                var defaultPriceId = postal.ServiceProviderSettings is IPriceCalculationProviderSettings settings ? settings.DefaultShippingPriceId : null;
                address.PriceId = defaultPriceId;
            }

            return Create(address, postal);
        }

        public void DeleteShippingAddresses(IReadOnlyCollection<int> shippingAddresses, Shipping shipping)
        {
            if (shippingAddresses is null) throw new ArgumentNullException(nameof(shippingAddresses));
            if (shipping is null) throw new ArgumentNullException(nameof(shipping));

            if (shippingAddresses.Count == 0) return;

            ValidatePermissionsAndThrow(shipping);

            _shippingAddressRepository.Delete(shippingAddresses);

            _cacheService.Invalidate(shipping).WithRemote();
        }


        public void UpdateShippingAddress(ShippingAddress address, Shipping shipping)
        {
            if (address is null) throw new ArgumentNullException(nameof(address));
            if (shipping is null) throw new ArgumentNullException(nameof(shipping));

            ValidatePermissionsAndThrow(shipping);

            Update(address, shipping);
            _cacheService.Invalidate(shipping.GetCacheTag()).WithRemote();
        }

        public void UpdateShippingAddressPositions(Dictionary<int, int> positions, Shipping shipping)
        {
            _shippingAddressRepository.UpdateShippingAddressPositions(positions);
            _cacheService.Invalidate(shipping.GetCacheTag()).WithRemote();
        }

        #endregion

        #region CREATE

        public ShippingAddress Create(ShippingAddress address, Shipping shipping)
        {

            _cacheService.Invalidate(shipping).WithRemote();

            var newAddress = _shippingAddressRepository.Create(address);
            var key = GetAddressKey(newAddress.Id);
            _cacheService.Add(key, newAddress, new List<string>{shipping.GetCacheTag()});
            return newAddress;
        }

        public IReadOnlyCollection<ShippingAddress> CreateList(IReadOnlyCollection<ShippingAddress> addresses,
            Shipping shipping)
        {
            _cacheService.Invalidate(shipping).WithRemote();
            return _shippingAddressRepository.CreateList(addresses);
        }

        #endregion

        #region GET

        public ShippingAddress GetById(int id, Shipping shipping = null)
        {
            return shipping is null
                ? _shippingAddressRepository.GetById(id)
                : _cacheService.GetFromCache(() => _shippingAddressRepository.GetById(id), GetAddressKey(id), CacheTime.Long, shipping);
        }
        public IReadOnlyCollection<ShippingAddress> GetListByIds(IReadOnlyCollection<int> addressesIds)
        {
            return _shippingAddressRepository.GetListByIds(addressesIds);
        }

        public IReadOnlyCollection<ShippingAddress> GetList(Shipping shipping)
        {
            var key = $"shipping addresses (sid:{shipping.Id})";
            return _cacheService.GetFromCache(
                () => _shippingAddressRepository.GetList(shipping.Id).ToArray(), key, CacheTime.Long, shipping);
        }

        public IReadOnlyCollection<ShippingAddress> GetAvailableList(Shipping shipping)
        {
            var key = $"available shipping addresses (sid:{shipping.Id})";
            return _cacheService.GetFromCache(
                () => _shippingAddressRepository.GetList(shipping.Id).Where(sa => sa.AvailableOnSite).ToArray(), key, CacheTime.Long, shipping);
        }

        public ShippingAddress GetSuitableShipingAddresses(Shipping selectedShipping, string selectedCountry, string selectedRegion, string selectedCity, string address = null)
        {
            selectedCountry = (selectedCountry ?? string.Empty).Trim();
            selectedRegion = (selectedRegion ?? string.Empty).Trim();
            selectedCity = (selectedCity ?? string.Empty).Trim();
            address = address ?? string.Empty;
            address = address.Trim();

            if (!(selectedShipping is Postal) || selectedShipping.ShippingServiceProviderType == ShippingServiceProviderType.General) return null;

            const StringComparison comp = StringComparison.InvariantCultureIgnoreCase;
            var shippingAddresses = GetAvailableList(selectedShipping);

            var fittedAddress = (from a in shippingAddresses
                                 where a.Country.Equals(selectedCountry, comp) && a.Region.Equals(selectedRegion, comp) && a.City.Equals(selectedCity, comp) && a.AddressLine1.Equals(address, comp)
                                 select a).FirstOrDefault();
            if (fittedAddress != null) return fittedAddress;

            fittedAddress = (from a in shippingAddresses
                             where a.Country.Equals(selectedCountry, comp) && a.City.Equals(selectedCity, comp) && a.AddressLine1.Equals(address, comp)
                             select a).FirstOrDefault();
            if (fittedAddress != null) return fittedAddress;

            fittedAddress = (from a in shippingAddresses
                             where a.Country.Equals(selectedCountry, comp) && a.City.Equals(selectedCity, comp)
                             select a).FirstOrDefault();
            if (fittedAddress != null) return fittedAddress;

            fittedAddress = (from a in shippingAddresses
                             where a.Country.Equals(selectedCountry, comp) && a.Region.Equals(selectedRegion, comp)
                             select a).FirstOrDefault();

            if (fittedAddress != null) return fittedAddress;

            fittedAddress = (from a in shippingAddresses
                             where a.Country.Equals(selectedCountry, comp) && a.City.Equals(selectedCity, comp)
                             select a).FirstOrDefault();

            if (fittedAddress != null) return fittedAddress;

            if (string.IsNullOrEmpty(selectedCity) && string.IsNullOrEmpty(selectedRegion))
            {
                fittedAddress = (from a in shippingAddresses
                                 where a.Country.Equals(selectedCountry, comp)
                                 select a).FirstOrDefault();

                if (fittedAddress != null) return fittedAddress;
            }

            if (selectedShipping is Postal postal && postal.PostalType == PostalType.ToClientDelivery)
            {
                fittedAddress = (from a in shippingAddresses
                                 where a.Country.Equals(selectedCountry, comp)
                                 select a).FirstOrDefault();
            }

            return fittedAddress;
        }

        public IReadOnlyDictionary<ShippingSmallToDeliveryModel, IReadOnlyCollection<ShippingAddress>>
            GetShippingsSmallWithAddresses(Photolab photolab,
                CityAddress city,
                IReadOnlyCollection<ShoppingCartItem> items,
                IReadOnlyCollection<int> shippingsIds, IShippingProviderResolverService providerResolver)
        {
            var shippingsInfo = _shippingAddressRepository.GetShippingsSmallWithAddresses(
                photolab.Id,
                currentOrderPrice: items.Sum(i => i.Price),
                currentOrderWeight: (decimal)items.Sum(i => i.TotalWeight), cityTitle: GetTitle(city.Title, true),
                availableShippingsIds: shippingsIds);

            var gropedAddressses = shippingsInfo
                .GroupBy(kp => kp.shipping, kp => kp.address, new ShippingSmallToDeliveryComparer());
            var result = new Dictionary<ShippingSmallToDeliveryModel, IReadOnlyCollection<ShippingAddress>>();
            foreach (var group in gropedAddressses)
            {
                var shipping = group.Key;
                switch (shipping.Type)
                {
                    case ShippingType.Postal:
                    {
                        switch (shipping.ServiceProviderType)
                        {
                            case ShippingServiceProviderType.Pickpoint:
                            case ShippingServiceProviderType.DDeliveryV2:
                                result.Add(shipping, new List<ShippingAddress>());
                                continue;
                            case ShippingServiceProviderType.RussianPost:
                            {
                                var provider = providerResolver?.GetProvider(shipping);
                                if (!(provider is IRussianPostProviderService service)) continue;
                                var filteredAddresses = service.GetShippingAddressesByCity(shipping, city);
                                shipping.AddressesCount = filteredAddresses.Count;
                                result.Add(shipping, filteredAddresses);
                                continue;
                            }
                            case ShippingServiceProviderType.General:
                            {
                                if (shipping.PostalType == PostalType.ToStorageDelivery)
                                {
                                    var filteredAddresses = group
                                        .Where(sa => CheckShippingAddress(sa, city, true)).ToList();
                                    shipping.AddressesCount = filteredAddresses.Count;
                                    result.Add(shipping, filteredAddresses);
                                    continue;
                                }

                                var filteredAddress = GetSuitableShipingAddresses(city, group.ToList());
                                shipping.AddressesCount = filteredAddress is null ? 0 : 1;
                                result.Add(shipping, filteredAddress is null ? new List<ShippingAddress>() : new List<ShippingAddress>(){filteredAddress});
                                continue;
                            }
                            default:
                            {
                                var settings = shipping.ServiceProviderSettings;
                                if (!settings.SupportAddresesSynchronization && settings.ShowAddressTab)
                                {
                                    var filteredAddress = GetSuitableShipingAddresses(city, group.ToList());
                                    shipping.AddressesCount = filteredAddress is null ? 0 : 1;
                                    result.Add(shipping, filteredAddress is null ? new List<ShippingAddress>() : new List<ShippingAddress>(){filteredAddress});
                                    continue;
                                }
                                var filteredAddresses = group
                                    .Where(sa =>
                                        CheckShippingAddress(sa, city, shipping.PostalType == PostalType.ToStorageDelivery))
                                    .ToList();
                                shipping.AddressesCount = filteredAddresses.Count;
                                result.Add(shipping, filteredAddresses);
                                continue;
                            }
                        }
                    }
                    case ShippingType.Point:
                    {
                        var filteredAddresses = group
                            .Where(sa => CheckShippingAddress(sa, city, true)).ToList();
                        shipping.AddressesCount = filteredAddresses.Count;
                        result.Add(shipping, filteredAddresses);
                        continue;
                    }
                    case ShippingType.Courier:
                    {
                        var filteredAddress = GetSuitableShipingAddresses(city, group.ToList());
                        shipping.AddressesCount = filteredAddress is null ? 0 : 1;
                        result.Add(shipping, filteredAddress is null ? new List<ShippingAddress>() : new List<ShippingAddress>(){filteredAddress});
                        continue;
                    }
                }
            }
            

            return result;
        }

        #endregion

        #region DELETE

        public void DeleteList(IReadOnlyCollection<ShippingAddress> shippingAddresses, Shipping shipping)
        {
            if (shippingAddresses is null) throw new ArgumentNullException(nameof(shippingAddresses));
            if (shipping is null) throw new ArgumentNullException(nameof(shipping));

            var shippingAddressIds = shippingAddresses.Where(sa => sa != null).Select(sa => sa.Id).ToArray();
            if(shippingAddressIds.Length == 0) return;

            _shippingAddressRepository.Delete(shippingAddressIds);

            _cacheService.Invalidate(shipping.GetCacheTag()).WithRemote();
        }

        #endregion

        #region UPDATE

        public void Update(ShippingAddress address, Shipping shipping)
        {
            if (address is null) throw new ArgumentNullException(nameof(address));
            if (shipping is null) throw new ArgumentNullException(nameof(shipping));
            
            _shippingAddressRepository.Update(address);
            
            _cacheService.Invalidate(shipping.GetCacheTag()).WithRemote();
        }

        public void UpdateList(IReadOnlyCollection<ShippingAddress> shippingAddresses, Shipping shipping)
        {
            if (shippingAddresses is null) throw new ArgumentNullException(nameof(shippingAddresses));
            if (shipping is null) throw new ArgumentNullException(nameof(shipping));
            
            _shippingAddressRepository.UpdateList(shippingAddresses);
            
            _cacheService.Invalidate(shipping.GetCacheTag()).WithRemote();
        }
        public void UpdateListStatus(IReadOnlyCollection<int> addressesIds, Shipping shipping, bool status)
        {
            if (addressesIds is null) throw new ArgumentNullException(nameof(addressesIds));
                _shippingAddressRepository.UpdateListStatus(addressesIds, shipping.Id, status);
            

            _cacheService.Invalidate(shipping.GetCacheTag()).WithRemote();
        }

        #endregion

        public void Copy(Shipping source, Shipping target, Photolab targetPhotolab)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (target is null) throw new ArgumentNullException(nameof(target));
            if (targetPhotolab is null) throw new ArgumentNullException(nameof(targetPhotolab));


            _shippingAddressRepository.Copy(source.Id, target.Id, targetPhotolab.Id);
        }

        #region Services

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
                    (city.Bound?.IsCoordInBound(shippingAddress.Latitude, shippingAddress.Longitude) ?? false));
        }

        private class ShippingSmallToDeliveryComparer : IEqualityComparer<ShippingSmallToDeliveryModel>
        {
            public bool Equals(ShippingSmallToDeliveryModel x, ShippingSmallToDeliveryModel y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Id == y.Id;
            }

            public int GetHashCode(ShippingSmallToDeliveryModel obj)
            {
                return obj.Id;
            }
        }

        private IReadOnlyCollection<ShippingAddress> GetRussianPostShippingAddresses(Shipping shipping,
            CityAddress city, Language language, IRussianPostProviderService service)
        {
            if (!(shipping is Postal postal) || postal.ShippingServiceProviderType != ShippingServiceProviderType.RussianPost)
                return Array.Empty<ShippingAddress>();

            return service.GetShippingAddressesByCity(postal, city);
        }
        private ShippingAddress GetSuitableShipingAddresses(CityAddress city, IReadOnlyCollection<ShippingAddress> addresses)
        {
            var notNullAddresses = addresses.Where(sa => sa != null).ToList();

            var selectedCountry = GetTitle(city.Country);
            var selectedRegion = GetTitle(city.Region);
            var selectedCity = GetTitle(city.Title);

            const StringComparison comp = StringComparison.InvariantCultureIgnoreCase;

            ShippingAddress fittedAddress = null;

            fittedAddress = notNullAddresses.FirstOrDefault(a =>
                GetTitle(a.Country).Equals(selectedCountry, comp) && GetTitle(a.Region).Equals(selectedRegion, comp) &&
                GetTitle(a.City).Equals(selectedCity, comp));
            if (fittedAddress != null) return fittedAddress;

            var regionAddresses = notNullAddresses.Where(a => string.IsNullOrWhiteSpace(a.City)
                                                              && !string.IsNullOrWhiteSpace(a.Region) && !string.IsNullOrWhiteSpace(a.Country));
            fittedAddress = regionAddresses.FirstOrDefault(a =>
                GetTitle(a.Country).Equals(selectedCountry, comp) && GetTitle(a.Region).Equals(selectedRegion, comp));
            if (fittedAddress != null) return fittedAddress;

            var countryAddresses = notNullAddresses.Where(a => string.IsNullOrWhiteSpace(a.City)
                                                               && string.IsNullOrWhiteSpace(a.Region) && !string.IsNullOrWhiteSpace(a.Country));
            fittedAddress = countryAddresses.FirstOrDefault(a => GetTitle(a.Country).Equals(selectedCountry, comp));
            return fittedAddress;
        }
        #endregion
    }
}
