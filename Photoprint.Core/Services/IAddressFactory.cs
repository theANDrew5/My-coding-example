using Photoprint.Core.Models;
using Photoprint.Core.Models.MobileAppSettings;

namespace Photoprint.Core.Services
{
    public interface IAddressFactory
    {
        Address AddressByCoordsFind(ILanguage language, string lat, string lng, Photolab photolab, MobileAppMapsSettings mobileAppMapsSettings = null, bool isMobile = false);
        Address GetAddress(ILanguage language, AddressInfoData data, Photolab photolab = null, MobileAppMapsSettings mobileAppMapsSettings = null, bool isMobile = false);
    }
}
