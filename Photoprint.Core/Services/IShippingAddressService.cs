using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Photoprint.Core.Models;

namespace Photoprint.Core.Services
{
    public interface IShippingAddressService
    {
        ShippingAddress Create(ShippingAddress address, Shipping shipping);

        IReadOnlyCollection<ShippingAddress> CreateList(IReadOnlyCollection<ShippingAddress> addresses,
            Shipping shipping);

        ShippingAddress GetById(int id, Shipping shipping = null);
        IReadOnlyCollection<ShippingAddress> GetListByIds(IReadOnlyCollection<int> addressesIds);
        IReadOnlyCollection<ShippingAddress> GetList(Shipping shipping);
        IReadOnlyCollection<ShippingAddress> GetAvailableList(Shipping shipping);
        void DeleteList(IReadOnlyCollection<ShippingAddress> shippingAddresses, Shipping shipping);
        void UpdateList(IReadOnlyCollection<ShippingAddress> shippingAddresses, Shipping shipping);
        void UpdateListStatus(IReadOnlyCollection<int> addressesIds, Shipping shipping, bool status);
        void Update(ShippingAddress address, Shipping shipping);
        ShippingAddress GetSuitableShipingAddresses(Shipping selectedShipping, string selectedCountry, string selectedRegion, string selectedCity, string address = null);
        void Copy(Shipping source, Shipping target, Photolab targetPhotolab);

        IReadOnlyDictionary<ShippingSmallToDeliveryModel, IReadOnlyCollection<ShippingAddress>>
            GetShippingsSmallWithAddresses(Photolab photolab,
                CityAddress city,
                IReadOnlyCollection<ShoppingCartItem> items,
                IReadOnlyCollection<int> shippingsIds, IShippingProviderResolverService providerResolver);
    }

    public interface IUserManualShippingAddressService
    {
        ShippingAddress AddPostalAddress(ShippingAddress address, Postal postal);
        ShippingAddress AddCourierAddress(ShippingAddress address, Courier courier);
        void DeleteShippingAddresses(IReadOnlyCollection<int> shippingAddresses, Shipping shipping);
        ShippingAddress GetById(int id, Shipping shipping = null);
        IReadOnlyCollection<ShippingAddress> GetList(Shipping shipping);
        void UpdateShippingAddress(ShippingAddress address, Shipping shipping);
        void UpdateShippingAddressPositions(Dictionary<int, int> positions, Shipping shipping);
    }
}
