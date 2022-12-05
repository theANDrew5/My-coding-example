#pragma warning disable CS0162
using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Spreadsheet;
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
	public class CityAddressFactory : ICityAddressFactory
	{
        private readonly IPhotolabSettingsService _photolabSettingsService;
        private readonly IDeliveryCityAddressProvider _yandex;
		private readonly IDeliveryCityAddressProvider _google;
		private readonly PixlparkCache _cache;

		public CityAddressFactory(IPhotolabSettingsService photolabSettingsService)
		{
            _photolabSettingsService = photolabSettingsService;
            _google = new GoogleCityProvider();
			_yandex = new YandexCityProvider();
			_cache = PixlparkCacheManager.SimpleCache.CreateOrGet("cities");
        }

        private bool TryGetProviderAndKey(out IDeliveryCityAddressProvider provider, out string apiKey, Photolab photolab = null, MobileAppMapsSettings mobileAppMapsSettings = null, bool isMobile = false)
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

        public IReadOnlyCollection<CityAddress> GetCityAddresses(ILanguage language, CityInfoData data, Photolab photolab = null, MobileAppMapsSettings mobileAppMapsSettings = null, bool isMobile = false)
        {
            if (!TryGetProviderAndKey(out var provider, out var apiKey, photolab, mobileAppMapsSettings, isMobile))
                throw new PhotoprintSystemException(AddressExceptionMessages.BadSettings, string.Empty);

            if (data is null) return new List<CityAddress>();

            if (!(data.Suggest is null))
            {
#if DEBUG
                return provider.GetCitiesBySuggest(apiKey, data.Country, data.Suggest, language);
#endif
                var key = $"{provider.CacheKey}:{(int)data.Country}:{data.Suggest.CacheKey}";
                return _cache.GetOrAdd(key, () => provider.GetCitiesBySuggest(apiKey, data.Country, data.Suggest, language));
            }

            if (data.Coords?.Ok ?? false)
            {
#if DEBUG
                return provider.GetCities(apiKey, data.Coords.Latitude, data.Coords.Longitude, language);
#endif
                var key = $"{provider.CacheKey}:{data.Coords.Latitude}:{data.Coords.Longitude}";
                return _cache.GetOrAdd(key, () => provider.GetCities(apiKey, data.Coords.Latitude, data.Coords.Longitude, language));
            }

            if (!string.IsNullOrWhiteSpace(data.Query))
            {
#if DEBUG
                return provider.GetCities(apiKey, data.Country, data.Query, language);
#endif
                var key = $"{provider.CacheKey}:{(int)data.Country}:{data.Query}";
                return  _cache.GetOrAdd(key, () => provider.GetCities(apiKey, data.Country, data.Query, language));
            }
            return Array.Empty<CityAddress>();
        }
    }
}