#pragma warning disable CS0162

using Photoprint.Core;
using Photoprint.Core.Infrastructure.Cache;
using Photoprint.Core.Models;
using Photoprint.Core.Models.MobileAppSettings;
using Photoprint.Core.Services;
using Photoprint.Services.Addresses;
using Photoprint.Services.Addresses.Google;
using Photoprint.Services.Addresses.Yandex;


namespace Photoprint.Services
{
    public class AddressFactory : IAddressFactory
	{
        private readonly IPhotolabSettingsService _photolabSettingsService;

        private readonly IDeliveryStreetAddressProvider _yandex;
        private readonly IDeliveryStreetAddressProvider _google;
		private readonly PixlparkCache _cache;

        

		public AddressFactory(IPhotolabSettingsService photolabSettingsService)
		{
            _photolabSettingsService = photolabSettingsService;

            _yandex = new YandexStreetProvider();
            _google = new GoogleStreetProvider();
			_cache = PixlparkCacheManager.SimpleCache.CreateOrGet("address");
		}


        public bool TryGetProviderAndKey(out IDeliveryStreetAddressProvider provider, out string apiKey, Photolab photolab = null, MobileAppMapsSettings mobileAppMapsSettings = null, bool isMobile = false)
        {
            apiKey = null;
            provider = null;
            if (isMobile && mobileAppMapsSettings != null)
            {
                switch (mobileAppMapsSettings.MapType)
                {
                    case DeliveryMapType.Google:
                        apiKey = mobileAppMapsSettings.Google?.ApiKey;
                        provider = _google;
                        return !string.IsNullOrWhiteSpace(apiKey);
                    case DeliveryMapType.Yandex:
                        apiKey = mobileAppMapsSettings.Yandex?.ApiKey;
                        provider = _yandex;
                        return !string.IsNullOrWhiteSpace(apiKey);
                    default:
                        return false;
                }
            }
            else
            {
                if (!isMobile && photolab != null)
                {
                    var settings = _photolabSettingsService.GetSettings<DeliveryWindowSettings>(photolab, PhotolabSettingsType.DeliveryWindowSettings)?.MapSettings;
                    if (settings == null) return false;
                    switch (settings.MapType)
                    {
                        case DeliveryMapType.Google:
                            apiKey = settings.GoogleMapSettings?.ApiKey;
                            provider = _google;
                            return !string.IsNullOrWhiteSpace(apiKey);
                        case DeliveryMapType.Yandex:
                            apiKey = settings.YandexMapSettings?.ApiKey;
                            provider = _yandex;
                            return !string.IsNullOrWhiteSpace(apiKey);
                        default:
                            return false;
                    }
                }
                return false;
            }
        }

        public Address GetAddress(ILanguage language, AddressInfoData data, Photolab photolab = null, 
            MobileAppMapsSettings mobileAppMapsSettings = null, bool isMobile = false)
        {
            if (!TryGetProviderAndKey(out var provider, out var apiKey, photolab, mobileAppMapsSettings, isMobile)) 
                throw new PhotoprintSystemException(AddressExceptionMessages.BadSettings, string.Empty);

            if (!(data.Suggest is null))
            {
#if DEBUG
                return provider.GetAddressBySuggest(apiKey, data.Suggest, language);
#endif
                var key = $"{provider.CacheKey}:bySuggest:{ data.Suggest.SearchString}";
                return _cache.GetOrAdd(key, () => provider.GetAddressBySuggest(apiKey, data.Suggest, language));
            }

            if (data.Coords?.Ok ?? false)
            {
#if DEBUG
                return provider.GetAddressByCoords(apiKey, data.Coords.Latitude, data.Coords.Longitude, language);
#endif
                var key = $"{provider.CacheKey}:byCoords:{data.Coords.Latitude},{data.Coords.Longitude}";
                return _cache.GetOrAdd(key, () => provider.GetAddressByCoords(apiKey, data.Coords.Latitude, data.Coords.Longitude, language));
            }

            if (!string.IsNullOrWhiteSpace(data.FullQuery))
            {
#if DEBUG
                return provider.GetAddressByQuery(apiKey, data.FullQuery, data.City, language);
#endif
                var key = $"{provider.CacheKey}:byQuery:{data.FullQuery}";
                return _cache.GetOrAdd(key, () => provider.GetAddressByQuery(apiKey, data.FullQuery, data.City, language));
            }

            return null;
        }

        public Address AddressByCoordsFind(ILanguage language, string lat, string lng, Photolab photolab = null, 
            MobileAppMapsSettings mobileAppMapsSettings = null, bool isMobile = false)
		{
            if (!TryGetProviderAndKey(out var provider, out var apiKey, photolab, mobileAppMapsSettings, isMobile)) 
                throw new PhotoprintSystemException(AddressExceptionMessages.BadSettings, string.Empty);
            if (string.IsNullOrWhiteSpace(lat) || string.IsNullOrWhiteSpace(lat)) return null;
#if DEBUG
            return provider.GetAddressByCoords(apiKey, lat, lng, language);
#endif
			var key = $"{provider.CacheKey}:byCoords:{lat},{lng}";
            return _cache.GetOrAdd(key, () => provider.GetAddressByCoords(apiKey, lat, lng, language));
		}

    }
}
