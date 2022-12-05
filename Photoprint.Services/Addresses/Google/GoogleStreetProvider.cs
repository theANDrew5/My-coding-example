using System;
using System.Collections.Generic;
using System.Linq;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using Photoprint.Core.Models;
using Photoprint.Services.Infrastructure;

namespace Photoprint.Services.Addresses.Google
{
    public class GoogleStreetProvider: IDeliveryStreetAddressProvider
    {
        public string CacheKey => "google";
        private readonly RateLimiter _rateLimiter = new RateLimiter(new CountByIntervalAwaitableConstraint(5, TimeSpan.FromSeconds(1)));
        private const string _bySuggestEndpoint = "https://maps.googleapis.com/maps/api/geocode/json?key={0}&place_id={1}&language={2}";
        private string GetAddressBySuggestUrl(string apiKey, string placeId, ILanguage language)
        {
            return string.Format(_bySuggestEndpoint, apiKey, placeId, language.LanguageCode);
        }
        public BoundedAddress GetAddressBySuggest(string apiKey, AddressSuggest suggest,
            ILanguage language)
        {
            var url = GetAddressBySuggestUrl(apiKey, suggest.GeoId, language);
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
            var data = (jsonObj["results"]?? new JObject()).ToObject<List<GoogleGeocodeResult>>();
            var geoObj = !string.IsNullOrWhiteSpace(suggest.House)
                ? data?.FirstOrDefault(d =>
                    d.Types[0] == "street_address" || d.Types[0] == "premise" || d.Types[0] == "subpremise")
                : data?.FirstOrDefault(d => d.Types[0] == "route"); 
            return geoObj == null ? null : DataTranslator.GetAddressFromGeoObj(geoObj);
        }

        private const string _byQueryEndpoint = "https://maps.googleapis.com/maps/api/geocode/json?key={0}&address={1}&language={2}&result_type=street_address|premise|subpremise";

        private string GetAddressByQueryUrl(string apiKey, string query, CityAddress city, ILanguage language)
        {
            var result = string.Format(_byQueryEndpoint, apiKey, query, language.LanguageCode);
            if (city.Bound.IsAvailable)
                result = string.Concat(
                    $"{city.Bound.UpperLatitude},{city.Bound.UpperLongitude}|{city.Bound.LowerLatitude},{city.Bound.LowerLongitude}",
                    result);
            return result;
        }
        public BoundedAddress GetAddressByQuery(string apiKey, string query, CityAddress city, ILanguage language)
        {
            var url = GetAddressByQueryUrl(apiKey, query, city, language);
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
            var data = (jsonObj["results"]?? new JObject()).ToObject<List<GoogleGeocodeResult>>();
            var geoObj = data?.FirstOrDefault();
            return geoObj == null ? null : DataTranslator.GetAddressFromGeoObj(geoObj);
        }

        private const string _byCoordsEndpoint = "https://maps.googleapis.com/maps/api/geocode/json?key={0}&latlng={1}&language={2}&result_type=street_address|premise|subpremise";

        private string GetAddressByCoordsUrl(string apiKey, string latitude, string longitude, ILanguage language)
        {
            return string.Format(_byCoordsEndpoint, apiKey, $"{latitude}, {longitude}", language.LanguageCode);
        }
        public BoundedAddress GetAddressByCoords(string apiKey, string lat, string lng, ILanguage language)
        {
            var url = GetAddressByCoordsUrl(apiKey, lat, lng, language);
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
            var data = (jsonObj["results"]?? new JObject()).ToObject<List<GoogleGeocodeResult>>();
            var geoObj = data?.FirstOrDefault();
            return geoObj == null ? null : DataTranslator.GetAddressFromGeoObj(geoObj);
        }
    }
}
