using System.Collections.Generic;
using Photoprint.Core.Models;

namespace Photoprint.Services.Addresses
{
	public interface ICityAddressProvider
	{
		string ProviderName { get; }
        string CacheKey { get; }

        IReadOnlyList<CityAddress> GetCities(CityAddressCountry country, string searchQuery);
        IReadOnlyList<CityAddress> GetCity(CityAddressCountry country, float lat, float lon);
		IReadOnlyList<CityAddress> GetCity(Language language, float lat, float lon);
        
    }
}