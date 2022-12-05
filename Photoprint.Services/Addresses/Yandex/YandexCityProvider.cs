using System;
using System.Collections.Generic;
using System.Globalization;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using Photoprint.Core.Models;
using System.Linq;
using Photoprint.Services.Infrastructure;


namespace Photoprint.Services.Addresses.Yandex
{


	public class YandexCityProvider : IDeliveryCityAddressProvider
	{
        public string ProviderName => "Yandex";
		public string CacheKey => "yandex";

		private readonly RateLimiter _rateLimiter = new RateLimiter(new CountByIntervalAwaitableConstraint(5, TimeSpan.FromSeconds(1)));
		private const int _countOfResults = 10;

        private const string _getCitiesEndpoint = "https://geocode-maps.yandex.ru/1.x/?apikey={1}&format=json&results={2}&lang={3}&geocode={0}";
        private bool GetBbox(string latitude, string longitude, out string bbox)
        {
            const double shift = 0.05;
            bbox = null;
            
            if (!double.TryParse(latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var lat)) return false;
            if (!double.TryParse(longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var lng)) return false;

            var latmax=lat+shift;
            var latmin=lat-shift;
            var lngmax=lng+shift;
            var lngmin=lng-shift;

            bbox = $"{lngmax.ToString("0.000000", CultureInfo.InvariantCulture)},{latmax.ToString("0.000000", CultureInfo.InvariantCulture)}~"+
                   $"{lngmin.ToString("0.000000", CultureInfo.InvariantCulture)},{latmin.ToString("0.000000", CultureInfo.InvariantCulture)}";

            return true;

        }
        private string GetUrlBySuggest(CitySuggest suggest, YandexCountry country, string apiKey, string lang,
            string latitude = null, string longitude = null)
        {
            var url = string.Format(_getCitiesEndpoint, suggest.Title, apiKey, _countOfResults, lang);

            if (country != YandexCountry.Everywhere)
            {

                url = string.Concat(url, "&in=" + (int)country);
            }
            if(suggest.Type==ToponymType.City && GetBbox(latitude, longitude, out var bbox))
            {

                url = string.Concat(url, "&rspn=1&bbox=" + bbox);

            }
            return url;		

        }
        public IReadOnlyList<CityAddress> GetCitiesBySuggest(string apiKey, CityAddressCountry country,
            CitySuggest suggest,
            ILanguage language)
		{
            var yandexCountry = DataTranslator.CountryConvert(country);
            var lang = DataTranslator.LanguageConvert(language);
            var url = GetUrlBySuggest(suggest, yandexCountry, apiKey, lang, suggest.Latitude, suggest.Longitude);

            var answer = string.Empty;

            void SendRequest()
            {
                var response = url.AllowAnyHttpStatus().GetAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                answer = response.Content.ReadAsStringAsync().Result;
            }

            _rateLimiter.Perform((Action)SendRequest).ConfigureAwait(false).GetAwaiter().GetResult();
			
            var jsonObj = JObject.Parse(answer);

            var data = (jsonObj["response"]?["GeoObjectCollection"]?["featureMember"] ?? new JObject()).Select(j => j["GeoObject"].ToObject<YandexGeoObject>()).ToList();
            return DataTranslator.GetCitiesFromGeoObj(suggest.Type, data);
		}
        
        private const string _getCitiesByCoordsEndpoint = "https://geocode-maps.yandex.ru/1.x/?apikey={1}&format=json&sco=latlong&kind=locality&results={2}&lang={3}&geocode={0}";
        private string GetUrlByCoords(string latitude, string longitude, string apiKey, ILanguage language)
        {
            var lang = DataTranslator.LanguageConvert(language);
            var latlong = $"{latitude},{longitude}";
            var url = string.Format(_getCitiesByCoordsEndpoint, latlong, apiKey, _countOfResults, lang);
            return url;
        }
        public IReadOnlyList<CityAddress> GetCities(string apiKey,
            string latitude, string longitude, ILanguage language)

        {

            var url = GetUrlByCoords(latitude, longitude, apiKey, language);

            var answer = string.Empty;

            void SendRequest()
            {
                var response = url.AllowAnyHttpStatus().GetAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                answer = response.Content.ReadAsStringAsync().Result;
            }

            _rateLimiter.Perform((Action)SendRequest).ConfigureAwait(false).GetAwaiter().GetResult();
			
            var jsonObj = JObject.Parse(answer);

            var data = (jsonObj["response"]?["GeoObjectCollection"]?["featureMember"] ?? new JObject()).Select(j => j["GeoObject"].ToObject<YandexGeoObject>()).ToList();
            return DataTranslator.GetCitiesFromGeoObj(ToponymType.City, data);

        }

        private string GetUrlByQuery(string searchQuery, YandexCountry country, string apiKey, string lang)
        {
            var url = string.Format(_getCitiesEndpoint, searchQuery, apiKey, _countOfResults, lang);

            if (country != YandexCountry.Everywhere)
            {

                url = string.Concat(url, "&in=" + (int)country);
            }
            return url;		

        }
        public IReadOnlyList<CityAddress> GetCities(string apiKey, CityAddressCountry country,
            string searchQuery, ILanguage language)
        {
            var yandexCountry = DataTranslator.CountryConvert(country);
            var lang = DataTranslator.LanguageConvert(language);
            var url = GetUrlByQuery(searchQuery, yandexCountry, apiKey, lang);

            var answer = string.Empty;

            void SendRequest()
            {
                var response = url.AllowAnyHttpStatus().GetAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                answer = response.Content.ReadAsStringAsync().Result;
            }

            _rateLimiter.Perform((Action)SendRequest).ConfigureAwait(false).GetAwaiter().GetResult();
			
            var jsonObj = JObject.Parse(answer);

            var data = (jsonObj["response"]?["GeoObjectCollection"]?["featureMember"] ?? new JObject()).Select(j => j["GeoObject"].ToObject<YandexGeoObject>()).ToList();
            return DataTranslator.GetCitiesFromGeoObj(ToponymType.City, data);
        }
        
	}	
}