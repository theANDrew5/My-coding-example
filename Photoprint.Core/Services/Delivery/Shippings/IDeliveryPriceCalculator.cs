using System.Collections.Generic;
using Photoprint.Core.Models;

namespace Photoprint.Core.Services
{
    public interface IDeliveryPriceCalculator
    {
        DeliveryPriceCalculationResult CalculateDeliveryPrice(Postal postal, OrderAddress selectedAddress,
            IReadOnlyCollection<IPurchasedItem> items);
    }
}