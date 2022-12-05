using Photoprint.Core.Models;
using System.Collections.Generic;

namespace Photoprint.Core.Services
{
    public interface IRussianPostProviderService : IShippingProviderService
    {
        bool IsPostOfficeNearAddress(Postal postal, string city);
        IReadOnlyCollection<ShippingAddress> GetShippingAddressesByCity(Postal postal, CityAddress city);

        void GetOrderTrackingNumber(Photolab photolab, Order order, IShippingServiceProviderSettings providerSettings);

        IReadOnlyCollection<UserPostOffice> GetUserPostOffices(Shipping shipping,
            RussianPostServiceProviderSettings settings);

        IReadOnlyCollection<ShippingAddress> GetAllShippingAddresses(Postal postal);
        IReadOnlyCollection<ShippingAddress> GetShippingAddressesByCity(ShippingSmallToDeliveryModel shipping, CityAddress city);
    }
}
