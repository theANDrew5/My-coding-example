using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Flurl.Http;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using Photoprint.Services.Infrastructure;

namespace Photoprint.Services.Addresses
{
    public class GoogleSuggestProvider: ISuggestProvider
    {
        public string cacheKey => "google";
        private readonly RateLimiter _rateLimiter = new RateLimiter(new CountByIntervalAwaitableConstraint(5, TimeSpan.FromSeconds(1)));

        private const string _suggestEndpoint =
            "https://maps.googleapis.com/maps/api/place/autocomplete/json?key={0}&input={1}&language={2}&types={3}";

        private string GetAddressSuggestUrl(string key, string query,
            ILanguage language,
            [CanBeNull] CityAddress city = null)
        {
            var url = string.Format(_suggestEndpoint, key, query, language.LanguageCode, "address");
            //var coutryCode = DataTranslator.GetCountryCode(country);
            //if (!string.IsNullOrEmpty(coutryCode))
            //{
            //    url = string.Concat(url,$"&components=country:{coutryCode}");
            //}

            if (city == null) return url;
            if (city.IsCoordsAvailable)
            {
                url = string.Concat(url, $"&location={city.Latitude}, {city.Longitude}&radius={city.Bound.Radius?? "50000"}&strictbounds=true");
            }
            return url;

        }

        public IReadOnlyList<AddressSuggest> GetAddressSuggest(string key,
            string query, string street, CityAddress cityInfo, SuggestType type,
            ILanguage language)
        {
            var url = GetAddressSuggestUrl(key, query, language, cityInfo);
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
            if (string.IsNullOrWhiteSpace(answer)) return Array.Empty<AddressSuggest>();

            var json = JObject.Parse(answer);
            var results = json["predictions"]?.ToObject<IReadOnlyList<GoogleSuggestResult>>();
            return results?.Select(
                result =>
                {
                    var names = Regex.Split(result.Formatting.MainText, ", ");
                    return new AddressSuggest
                    {
                        City = cityInfo,
                        Description = result.Formatting.SecondaryText,
                        GeoId = result.PlaceId,
                        Street = names[0],
                        House = names.Length > 1 ? names[1] : string.Empty
                    };
                }).Where(suggest => street==null || string.Equals(street, suggest.Street, StringComparison.InvariantCultureIgnoreCase)).ToList()?? new List<AddressSuggest>();

        }
        
        private string GetSuggestCityUrl(string key,
            ToponymType toponymType, string query, ILanguage language, CityAddressCountry country)
        {
            string type;
            switch (toponymType)
            {
                case ToponymType.Country:
                case ToponymType.Region:
                    type = "(regions)";
                    break;
                case ToponymType.City:
                    type = "(cities)";
                    break;
                default:
                    type = string.Empty;
                    break;
            }

            var url = string.Format(_suggestEndpoint, key, query, language.LanguageCode, type);
            var coutryCode = DataTranslator.GetCountryCode(country);
            if (!string.IsNullOrEmpty(coutryCode))
            {
                url = string.Concat(url,$"&components=country:{coutryCode}");
            }
            return url;
        }
        public IReadOnlyList<CitySuggest> GetCitySuggests(string key,
            CityAddressCountry country,
            ToponymType toponymType,
            string query,
            ILanguage language)
        {
            var url = GetSuggestCityUrl(key, toponymType, query, language, country);
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
            if (string.IsNullOrWhiteSpace(answer)) return Array.Empty<CitySuggest>();

            var json = JObject.Parse(answer);
            var results = json["predictions"]?.ToObject<IReadOnlyList<GoogleSuggestResult>>();
            if (results==null)
                return Array.Empty<CitySuggest>();
            switch (toponymType)
            {
                case ToponymType.Country:
                    return results.Where(result => result.Types[0] == "country")
                        .Select(result => new CitySuggest(result)).ToList();
                case ToponymType.Region:
                    return results.Where(result => result.Types[0] == "administrative_area_level_1")
                        .Select(result => new CitySuggest(result)).ToList();
                case ToponymType.City:
                    return results.Where(result => result.Types[0] == "locality")
                        .Select(result => new CitySuggest(result)).ToList();
                default: return Array.Empty<CitySuggest>();
            }
        }

        private const string _suggestCountryEndpoint = "https://maps.googleapis.com/maps/api/place/textsearch/json?query={0}&key={1}&language=en";
        public CityAddressCountryClass GetCountry(string query)
        {
            throw new System.NotImplementedException();
            //var key = settings.MapSettings.MapSettings?.ApiKey;
            //if (key == null) return new CityAddressCountryClass { County = CityAddressCountry.NoCountry };
            //var url = string.Format(_suggestCountryEndpoint, query, key);
            //var answer = string.Empty;
            //void SendRequest()
            //{
            //    var response = url
            //        .AllowAnyHttpStatus()
            //        .GetAsync()
            //        .ConfigureAwait(false)
            //        .GetAwaiter()
            //        .GetResult();

            //    answer = response.Content.ReadAsStringAsync().Result;
            //}
            //_rateLimiter.Perform((Action)SendRequest).ConfigureAwait(false).GetAwaiter().GetResult();
            //if (string.IsNullOrWhiteSpace(answer)) return new CityAddressCountryClass { County = CityAddressCountry.NoCountry };
            //var json = JObject.Parse(answer);
            //var results = json["results"]?.ToObject<IReadOnlyList<GoogleSuggestResult>>();
            //if (results == null || results.Count == 0) return new CityAddressCountryClass { County = CityAddressCountry.NoCountry };
            //var countryName = results.FirstOrDefault(res => res.Types[0] == "country")?.Name ?? "NoCountry";
            //return new CityAddressCountryClass { County = DataTranslator.CountryConvert(countryName) };
        }
    }
}
