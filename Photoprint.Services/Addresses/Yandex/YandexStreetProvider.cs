using Flurl.Http;
using Newtonsoft.Json.Linq;
using Photoprint.Core.Models;
using System;
using Photoprint.Services.Infrastructure;

namespace Photoprint.Services.Addresses.Yandex
{
    public class YandexStreetProvider : IDeliveryStreetAddressProvider
	{
		// test case https://suggest-maps.yandex.ru/suggest-geo?callback=id_1&apikey=60445215-6d3a-4f88-87fe-8d52b72e5bc9&v=5&search_type=tp&n=10&part=Томск+Лебедева+1
		private const int _countOfResults = 1;
        private const string _suggestDataResponseGroupKey = "suggestData";
		private readonly RateLimiter _rateLimiter = new RateLimiter(new CountByIntervalAwaitableConstraint(5, TimeSpan.FromSeconds(1)));

        public string CacheKey => "yandex";
        
		public BoundedAddress GetAddressBySuggest(string apiKey, AddressSuggest suggest,
            ILanguage language)
        {
            if (suggest.SearchByCoordsEnable && !string.IsNullOrWhiteSpace(suggest.House))
                return GetAddressByCoords(apiKey, suggest.Latitude, suggest.Longitude, language);
            return !string.IsNullOrWhiteSpace(suggest.SearchString) ? GetAddressByQuery(apiKey, suggest.SearchString, suggest.City, language) : null;
        }

        private const string _getAddressByCoordsEndpoint = "https://geocode-maps.yandex.ru/1.x/?apikey={1}&format=json&sco=latlong&kind=house&results={2}&lang={3}&geocode={0}";
        private string GetAddressByCoordUrl(string apiKey, string lat, string lng, string language)
        {
            var part = lat + "," + lng;
            return string.Format(_getAddressByCoordsEndpoint, part, apiKey, _countOfResults, language);
        }
		public BoundedAddress GetAddressByCoords(string apiKey, string lat, string lng, ILanguage language)
        {
			var lang = DataTranslator.LanguageConvert(language);
			var url = GetAddressByCoordUrl(apiKey, lat, lng, lang);

            var answer = string.Empty;
            void SendRequest()
            {
                var response = url
                    .AllowAnyHttpStatus()
                    .GetAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                answer = response.Content.ReadAsStringAsync().Result;
            }

            _rateLimiter.Perform((Action)SendRequest).ConfigureAwait(false).GetAwaiter().GetResult();

            var jsonObj = JObject.Parse(answer);
            var results = jsonObj["response"]?["GeoObjectCollection"]?["featureMember"]?.Value<JArray>();
            var data = results?.HasValues ?? false ? results[0]["GeoObject"]?.ToObject<YandexGeoObject>() : null;
            return data is null ? null : DataTranslator.GetAddressFromGeoObj(data);
        }

        private const string _getAddressByQueryEndpoint = "https://geocode-maps.yandex.ru/1.x/?apikey={1}&format=json&results={2}&lang={3}&geocode={0}";
        private string GetAddressByQueryUrl(string apiKey, string query, CityAddress city, string language)
        {
            var url = string.Format(_getAddressByQueryEndpoint, query, apiKey, _countOfResults, language);
            if(city.Bound.IsAvailable)
                url = string.Concat(url, $"&bbox={city.Bound.UpperLatitude},{city.Bound.UpperLongitude}~{city.Bound.LowerLatitude},{city.Bound.LowerLongitude}");
            return url;
        }
		public BoundedAddress GetAddressByQuery(string apiKey, string query, CityAddress city, ILanguage language)
        {
			var lang = DataTranslator.LanguageConvert(language);
			var url = GetAddressByQueryUrl(apiKey, query, city, lang);

            var answer = string.Empty;
            void SendRequest()
            {
                var response = url
                    .AllowAnyHttpStatus()
                    .GetAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                answer = response.Content.ReadAsStringAsync().Result;
            }

            _rateLimiter.Perform((Action)SendRequest).ConfigureAwait(false).GetAwaiter().GetResult();

            var jsonObj = JObject.Parse(answer);
            var results = jsonObj["response"]?["GeoObjectCollection"]?["featureMember"]?.Value<JArray>();
            var data = results?.HasValues ?? false ? results[0]["GeoObject"]?.ToObject<YandexGeoObject>() : null;
            return data != null ? DataTranslator.GetAddressFromGeoObj(data) : null;
        }

	}
}
