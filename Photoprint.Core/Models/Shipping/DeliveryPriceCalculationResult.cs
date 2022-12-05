using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Photoprint.Core.Models
{
    public class DeliveryPriceCalculationResult
    {
        public DeliveryPriceCalculationResult() { }
        public DeliveryPriceCalculationResult(decimal? cost = null)
        {
            if (cost.HasValue)
            {
                Cost = cost.Value;
                Success = true;
            }
        }

        public DeliveryPriceCalculationResult(decimal? cost = null, DeliveryAddressProperties properties = null,
            int? minDeliveryPeriod = null, int? maxDeliveryPeriod = null) : this(cost)
        {
            Properties = properties;
            if (minDeliveryPeriod.HasValue && minDeliveryPeriod > 0)
            {
                MinPeriod = minDeliveryPeriod.Value;
            }
            if (maxDeliveryPeriod.HasValue && maxDeliveryPeriod > 0)
            {
                MaxPeriod = maxDeliveryPeriod.Value;
            }
        }

        public void ApplyDiscounts(IReadOnlyCollection<Discount> discounts, IReadOnlyCollection<IPurchasedItem> items)
        {
            var quantity = items.Sum(i => i.Quantity);
            var partsQuatity = items.Sum(i => i.ItemPartsQuantity);
            Discount = discounts.Where(i => i.ValueForShipping.HasValue)
                .Sum(d => d.ValueForShipping.CalculateFor(Cost, quantity, partsQuatity));
        }


        public bool Success { get;}

        public decimal Price => Success ? Cost - Discount : 0m;
        public decimal Cost { get; }
        public decimal Discount { get; private set; }
        public int MinPeriod { get; }
        public int MaxPeriod { get; }

        //пропсы получаемые в IDeliveryPriceCalculator.CalculateDeliveryPrice
        //JObject потомучто сериализатор для пропсов написан только под NewtonSoft
        //нудны для:
        //CDEK v1
        //RussianPost
        public DeliveryAddressProperties Properties { get; }
    }
}