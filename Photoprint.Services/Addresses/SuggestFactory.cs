#pragma warning disable CS0162

using System;
using Photoprint.Core.Infrastructure.Cache;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photoprint.Core;
using Photoprint.Services.Addresses;
using Photoprint.Core.Models.MobileAppSettings;

namespace Photoprint.Services
{
    public class SuggestFactory: ISuggestFactory
    {
        private readonly ISuggestProvider _yandex;
        private readonly ISuggestProvider _google;
        private readonly PixlparkCache _cache;
        private readonly IPhotolabSettingsService _photolabSettingsService;

        public SuggestFactory(IPhotolabSettingsService photolabSettingsService)
        {
            _yandex = new YandexSuggestProvider();
            _google = new GoogleSuggestProvider();
            _cache = PixlparkCacheManager.SimpleCache.CreateOrGet("suggests");
            _photolabSettingsService = photolabSettingsService;
        }

        public bool TryGetProviderAndKey(out ISuggestProvider provider, out string apiKey, Photolab photolab = null, MobileAppMapsSettings mobileAppMapsSettings = null, bool isMobile = false)
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

        public IReadOnlyCollection<CitySuggest> GetCitySuggests(ILanguage language, [NotNull] CitySuggestData data,
            Photolab photolab = null, MobileAppMapsSettings mobileAppMapsSettings = null, bool isMobile = false,
            SuggestProviderType type = SuggestProviderType.Yandex)
        {
            if (!TryGetProviderAndKey(out var provider, out var apiKey, photolab, mobileAppMapsSettings, isMobile))
                throw new PhotoprintSystemException(AddressExceptionMessages.BadSettings, string.Empty);
            if (string.IsNullOrWhiteSpace(data.Query)) return new List<CitySuggest>();
#if DEBUG
            return provider.GetCitySuggests(apiKey, data.Country, data.Type, data.Query, language);
#endif
            var key = $"{provider.cacheKey}:{(int)data.Country}:{data.Query}";
            return _cache.GetOrAdd(key, () => provider.GetCitySuggests(apiKey, data.Country, data.Type, data.Query, language));
        }

        public IReadOnlyCollection<AddressSuggest> GetAddressSuggests(
            ILanguage language,
            SuggestAddressData data, Photolab photolab = null, MobileAppMapsSettings mobileAppMapsSettings = null, bool isMobile = false,
            SuggestProviderType type = SuggestProviderType.Yandex)
        {
            if (!TryGetProviderAndKey(out var provider, out var apiKey, photolab, mobileAppMapsSettings, isMobile))
                throw new PhotoprintSystemException(AddressExceptionMessages.BadSettings, string.Empty);
            if (data is null) return new List<AddressSuggest>();
#if DEBUG
            return provider.GetAddressSuggest(apiKey, data.FullQuery, data.Street, data.City, data.Type, language);
#endif
            var key = $"{provider.cacheKey}:{data.FullQuery}";
            return _cache.GetOrAdd(key, () => provider.GetAddressSuggest(apiKey, data.FullQuery, data.Street, data.City, data.Type, language));
        }

        public CityAddressCountry GetCountry(string query)
        {
            var provider = _yandex;
            var key = $"{_yandex.cacheKey}:{query}";
            return _cache.GetOrAdd(key, () => _yandex.GetCountry(query)).County;
        }


    }
}
