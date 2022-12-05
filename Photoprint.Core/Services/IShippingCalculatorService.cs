using System.Collections.Generic;
using Photoprint.Core.Models;

namespace Photoprint.Core.Services
{
    public interface IShippingCalculatorService
    {
        bool TryGetApproximateShippingPrice(Shipping shipping, IReadOnlyCollection<IPurchasedItem> items,
            IReadOnlyCollection<Discount> discounts, out decimal resultPrice, out bool isPriceFixed);
        DeliveryPriceCalculationResult GetShippingPrice(Shipping shipping, OrderAddress selectedAddress,
            IReadOnlyCollection<IPurchasedItem> items, IReadOnlyCollection<Discount> discounts);

        DeliveryPriceCalculationResult GetShippingPriceWithoutDiscount(Shipping shipping,
            OrderAddress selectedAddress,
            IReadOnlyCollection<IPurchasedItem> items);
    }
}