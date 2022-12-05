#pragma warning disable CS0162

using JetBrains.Annotations;
using Photoprint.Core;
using Photoprint.Core.Models;
using Photoprint.Core.Repositories;
using Photoprint.Core.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace Photoprint.Services
{
    public class ShippingService : ServiceBase, IShippingService
    {
        private readonly IShippingRepository _shippingRepository;
        private readonly IShippingAddressService _shippingAddressService;
        private readonly IImageService _imageService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICacheService _cacheService;
        private readonly IPhotolabService _photolabService;
        private readonly IShippingPriceService _shippingPriceService;

        protected void ValidatePermissionsAndThrow(int currentPhotolabId)
        {
            var user = _authenticationService.LoggedInUser;
            var lab = _photolabService.GetById(currentPhotolabId);
            if (user is null || (!user.IsFrontendAdministrator(lab) && !user.IsManager(lab) && !user.IsAdministrator))
                throw new PhotoprintValidationException("You don't have access to shipping settings", string.Empty);
        }

        public ShippingService(IShippingRepository shippingRepository,
                               IShippingAddressService shippingAddressService,
                               IImageService imageService,
                               IAuthenticationService authenticationService,
                               ICacheService cacheService,
                               IPhotolabService photolabService,
                               IShippingPriceService shippingPriceService)
        {
            _shippingRepository = shippingRepository;
            _shippingAddressService = shippingAddressService;
            _imageService = imageService;
            _authenticationService = authenticationService;
            _cacheService = cacheService;
            _photolabService = photolabService;
            _shippingPriceService = shippingPriceService;
        }

        // CREATE
        public Postal Create(PostalInput input)
        {
            ValidatePermissionsAndThrow(input.PhotolabId);
            var postal = new Postal
            {
                IsEnabled = input.IsEnabled,
                IsEnabledForMobileApp = input.IsEnabledForMobileApp,
                IsShippingPricePaidSeparately = input.IsShippingPricePaidSeparately,
                PhotolabId = input.PhotolabId,
                AdminTitle = input.AdminTitle,
                IsRegionRequired = input.IsRegionRequired,
                IsIndexRequired = input.IsIndexRequired,
                IsMultipleAddressLines = input.IsMultipleAddressLines,
                PostalType = input.PostalType,
                Phone = input.Phone,
                Email = input.Email
            };
            postal = (Postal)_shippingRepository.Create(postal);

            _cacheService.Invalidate(new Photolab { Id = postal.PhotolabId }.GetCacheTag());

            return postal;
        }

        public DistributionPoint Create(DistributionPointInput input)
        {
            ValidatePermissionsAndThrow(input.PhotolabId);
            var point = new DistributionPoint
            {
                IsEnabled = input.IsEnabled,
                IsEnabledForMobileApp = input.IsEnabledForMobileApp,
                IsShippingPricePaidSeparately = input.IsShippingPricePaidSeparately,
                PhotolabId = input.PhotolabId,
                AdminTitle = input.AdminTitle,
                Phone = input.Phone,
                Email = input.Email,
                OfficeHours = input.OfficeHours,
                SitePageUrl = input.SitePageUrl
            };
            point = (DistributionPoint)_shippingRepository.Create(point);
            var shippingAddress = new ShippingAddress(input.Address)
            {
                ShippingId = point.Id,
                PriceId = _shippingPriceService.GetOrCreateShippingAddressPrice(_photolabService.GetById(input.PhotolabId), input.ShippingPrices).Id
            };

            _shippingAddressService.Create(shippingAddress, point);

            point = GetById<DistributionPoint>(point.Id);
            _cacheService.Invalidate(new Photolab { Id = point.PhotolabId }.GetCacheTag());

            return point;
        }

        public Courier Create(CourierInput input)
        {
            ValidatePermissionsAndThrow(input.PhotolabId);
            var courier = new Courier
            {
                IsShippingPricePaidSeparately = input.IsShippingPricePaidSeparately,
                IsEnabled = input.IsEnabled,
                IsEnabledForMobileApp = input.IsEnabledForMobileApp,
                PhotolabId = input.PhotolabId,
                AdminTitle = input.AdminTitle,
                IsRegionRequired = input.IsRegionRequired,
                IsIndexRequired = input.IsIndexRequired,
                Email = input.Email,
                Phone = input.Phone
            };
            courier = (Courier)_shippingRepository.Create(courier);

            courier = GetById<Courier>(courier.Id);

            _cacheService.Invalidate(new Photolab { Id = courier.PhotolabId }.GetCacheTag());

            return courier;
        }

        // GET
        private string GetCacheKeyForGetById(int shippingId) => $"shipping_by_id_{shippingId}";

        private Shipping GetById(int shippingId)
        {
            if (shippingId == 0) return null;

            var key = GetCacheKeyForGetById(shippingId);
            return _cacheService.GetFromCache(() => _shippingRepository.GetById(shippingId), key, CacheTime.Long);
        }

        public TK GetById<TK>(int shippingId) where TK : Shipping
        {
            return GetById(shippingId) as TK;
        }

        public IReadOnlyCollection<Shipping> GetList(CustomWorkItem item, int photolabId)
        {
            if (item is null) return null;
            return _shippingRepository.GetList(item.Id, photolabId);
        }

        public IReadOnlyCollection<Shipping> GetList(int photolabId)
        {
            if (photolabId == 0) return Array.Empty<Shipping>();
            return _shippingRepository.GetList(photolabId);
        }
        public IReadOnlyCollection<ShippingSmall> GetListWithPriceRestrictions(int photolabId, decimal? totalPrice)
        {
            var shipping = totalPrice != null ? 
                GetList(photolabId).Where(s => (!s.MinimumPrice.HasValue || s.MinimumPrice < totalPrice) && (!s.MaximumPrice.HasValue || totalPrice < s.MaximumPrice)) :
                GetList(photolabId);
            var result = shipping.OrderBy(s=>s.Position).Select(s => new ShippingSmall
            {
                Id = s.Id,
                Type = s.Type,
                AdminTitle = s.AdminTitle
            }).ToList();
            return result.AsReadOnly();
        }

        public IReadOnlyCollection<TK> GetListByServiceProvider<TK>(Photolab photolab, ShippingServiceProviderType serviceProvider) where TK : Shipping
        {
            _ = photolab ?? throw new ArgumentNullException(nameof(photolab));

            var shippingIds = _shippingRepository.GetIdsListByServiceProvider(photolab.Id, GetShippingType(typeof(TK)), serviceProvider);

            return GetListByIds<TK>(shippingIds).Where(s => s != null).ToList();
        }

        public int GetShippingsCountByPhotolab(IEnumerable<Photolab> photolabs)
        {
            if (photolabs?.Any() != true) return 0;

            var ids = photolabs.Select(x => x.Id);
            var key = "total-shippings-count" + string.Join(",", ids);
            return _cacheService.GetFromCache(() => _shippingRepository.GetShippingsCountByPhotolab(ids), key, CacheTime.Default, photolabs.ToArray());
        }

        private static readonly string _shippingWithUsersCacheKey = "shippingsWithUsers";
        public IReadOnlyCollection<ShippingSmall> GetSmallList([NotNull] Photolab photolab, bool hideDisabled, bool forMobile = false)
        {
            _ = photolab ?? throw new ArgumentNullException(nameof(photolab));

            var key = string.Concat("ShippingSmall pid:", photolab.Id, "(", hideDisabled, ",", forMobile, ")");
            return _cacheService.GetFromCache(() => _shippingRepository.GetSmallList(photolab.Id, hideDisabled, forMobile), key, CacheTime.Default, photolab);
        }
        public IReadOnlyCollection<ShippingSmall> GetSmallListByUser(User user, Photolab photolab, bool hideDisabled, out int totalCount)
        {
            var smallShippings = GetSmallList(photolab, hideDisabled);
            totalCount = smallShippings.Count;
            if (user.IsAdministrator || user.IsFrontendAdministrator(photolab)) // Админам доступны все доставки, а для остальных фильтруем
            {
                return smallShippings.ToList();
            }

            var shippingWithUsers = _cacheService.GetFromCache(() => _shippingRepository.GetUsersInShippings(), _shippingWithUsersCacheKey);
            var shippingIds = new List<int>(shippingWithUsers.Count);
            foreach (var shippingWithUsersDto in shippingWithUsers)
            {
                if (shippingWithUsersDto.UserIds?.Contains(user.Id) == true)
                {
                    shippingIds.Add(shippingWithUsersDto.ShippingId);
                }
            }

            var result = shippingIds.Select(shippingId => smallShippings.FirstOrDefault(s => s.Id == shippingId)).Where(data => data != null).ToList();
            if (result.Count == 0 && (user.IsOperator(photolab) || user.IsManager(photolab) || user.IsAccountant(photolab)))
            {
                return smallShippings.ToList();
            }

            return result;
        }

        public IReadOnlyCollection<ShippingSmall> GetSmallListByIds(Photolab photolab, IEnumerable<int> shippingsIds)
        {
            _ = photolab ?? throw new ArgumentNullException(nameof(photolab));
            if (shippingsIds is null) throw new ArgumentNullException(nameof(shippingsIds));

            var allShippings = GetSmallList(photolab, false);
            return allShippings.Where(s => shippingsIds.Contains(s.Id)).ToList();
        }

        // Получаем коллекцию SmallShipping по CustomWorkItem, где есть связь в таблице PR_CustomWorkItemsInShippings
        public IReadOnlyCollection<ShippingSmall> GetSmallListByCustomWorkItem(CustomWorkItem item, Photolab photolab)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            if (photolab is null) throw new ArgumentNullException(nameof(photolab));

            return _shippingRepository.GetSmallListByCustomWorkItem(item, photolab);
        }
        // Получаем коллекцию ShippingSmallInCustomWorkItem по CustomWorkItem, где есть связь в таблице PR_CustomWorkItemsInShippings
        public IReadOnlyCollection<ShippingSmallInCustomWorkItem> GetSmallListByCustomWorkItemWithActivity(CustomWorkItem item, Photolab photolab)
        {
            if (item is null || photolab is null) return null;

            return _shippingRepository.GetConstrainedSmallListByCustomWorkItemWithActivity(item, photolab);
        }
        // Получаем коллекцию ShippingSmallInCustomWorkItem, которые есть для сайта, но в них указываем, активны ли они для 
        // данного CustomWorkItem или нет (проверка на строки в PR_CustomWorkItemsDisabledInShippings)
        public IReadOnlyCollection<ShippingSmallInCustomWorkItem> GetAllSmallListByCustomWorkItemWithActivity(CustomWorkItem item, Photolab photolab)
        {
            if (item is null || photolab is null) return null;

            return _shippingRepository.GetSmallListByCustomWorkItemWithActivity(item, photolab);
        }

        private ShippingType GetShippingType(Type TKtype)
        {
            var type = ShippingType.Unknown;

            if (typeof(Courier).IsAssignableFrom(TKtype))
                type = ShippingType.Courier;
            else if (typeof(DistributionPoint).IsAssignableFrom(TKtype))
                type = ShippingType.Point;
            else if (typeof(Postal).IsAssignableFrom(TKtype))
                type = ShippingType.Postal;

            return type;
        }

        public IReadOnlyCollection<TK> GetList<TK>(Photolab photolab) where TK : Shipping
        {
            var type = GetShippingType(typeof(TK));
            return (type != ShippingType.Unknown
                ? _shippingRepository.GetList(photolab.Id, type, false).Cast<TK>()
                : _shippingRepository.GetList(photolab.Id, null, false).Cast<TK>()).AsList();
        }
        public IReadOnlyCollection<TK> GetList<TK>(Photolab photolab, Payment payment) where TK : Shipping
        {
            var type = GetShippingType(typeof(TK));
            var paymentId = payment?.Id ?? 0;
            return (type != ShippingType.Unknown
                ? _shippingRepository.GetListByPayment(photolab.Id, type, paymentId, false).Cast<TK>()
                : _shippingRepository.GetListByPayment(photolab.Id, paymentId, false).Cast<TK>()).AsList();
        }
        public IReadOnlyCollection<TK> GetList<TK>(int photolabId) where TK : Shipping
        {
            var type = GetShippingType(typeof(TK));
            return (type != ShippingType.Unknown
                ? _shippingRepository.GetList(photolabId, type, false).Cast<TK>()
                : _shippingRepository.GetList(photolabId, null, false).Cast<TK>()).AsList();
        }

        public IReadOnlyCollection<Postal> GetListByProviderType(ShippingServiceProviderType providerType)
        {
            return _shippingRepository.GetListByProviderType(providerType).Cast<Postal>().ToList();
        }
        public IReadOnlyCollection<TK> GetListByUser<TK>(User user, int photolabId, bool withAddress) where TK : Shipping
        {
            if (user is null) throw new ArgumentNullException(nameof(user));

            return _shippingRepository.GetListByUser(user.Id, photolabId, withAddress).Cast<TK>().AsList();
        }
        public IReadOnlyCollection<TK> GetAvailableList<TK>(Photolab photolab) where TK : Shipping
        {
            var type = GetShippingType(typeof(TK));
            var key = $"shippings available (pid:{photolab.Id} type:{type})";
            return (type != ShippingType.Unknown
                ? _cacheService.GetFromCache(() => _shippingRepository.GetList(photolab.Id, type, false).Cast<TK>(), key, CacheTime.Long, photolab.GetCacheTag())
                : _cacheService.GetFromCache(() => _shippingRepository.GetList(photolab.Id, null, false).Cast<TK>(), key, CacheTime.Long, photolab.GetCacheTag())).AsList();
        }
        public IReadOnlyCollection<TK> GetAvailableList<TK>(Photolab photolab, IEnumerable<ShoppingCartItem> items, bool forMobileApp = false) where TK : Shipping
        {
            _ = photolab ?? throw new ArgumentNullException(nameof(photolab));
            if (items is null) throw new ArgumentNullException(nameof(items));

            var availableList = GetAvailableList<TK>(photolab).OrderBy(s => s.Position).ToList();
            if (items.Any())
            {
                var price = items.Sum(i => i.Price);
                availableList = availableList.Where(s => (!s.MinimumPrice.HasValue || s.MinimumPrice < price) && (!s.MaximumPrice.HasValue || price < s.MaximumPrice)).ToList();
            }

            var availableForItems = GetAvailableShippingsIdsByCartItems(items, photolab);

            return forMobileApp
                ? availableList.Where(shipping => shipping.IsEnabledForMobileApp && availableForItems.Contains(shipping.Id)).AsList()
                : availableList.Where(shipping => shipping.IsEnabled && availableForItems.Contains(shipping.Id)).AsList();
        }

        public IReadOnlyCollection<int> GetAvailableShippingsIdsByCartItems(IEnumerable<ShoppingCartItem> items, Photolab photolab)
        {
            _ = photolab ?? throw new ArgumentNullException(nameof(photolab));
            var shoppingCartItems = items as IList<ShoppingCartItem> ?? items.ToList();

            int capacity = shoppingCartItems.Count;

            var materialsIds = new List<int>(capacity);
            var productsIds = new List<int>(capacity);
            var gfProductsIds = new List<int>(capacity);
            var customWorkItemsIds = new List<int>(capacity);
            var tags = new List<string>(capacity);

            foreach (var item in shoppingCartItems)
            {
                tags.Add(item.GetCacheTag());

                if (item is MaterialShoppingCartItem msItem && !materialsIds.Contains(msItem.MaterialId))
                {
                    materialsIds.Add(msItem.MaterialId);
                    customWorkItemsIds.AddRange(
                        msItem.Properties.AdditionalPriceItems
                            .Where(pi => !customWorkItemsIds
                                .Contains(pi.ItemId)).Select(a => a.ItemId));
                    continue;
                }
                if (item is ProductShoppingCartItem psItem && !productsIds.Contains(psItem.ProductId))
                {
                    productsIds.Add(psItem.ProductId);
                    continue;
                }
                if (item is GFShoppingCartItem gfItem && !gfProductsIds.Contains(gfItem.GFProductId))
                {
                    gfProductsIds.Add(gfItem.GFProductId);
                }
            }

            var key = $"available-shipings-for (phid:{photolab.Id} materialsIds:{string.Join(",", materialsIds)} "
                + $"gfproductsIds:{string.Join(",", gfProductsIds)} "
                + $"productsIds:{string.Join(",", productsIds)} customWorkItemsIds:{string.Join(",", customWorkItemsIds)})";

            tags.AddRange(new[] { photolab.GetCacheTag() });

#if DEBUG
            var availableShippingsByItemsDebug = _shippingRepository.GetIdsListByCartItems(materialsIds, productsIds, gfProductsIds, photolab.Id);
            var availableShippingsByCustomWorkItemsDebug = _shippingRepository.GetIdsListByCustomWorksItems(customWorkItemsIds, photolab.Id);
            return availableShippingsByItemsDebug.Intersect(availableShippingsByCustomWorkItemsDebug).ToArray();
#endif

            return _cacheService.GetFromCache(() =>
            {
                var availableShippingsByItems = _shippingRepository.GetIdsListByCartItems(materialsIds, productsIds, gfProductsIds, photolab.Id);
                var availableShippingsByCustomWorkItems = _shippingRepository.GetIdsListByCustomWorksItems(customWorkItemsIds, photolab.Id);
                return availableShippingsByItems.Intersect(availableShippingsByCustomWorkItems).ToArray();
            }, 
            key, CacheTime.Default, tags.ToArray());
        }

        public IEnumerable<TK> GetListByIds<TK>(IReadOnlyCollection<int> shippingIds) where TK : Shipping
        {
            var shippings = GetListByIds(shippingIds);
            foreach (var shipping in shippings)
            {
                yield return shipping as TK;
            }
        }
        public IReadOnlyCollection<Shipping> GetListByIds(IReadOnlyCollection<int> shippingsIds)
        {
            if (shippingsIds.Count == 0) return Array.Empty<Shipping>();

            var param = new EntityListByIdsParams<Shipping>
            {
                CacheService = _cacheService,
                GetCacheKey = GetCacheKeyForGetById,
                GetEntityById = GetById,
                GetListFromRepository = idsList => _shippingRepository.GetListByIds(idsList),
            };
            return GetListByIds(shippingsIds, param);
        }

        private static string GetShippingInDiscountsKey(int photolabId) => "shippings in discounts pid:" + photolabId;
        public IReadOnlyCollection<int> GetRestrictedIdsListByDiscount(Discount discount)
        {
            var key = GetShippingInDiscountsKey(discount.PhotolabId);
            var all = _cacheService.GetFromCache(() => _shippingRepository.GetRestrictionsListByDiscount(discount.PhotolabId), key, CacheTime.VeryLong, Photolab.GetCacheTag(discount.PhotolabId));
            return all.ContainsKey(discount.Id) ? all[discount.Id] : Array.Empty<int>();
        }

        private string GetUserBindingsCacheKey(int companyAccountId) => "shippingUserBindings cid: " + companyAccountId;
        public ShippingsToUsersBindingDTO GetUserRestrictionsAll(CompanyAccount account)
        {
            var key = GetUserBindingsCacheKey(account.Id);
            return _cacheService.GetFromCache(() => _shippingRepository.GetUserRestrictionsAll(account.Id), key, CacheTime.Default, account);
        }

        public UserShippingBindings GetShippingBindings([NotNull] CompanyAccount account, [NotNull] User user)
        {
            if (account is null) throw new ArgumentNullException(nameof(account));
            if (user is null) throw new ArgumentNullException(nameof(user));

            var frontends = _photolabService.GetAvailableListSmall(account, user);
            if (frontends.Count == 0)
            {
                return new UserShippingBindings(user.Id, null);
            }

            var allRestrictions = GetUserRestrictionsAll(account);

            var prepared = new Dictionary<int, UserShippingBindings.ShippingBinding>(frontends.Count);
            foreach (var photolab in frontends)
            {
                if (user.IsOperator(photolab) || user.IsManager(photolab))
                {
                    var shippingRestrictions = allRestrictions.GetShippingIdsByUser(user, photolab);
                    if (shippingRestrictions != null)
                    {
                        prepared.Add(photolab.Id, new UserShippingBindings.ShippingBinding(shippingRestrictions));
                    }
                    else
                    {
                        prepared.Add(photolab.Id, new UserShippingBindings.ShippingBinding(Array.Empty<int>()));
                    }
                }
                else
                {
                    prepared.Add(photolab.Id, null);
                }
            }
            return new UserShippingBindings(user.Id, prepared);
        }

        // UPDATE
        public void Update(Shipping shipping)
        {
            if (shipping is null) throw new ArgumentNullException(nameof(shipping));
            ValidatePermissionsAndThrow(shipping.PhotolabId);

            switch (shipping)
            {
                case DistributionPoint point:
                    var pointFromDB = GetById<DistributionPoint>(point.Id);

                    if (point.Properties.IsContractorAddressNotEqual && point.ContractorAddress != null)
                    {
                        if (point.ContractorAddress.Id == 0)
                        {
                            point.ContractorAddress.ShippingId = point.Id;
                            point.ContractorAddress = _shippingAddressService.Create(point.ContractorAddress, point);
                        }
                        else
                        {
                            _shippingAddressService.Update(point.ContractorAddress, point);
                        }
                    }

                    point.Address.PriceId = _shippingPriceService.GetOrCreateShippingAddressPrice(_photolabService.GetById(shipping.PhotolabId), point.PriceList).Id;
                    _shippingAddressService.Update(point.Address, point);

                    if (pointFromDB.Address.AddressLine1 != point.Address.AddressLine1)
                    {
                        WriteLogAddressChanged(pointFromDB, point, shipping.PhotolabId);
                    }
                    break;

                case Courier courier:

                    if (courier.Address.Id == 0)
                    {
                        courier.Address.ShippingId = courier.Id;
                        _shippingAddressService.Create(courier.Address, courier);
                    }
                    else
                    {
                        _shippingAddressService.Update(courier.Address, courier);
                    }
                    break;

                case Postal postal:
                    if (postal.Properties.IsContractorAddressNotEqual && postal.ContractorAddress != null)
                    {
                        if (postal.ContractorAddress.Id == 0)
                        {
                            postal.ContractorAddress.ShippingId = postal.Id;
                            _shippingAddressService.Create(postal.ContractorAddress, postal);
                        }
                        else
                        {
                            _shippingAddressService.Update(postal.ContractorAddress, postal);
                        }
                    }
                    break;
            }

            _shippingRepository.Update(shipping);

            _cacheService.Invalidate(new Photolab { Id = shipping.PhotolabId }, shipping).WithRemote();
            _cacheService.RemoveFromCache(GetCacheKeyForGetById(shipping.Id)).WithRemote();
        }
        private void WriteLogAddressChanged(DistributionPoint pointOld, DistributionPoint pointNew, int photolabId)
        {
            var user = _authenticationService.LoggedInUser;
            Logger.Info($"Address has been changed. PointId: {pointNew.Id}, PhotolabId: {photolabId}, UserId: {user?.Id}, " +
                        $"old: {pointOld.Address.AddressLine1} => new: {pointNew.Address.AddressLine1}");
        }

        public void UpdatePaymentBinding(Photolab photolab, Payment payment, IEnumerable<Shipping> shippings)
        {
            _ = photolab ?? throw new ArgumentNullException(nameof(photolab));
            if (payment is null) throw new ArgumentNullException(nameof(payment));

            var currentList = GetList<Shipping>(photolab, payment);
            var forInsert = shippings.Where(p => !currentList.Contains(p));
            var forDelete = currentList.Where(p => !shippings.Contains(p));

            foreach (var shipping in forInsert)
            {
                _shippingRepository.BindShippingToPayment(shipping.Id, payment.Id);
            }
            foreach (var shipping in forDelete)
            {
                _shippingRepository.UnbindShippingFromPayment(shipping.Id, payment.Id);
            }

            _cacheService.Invalidate(photolab.GetCacheTag());
        }

        public void UpdatePositions(Dictionary<Shipping, int> positions)
        {
            foreach (var position in positions)
            {
                position.Key.Position = position.Value;
                Update(position.Key);
            }
        }

        public void UpdateShippingBindings(Photolab photolab, CustomWorkItem item, IReadOnlyCollection<ShippingSmall> shippings)
        {
            _ = photolab ?? throw new ArgumentNullException(nameof(photolab));
            if (shippings is null) throw new ArgumentNullException(nameof(shippings));
            if (item is null) throw new ArgumentNullException(nameof(item));

            var shippingsIds = shippings.Select(x => x.Id);
            var currentBindedShippings = GetSmallListByCustomWorkItem(item, photolab).Select(x => x.Id);

            var forInsert = shippingsIds.Where(id => !currentBindedShippings.Contains(id)).AsList();
            var forDelete = currentBindedShippings.Where(id => !shippingsIds.Contains(id)).AsList();

            if (forInsert.Count != 0) _shippingRepository.BindShippingsToCustomWorkItem(forInsert, item);
            if (forDelete.Count != 0) _shippingRepository.UnbindShippingsFromCustomWorkItem(forDelete, item);

            //_cacheService.Invalidate(photolab.DefaultLanguage.GetCacheTag());
        }

        public void UpdateMap(DistributionPoint distributionPoint, MapInput input)
        {
            if (distributionPoint is null) throw new ArgumentNullException(nameof(distributionPoint));
            if (input is null) throw new ArgumentNullException(nameof(input));

            ValidatePermissionsAndThrow(distributionPoint.PhotolabId);

            if (input.MapType == MapType.Static && input.MapFile != null && input.MapFile.Length > 0)
            {
                var size = new Size(input.ThumbnailWidth, input.ThumbnailHeight);
                _imageService.SetElementCover(input.MapFile, distributionPoint.ImagePath, new Size(0, 0), false, true);
                _imageService.SetElementCover(input.MapFile, distributionPoint.ThumbnailImagePath, size, true, true);

                distributionPoint.Longitude = string.Empty;
                distributionPoint.Latitude = string.Empty;
                distributionPoint.MapZoom = string.Empty;
                distributionPoint.CustomMapCode = string.Empty;
                Update(distributionPoint);
            }
            else if (input.MapType == MapType.GMap)
            {
                if (System.IO.File.Exists(distributionPoint.ImagePath)) System.IO.File.Delete(distributionPoint.ImagePath);
                if (System.IO.File.Exists(distributionPoint.ThumbnailImagePath)) System.IO.File.Delete(distributionPoint.ThumbnailImagePath);

                distributionPoint.Longitude = input.Longitude;
                distributionPoint.Latitude = input.Latitude;
                distributionPoint.MapZoom = input.Zoom.ToString(CultureInfo.InvariantCulture);
                distributionPoint.StaticMapWidth = input.ThumbnailWidth.ToString(CultureInfo.InvariantCulture);
                distributionPoint.StaticMapHeight = input.ThumbnailHeight.ToString(CultureInfo.InvariantCulture);
                distributionPoint.CustomMapCode = string.Empty;
                Update(distributionPoint);
            }
            else if (input.MapType == MapType.Custom)
            {
                if (System.IO.File.Exists(distributionPoint.ImagePath)) System.IO.File.Delete(distributionPoint.ImagePath);
                if (System.IO.File.Exists(distributionPoint.ThumbnailImagePath)) System.IO.File.Delete(distributionPoint.ThumbnailImagePath);

                distributionPoint.CustomMapCode = input.CustomMapCode;
                distributionPoint.Longitude = string.Empty;
                distributionPoint.Latitude = string.Empty;
                distributionPoint.MapZoom = string.Empty;
                Update(distributionPoint);
            }
            else if (input.MapType == MapType.None)
            {
                if (System.IO.File.Exists(distributionPoint.ImagePath)) System.IO.File.Delete(distributionPoint.ImagePath);
                if (System.IO.File.Exists(distributionPoint.ThumbnailImagePath)) System.IO.File.Delete(distributionPoint.ThumbnailImagePath);

                distributionPoint.Longitude = string.Empty;
                distributionPoint.Latitude = string.Empty;
                distributionPoint.MapZoom = string.Empty;
                distributionPoint.CustomMapCode = string.Empty;
                Update(distributionPoint);
            }

            _cacheService.Invalidate(new Photolab { Id = distributionPoint.PhotolabId }.GetCacheTag());
        }

        public void UpdateShippingsBinding(Discount discount, IEnumerable<int> shippingsId)
        {
            _shippingRepository.UpdateDiscountBindings(discount.Id, shippingsId);
            _cacheService.RemoveFromCache(GetShippingInDiscountsKey(discount.PhotolabId));
        }

        public int DuplicateShipping(Shipping sourceShipping, Photolab target)
        {
            if (sourceShipping is null) throw new ArgumentNullException(nameof(sourceShipping));
            if (target is null) throw new ArgumentNullException(nameof(target));

            var duplicatedShippingId = 0;
            switch (sourceShipping)
            {
                case Courier courier:
                    {
                        var input = new CourierInput(courier) { PhotolabId = target.Id };
                        var newCourier = Create(input);
                        duplicatedShippingId = newCourier.Id;
                        newCourier.Properties = courier.Properties;
                        Update(newCourier);
                        break;
                    }
                case DistributionPoint point:
                    {
                        var input = new DistributionPointInput(point) { PhotolabId = target.Id, Address = sourceShipping.Address };
                        var newPoint = Create(input);
                        duplicatedShippingId = newPoint.Id;
                        newPoint.Properties = point.Properties;
                        Update(newPoint);
                        break;
                    }
                case Postal postal:
                    {
                        var input = new PostalInput(postal) { PhotolabId = target.Id };
                        var newPostal = Create(input);
                        duplicatedShippingId = newPostal.Id;
                        newPostal.Properties = postal.Properties;
                        Update(newPostal);
                        _shippingAddressService.Copy(postal, newPostal, target);
                        break;
                    }
            }

            return duplicatedShippingId;
        }

        public void ActivateBindedToCustomWorkItemShipping(int itemId, int shippingId, Photolab photolab)
        {
            _shippingRepository.ActivateBindingToCustomWortItemShipping(itemId, shippingId);
            _cacheService.Invalidate(photolab.DefaultLanguage.GetCacheTag());
        }

        public void DeactivateBindedToCustomWorkItemShipping(int itemId, int shippingId, Photolab photolab)
        {
            _shippingRepository.DeactivateBindingToCustomWortItemShipping(itemId, shippingId);
            _cacheService.Invalidate(photolab.DefaultLanguage.GetCacheTag());
        }

        public void AddOperator(CompanyAccount account, User user, IEnumerable<int> shippingIds)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));
            if (shippingIds is null || !shippingIds.Any()) return;
            _shippingRepository.AddOperator(user.Id, shippingIds);

            foreach (var id in shippingIds)
            {
                var shipping = GetById<Shipping>(id);
                _cacheService.Invalidate(user, shipping);
            }

            _cacheService.RemoveFromCache(GetUserBindingsCacheKey(account.Id)).WithRemote();
        }

        public void RemoveOperator(CompanyAccount account, User user, IEnumerable<int> shippingIds)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));
            if (shippingIds is null || !shippingIds.Any()) return;

            _shippingRepository.RemoveOperator(user.Id, shippingIds);
            foreach (var id in shippingIds)
            {
                var shipping = GetById<Shipping>(id);
                _cacheService.Invalidate(user, shipping);
            }

            _cacheService.RemoveFromCache(GetUserBindingsCacheKey(account.Id)).WithRemote();
        }

        // DELETE
        public void Delete(int shippingId)
        {
            var shipping = GetById<Shipping>(shippingId);
            if (shipping is null) throw new ArgumentException("Shipping not found");

            ValidatePermissionsAndThrow(shipping.PhotolabId);

            WriteToLoggerForDelete(nameof(Shipping), shipping.Id, shipping.AdminTitle, _authenticationService.LoggedInUser, shipping.PhotolabId);
            _shippingRepository.Delete(shippingId);

            _cacheService.Invalidate(new Photolab { Id = shipping.PhotolabId }.GetCacheTag());
        }

        public bool TrySyncAddressRepository(Postal postal, IReadOnlyCollection<ShippingAddress> forDelete,
            IReadOnlyCollection<ShippingAddress> forUpdate, IReadOnlyCollection<ShippingAddress> forAdd, out IReadOnlyCollection<ShippingAddress> added)
        {
            added = new List<ShippingAddress>(forAdd.Count);
            if (forDelete.Count > 0)
            {
                try
                {
                    _shippingAddressService.DeleteList(forDelete, postal);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    return false;
                }
            }

            if (forUpdate.Count > 0)
            {
                try
                {
                    _shippingAddressService.UpdateList(forUpdate, postal);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    return false;
                }
            }

            if (forAdd.Count > 0)
            {
                try
                {
                    var defaultPriceId = postal.ServiceProviderSettings is IPriceCalculationProviderSettings settings ? settings.DefaultShippingPriceId : null;
                    forAdd = forAdd.Select(address =>
                    {
                        address.ShippingId = postal.Id;
                        address.PriceId = address.PriceId ?? defaultPriceId;
                        return address;
                    }).ToArray();
                    added = _shippingAddressService.CreateList(forAdd, postal);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    return false;
                }
            }
            return true;
        }

        public IReadOnlyCollection<Shipping> GetShippingUsages(ShippingServiceProviderType? type = null)
        {
            return _shippingRepository.GetShippingUsages(type);
        }
    }
}