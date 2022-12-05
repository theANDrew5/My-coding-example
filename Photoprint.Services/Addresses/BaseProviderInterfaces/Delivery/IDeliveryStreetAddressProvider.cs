using Photoprint.Core.Models;

namespace Photoprint.Services.Addresses
{
    public interface IDeliveryStreetAddressProvider
    {
        string CacheKey { get; }
        BoundedAddress GetAddressBySuggest(string apiKey, AddressSuggest suggest, ILanguage language);
        BoundedAddress GetAddressByQuery(string apiKey, string query, CityAddress city, ILanguage language);
        BoundedAddress GetAddressByCoords(string apiKey, string lat, string lng, ILanguage language);
    }
}