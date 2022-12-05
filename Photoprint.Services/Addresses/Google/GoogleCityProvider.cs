using System;
using System.Collections.Generic;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using Photoprint.Core.Models;
using Photoprint.Services.Infrastructure;

namespace Photoprint.Services.Addresses.Google
{
    public class GoogleCityProvider: IDeliveryCityAddressProvider
    {
        public string ProviderName => "Google";
        public string CacheKey => "google";

        private readonly RateLimiter _rateLimiter = new RateLimiter(new CountByIntervalAwaitableConstraint(5, TimeSpan.FromSeconds(1)));

        private const string _byQueryEndpoint = "https://maps.googleapis.com/maps/api/geocode/json?key={0}&address={1}&language={2}&result_type=locality";

        private static string GetCitiesByQuerytUrl(string apiKey, string query, CityAddressCountry country, ILanguage language)
        {
            var url = string.Format(_byQueryEndpoint, apiKey, query, language.LanguageCode);
            var countryCode = DataTranslator.GetCountryCode(country);
            if (countryCode != null)
            {
                url = string.Concat(url, $"&region={countryCode}");
            }
            return url;
        }
        public IReadOnlyList<CityAddress> GetCities(string apiKey, CityAddressCountry country,
            string searchQuery, ILanguage language)
        {
            var url = GetCitiesByQuerytUrl(apiKey, searchQuery, country, language);
            var answer = string.Empty;

            void SendRequest()
            {
                var response = url.AllowAnyHttpStatus().GetAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                answer = response.Content.ReadAsStringAsync().Result;
            }

            _rateLimiter.Perform(SendRequest).ConfigureAwait(false).GetAwaiter().GetResult();
			
            var jsonObj = JObject.Parse(answer);
            var data = (jsonObj["results"]?? new JObject()).ToObject<List<GoogleGeocodeResult>>()?? new List<GoogleGeocodeResult>();
            return data.Count == 0 ? null : DataTranslator.GetCitiesFromGeoObj(ToponymType.City, data.AsReadOnly());
        }

        private const string _bySuggestEndpoint = "https://maps.googleapis.com/maps/api/geocode/json?key={0}&place_id={1}&language={2}";

        private string GetCitiesBySuggestUrl(string apiKey, string placeId, CityAddressCountry country,
            ILanguage language)
        {
            return string.Format(_bySuggestEndpoint, apiKey, placeId, language.LanguageCode);
        }
        public IReadOnlyList<CityAddress> GetCitiesBySuggest(string apiKey, CityAddressCountry country,
            CitySuggest suggest,
            ILanguage language)
        {
            var url = GetCitiesBySuggestUrl(apiKey, suggest.GeoId, country, language);
            var answer = string.Empty;

            void SendRequest()
            {
                var response = url.AllowAnyHttpStatus().GetAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                answer = response.Content.ReadAsStringAsync().Result;
            }

            _rateLimiter.Perform(SendRequest).ConfigureAwait(false).GetAwaiter().GetResult();
			
            var jsonObj = JObject.Parse(answer);
            var data = (jsonObj["results"]?? new JObject())
                .ToObject<List<GoogleGeocodeResult>>()?? new List<GoogleGeocodeResult>();
            return data.Count == 0 ? new List<CityAddress>() : DataTranslator.GetCitiesFromGeoObj(suggest.Type, data.AsReadOnly());
        }

        private const string _byCoordsEndpoint = "https://maps.googleapis.com/maps/api/geocode/json?key={0}&latlng={1}&language={2}&result_type=locality";

        private string GetCitiesByCoordstUrl(string apiKey, string latitude, string longitude, ILanguage language)
        {
            return string.Format(_byCoordsEndpoint, apiKey, $"{latitude}, {longitude}", language.LanguageCode);
        }
        public IReadOnlyList<CityAddress> GetCities(string apiKey, string latitude,
            string longitude, ILanguage language)
        {
            var url = GetCitiesByCoordstUrl(apiKey, latitude, longitude, language);
            var answer = string.Empty;

            void SendRequest()
            {
                var response = url.AllowAnyHttpStatus().GetAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                answer = response.Content.ReadAsStringAsync().Result;
            }

            _rateLimiter.Perform(SendRequest).ConfigureAwait(false).GetAwaiter().GetResult();
			
            var jsonObj = JObject.Parse(answer);
            var data = (jsonObj["results"]?? new JObject()).ToObject<List<GoogleGeocodeResult>>()?? new List<GoogleGeocodeResult>();
            return data.Count == 0 ? new List<CityAddress>() : DataTranslator.GetCitiesFromGeoObj(ToponymType.City, data.AsReadOnly());
        }
    }
}
