using Flurl.Http;
using Newtonsoft.Json.Linq;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Photoprint.Services.Infrastructure;

namespace Photoprint.Services
{
    public class YandexSuggestProvider: ISuggestProvider
    {
		public string cacheKey => "yandex";
		private readonly RateLimiter _rateLimiter = new RateLimiter(new CountByIntervalAwaitableConstraint(5, TimeSpan.FromSeconds(1)));

		private const int _countOfSuggestResults = 10;
		private const int _version = 7;
		private const string _suggestEndpoint = "https://suggest-maps.yandex.ru/suggest-geo?v={2}&n={1}&lang={3}&search_type=tp&outformat=json&part={0}";
        private const string _suggestCountryEndpoint = "https://suggest-maps.yandex.ru/suggest-geo?v={2}&n={1}&search_type=tp&outformat=json&part={0}";

        private string GetSuggestCityUrl(YandexCountry country, string query, string language)
        {
			var url = string.Format(_suggestEndpoint, query, _countOfSuggestResults, _version, language);
			if (country != YandexCountry.Everywhere)
			{
				url = string.Concat(url, "&in=" + (int)country);
			}
			return url;
		}
        public IReadOnlyList<CitySuggest> GetCitySuggests(string key,
            CityAddressCountry country,
            ToponymType toponymType,
            string query,
            ILanguage language)
        {
			var yaCountry = DataTranslator.CountryConvert(country);
			var lang = DataTranslator.LanguageConvert(language);
            var url = GetSuggestCityUrl(yaCountry, query, lang);

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
            var results = json["results"]?.ToObject<IReadOnlyList<YandexSuggestResult>>();
            if (results == null) return Array.Empty<CitySuggest>();
            switch (toponymType)
            {
                case ToponymType.Country:
                    return results.Where(res => res.Kind=="country")
                        .Select(res => new CitySuggest(res)).ToList();
                case ToponymType.Region:
                    return results.Where(res => res.Kind == "province")
                        .Select(res => new CitySuggest(res)).ToList();
                case ToponymType.City:
                    return results.Where(res => res.Kind == "locality")
                        .Select(res => new CitySuggest(res)).ToList();
                default: return Array.Empty<CitySuggest>();

            }
        }

		private string GetSuggestAddressUrl(string query, string laguage)
        {
			return string.Format(_suggestEndpoint, query, _countOfSuggestResults, _version, laguage);
        }

		public IReadOnlyList<AddressSuggest> GetAddressSuggest(string key,
            string query, string street, CityAddress cityInfo, SuggestType type,
            ILanguage language)
        {
			var lang = DataTranslator.LanguageConvert(language);
			var url = GetSuggestAddressUrl(query, lang);

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
            var results = json["results"]?.ToObject<IReadOnlyList<YandexSuggestResult>>();
			if (results==null)
				return Array.Empty<AddressSuggest>();

            switch (type)
            {
                default:
                case SuggestType.Street:
                {
                    return results.Where(r => r.Kind == "street").Select(result => new AddressSuggest
                    {
                        City = cityInfo,
                        Street = result.Name,
                        Latitude = result.Latitude,
                        Longitude = result.Longitude,
                        Description = result.Description,
                        GeoId = result.GeoId
                    }).ToList();
                }
                case SuggestType.House:
                {
                    if (string.IsNullOrWhiteSpace(street))
                    {
                        return (from result in results.Where(r => r.Kind == "house")
                            let names = Regex.Split(result.Name, @", ")
                            select new AddressSuggest
                            {
                                City = cityInfo,
                                Street = string.Empty,
                                House = names[1],
                                Latitude = result.Latitude,
                                Longitude = result.Longitude,
                                Description = result.Description,
                                GeoId = result.GeoId
                            }).ToList();
                    }

                    return (from result in results.Where(r => r.Kind == "house")
                        let names = Regex.Split(result.Name, @", ")
                        where DataTranslator.CheckSuggestData(result.Description, names[0], cityInfo.Title, street)
                        select new AddressSuggest
                        {
                            City = cityInfo,
                            Street = names[0],
                            House = names[1],
                            Latitude = result.Latitude,
                            Longitude = result.Longitude,
                            Description = result.Description,
                            GeoId = result.GeoId
                        }).ToList();
                    }
            }
        }


        public CityAddressCountryClass GetCountry(string query)
        {
            var url = string.Format(_suggestCountryEndpoint, query, _countOfSuggestResults, _version);
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
            if (string.IsNullOrWhiteSpace(answer))   return new CityAddressCountryClass{County =  CityAddressCountry.NoCountry};
            var json = JObject.Parse(answer);
            var results = json["results"]?.ToObject<IReadOnlyList<YandexSuggestResult>>();
            if (results == null) return new CityAddressCountryClass {County = CityAddressCountry.NoCountry};
            var countryId = Enum.TryParse(results.FirstOrDefault(res => res.Kind == "country")?.GeoId?? "0", out YandexCountry id)? id: YandexCountry.Everywhere;
            return new CityAddressCountryClass{County =  DataTranslator.CountryConvert((YandexCountry)countryId)};
        }
    }
}
