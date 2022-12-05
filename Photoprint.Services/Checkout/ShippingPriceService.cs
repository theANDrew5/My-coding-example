using System;
using System.Linq;
using Newtonsoft.Json;
using Photoprint.Core;
using JetBrains.Annotations;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using System.Collections.Generic;
using DocumentFormat.OpenXml.InkML;
using Photoprint.Core.Repositories;

namespace Photoprint.Services
{
    public class ShippingPriceService : ServiceBase, IShippingPriceService
    {
        private readonly IShippingAddressService _shippingAddressService;
        private readonly IShippingAddressPricesRepository _shippingAddressPricesRepository;
        private readonly IShippingAddressRepository _addressRepository;
        private readonly ICacheService _cacheService;

        public static byte[] GetPriceListHash([NotNull] ShippingPrices prices)
        {
            if (prices == null) throw new ArgumentNullException(nameof(prices));
            var str = JsonConvert.SerializeObject(prices);
            var hashString = str.GetMd5Hash();
            var result = Utility.Hash.ConvertHexStringToBytes(hashString);
            return result;
        }

        public ShippingPriceService(
            IShippingAddressService shippingAddressService,
            IShippingAddressPricesRepository shippingAddressPricesRepository,
            IShippingAddressRepository addressRepository,
            ICacheService cacheService)
        {
            _shippingAddressService = shippingAddressService;
            _shippingAddressPricesRepository = shippingAddressPricesRepository;
            _addressRepository = addressRepository;
            _cacheService = cacheService;
        }

        public ShippingAddressPrice GetOrCreateShippingAddressPrice(PhotolabSmall photolab, ShippingPrices shippingPrices)
        {
            if (shippingPrices == null) throw new ArgumentNullException(nameof(shippingPrices));
            if ((shippingPrices.Prices.Count == 0 || shippingPrices.Prices.All(price => price.IsDefault)) && shippingPrices.MaximumWeight <= 0)
                return new ShippingAddressPrice(null, photolab.Id, shippingPrices);

            var result = _shippingAddressPricesRepository.GetOrCreate(photolab.Id, GetPriceListHash(shippingPrices), shippingPrices, out var isNew);
            if (isNew)
                _cacheService.RemoveFromCache(GetListCacheKey(photolab)).WithRemote();

            return result;
        }

        private static string GetListCacheKey(PhotolabSmall photolab) => "photolab shipping prices: " + photolab.Id;

        public ShippingAddressPrice GetById([NotNull] PhotolabSmall photolab, int? priceId)
        {
            if (photolab == null) throw new ArgumentNullException(nameof(photolab));

            var key = GetListCacheKey(photolab);
            var list = _cacheService.GetFromCache(() => GetListByPhotolab(photolab), key, CacheTime.Default, photolab);

            return list.TryGetValue(priceId ?? 0, out var result) ? result : new ShippingAddressPrice(null, photolab.Id, new ShippingPrices());
        }

        private Dictionary<int, ShippingAddressPrice> GetListByPhotolab(PhotolabSmall photolab) => _shippingAddressPricesRepository.GetListByPhotolab(photolab.Id);

        public IReadOnlyCollection<ShippingAddressPrice> GetListByShipping([NotNull] Shipping shipping)
        {
            if (shipping == null) throw new ArgumentNullException(nameof(shipping));

            var key = $"prices for shipping: {shipping.Id}";
            return _cacheService.GetFromCache(() => GetList(shipping), key, CacheTime.Default, shipping);
        }

        private IReadOnlyCollection<ShippingAddressPrice> GetList(Shipping shipping)
        {
            var fromBase = _shippingAddressPricesRepository.GetByShippingId(shipping.Id);
            if (fromBase.Any())
                return fromBase;
            var addresses = _shippingAddressService.GetList(shipping);
            return addresses.Select(sa => new ShippingAddressPrice(null, shipping.PhotolabId, new ShippingPrices())).ToArray();
        }

