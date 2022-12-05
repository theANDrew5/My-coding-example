using Photoprint.Core.Models;
using System.Collections.Generic;

namespace Photoprint.Core.Services
{
    public interface IShippingPriceService
    {
        ShippingAddressPrice GetOrCreateShippingAddressPrice(PhotolabSmall photolab, ShippingPrices shippingPrices);
        ShippingAddressPrice GetById(PhotolabSmall photolab, int? priceId);
        IReadOnlyCollection<ShippingAddressPrice> GetListByShipping(Shipping shipping);
        void UpdatePrice(int? priceId, Shipping shipping, IReadOnlyCollection<int> shippingAddressesId);

        // Price calculation
        decimal GetMinimumPrice(PhotolabSmall photolab, int? priceId);
        bool TryCalculatePriceForShipping(Shipping shipping, IReadOnlyCollection<IShippable> items,
            out decimal resultPrice,
            out bool isFixed);

        bool TryCalculatePriceForShippingAddress(int photolabId, ShippingAddress address,
            IReadOnlyCollection<IShippable> items,
            out decimal resultPrice);

        bool TryCalculatePriceForShipping(Shipping shipping, IReadOnlyCollection<IShippable> items,
            out decimal resultPrice);

        bool TryCalculatePriceForOrderAddress(int photolabId, OrderAddress address,
            IReadOnlyCollection<IShippable> items,
            out decimal resultPrice);
    }
}
