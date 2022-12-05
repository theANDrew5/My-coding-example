using Photoprint.Core.Models;
using System.Collections.Generic;

namespace Photoprint.Core.Repositories
{
    public interface IShippingAddressPricesRepository
    {
        ShippingAddressPrice GetOrCreate(int photolabId, byte[] hash, ShippingPrices shippingPrices, out bool isNewCreated);
        ShippingAddressPrice GetById(int? priceId);
        IReadOnlyCollection<ShippingAddressPrice> GetByShippingId(int shippingId);
        Dictionary<int, ShippingAddressPrice> GetListByPhotolab(int photolabId);
        void UpdateShippingAddressesByIds(int? priceId, int shippingId, IReadOnlyCollection<int> shippingAddressesIds);
        void UpdateShippingPrices(ShippingAddressPrice shippingAddressPrice, byte[] hash);

        /// <summary>
        /// for fix ONLY
        /// remove after fix
        /// </summary>
        Dictionary<int, ShippingAddressPrice> GetOldListByPhotolab(int photolabId);
    }
}