        public bool TryCalculatePriceForShipping(Shipping shipping, IReadOnlyCollection<IShippable> items,
            out decimal resultPrice,
            out bool isFixed)
        {
            resultPrice = 0m;
            isFixed = true;
            var itemsWeight = items?.Sum(i => i.TotalWeight);
            if (shipping.IsAvailableWeightConstrain && shipping.MaximumWeight < itemsWeight) return false;
            switch (shipping)
            {
                case DistributionPoint point:
                {
                    var prceList = point.PriceList;
                    isFixed = prceList.Count() <= 1;
                    return TryCalculatePrice(prceList, items, out resultPrice);
                }
                case Courier courier:
                {
                    var addressPrices = courier.ShippingAddressPrices;
                    var priceResults = new List<(decimal, ShippingAddressPrice)>();
                    foreach (var addressPrice in addressPrices)
                    {
                        if (!TryCalculatePrice(addressPrice.PriceList, items, out var apResult))
                            continue;
                        priceResults.Add((apResult, addressPrice));
                    }
                    if (!priceResults.Any()) return false;
                    var minResult = priceResults.OrderBy(r => r.Item1).First();
                    isFixed = minResult.Item2.PriceList.Count() <= 1;
                    resultPrice = minResult.Item1;
                    return true;
                }
                case Postal postal:
                {
                    var addressPrices = postal.ShippingAddresses.Where(sa => !sa.IsProviderWeightConstrain || sa.MaxWeight >= itemsWeight).Select(sa =>
                        postal.ShippingAddressPrices.FirstOrDefault(ap => ap.Id == sa.PriceId));
                    var priceResults = new List<(decimal, ShippingAddressPrice)>();
                    foreach (var addressPrice in addressPrices)
                    {
                        if (!TryCalculatePrice(addressPrice?.PriceList, items, out var apResult))
                            continue;
                        priceResults.Add((apResult, addressPrice));
                    }
                    if (!priceResults.Any()) return false;
                    var minResult = priceResults.OrderBy(r => r.Item1).First();
                    isFixed = minResult.Item2.PriceList.Count() <= 1;
                    resultPrice = minResult.Item1;
                    return true;
                }
                default:
                    return false;
            }
        }

        public bool TryCalculatePriceForShipping(Shipping shipping, IReadOnlyCollection<IShippable> items,
            out decimal resultPrice) => TryCalculatePriceForShipping(shipping, items, out resultPrice, out _);

        public bool TryCalculatePriceForShippingAddress(int photolabId, ShippingAddress address,
            IReadOnlyCollection<IShippable> items,
            out decimal resultPrice)
        {
            resultPrice = 0m;
            if (address is null) return false;

            var itemsWeight = items?.Sum(i => i.TotalWeight) ?? 0.0;

            if (address.IsProviderWeightConstrain && address.MaxWeight <= itemsWeight)
                return false;

            var price = GetById(address.PriceId, photolabId);
            return TryCalculatePrice(price.PriceList, items, out resultPrice);
        }

        private ShippingAddressPrice GetById(int? priceListId, int photolabId)
        {
            var key = $"address price id: {priceListId}";
            return _cacheService.GetFromCache(
                () => _shippingAddressPricesRepository.GetById(priceListId) ??
                      new ShippingAddressPrice(null, photolabId, new ShippingPrices()), key);
        }

        private bool TryCalculatePrice(ShippingPrices priceList, IReadOnlyCollection<IShippable> items,
            out decimal resultPrice)
        {
            resultPrice = 0m;

            if (priceList == null)
                return false;

            var totalWeight = items?.Where(i => i != null).Sum(i => i.TotalWeight) ?? 0;

            if (totalWeight > priceList.MaximumWeight && priceList.IsAvailableWeightConstrain)
                return false;

            if (priceList.IsFree)
                return true;

            var calculatedPrices = new List<decimal>();
            foreach (var price in priceList)
                if (price.TryGetPrice(totalWeight, out var calculatedPrice))
                    calculatedPrices.Add(calculatedPrice);

            var totalPrice = calculatedPrices.Count == 0
                ? (decimal)priceList.Last().Price
                : calculatedPrices.Min();

            resultPrice = Math.Round(totalPrice, 2);
            return true;
        }

        public decimal GetMinimumPrice(PhotolabSmall photolab, int? priceId)
        {
            var shippingAddressPrice = GetById(photolab, priceId);

            return shippingAddressPrice.PriceList.Prices.Count > 0
                ? Convert.ToDecimal(shippingAddressPrice.PriceList.Min(p => p.Price))
                : 0m;
        }

        public void UpdatePrice(int? priceId, Shipping shipping, IReadOnlyCollection<int> shippingAddressesId)
        {
            _shippingAddressPricesRepository.UpdateShippingAddressesByIds(priceId, shipping.Id, shippingAddressesId);
            _cacheService.Invalidate(shipping);
        }

        public bool TryCalculatePriceForOrderAddress(int photolabId, OrderAddress address,
            IReadOnlyCollection<IShippable> items,
            out decimal resultPrice)
        {
            resultPrice = 0m;
            if (address.ShippingAddressId is null) return false; //метод используется только для доставок с адресами в базе
            var shippingAddress = _shippingAddressService.GetById((int)address.ShippingAddressId);
            return !(shippingAddress is null) && TryCalculatePriceForShippingAddress(photolabId, shippingAddress, items, out resultPrice);
        }
    }
}
