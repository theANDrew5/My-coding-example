using Photoprint.Core.Models;
using System.Collections.Generic;

namespace Photoprint.Services.Addresses
{
    public interface IDeliveryCityAddressProvider
    {
        string ProviderName { get; }
        string CacheKey { get; }
        
        IReadOnlyList<CityAddress> GetCities(string apiKey, CityAddressCountry country,
            string searchQuery, ILanguage language);

        IReadOnlyList<CityAddress> GetCitiesBySuggest(string apiKey, CityAddressCountry country,
            CitySuggest suggest,
            ILanguage language);

        IReadOnlyList<CityAddress> GetCities(string apiKey,
            string latitude, string longitude, ILanguage language);
    }
}
