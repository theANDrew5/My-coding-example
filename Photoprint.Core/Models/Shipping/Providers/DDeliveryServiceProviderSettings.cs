using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using Photoprint.Core.Models.DDelivery;

namespace Photoprint.Core.Models
{
    public enum DDeliveryDeclaredPriceMode : byte
    {
        Submit = 0, // Отправлять
        [Obsolete("Use \"Distort\" instead")]
        Prevent = 1, // Не отправлять
        Distort = 2 // Модификация данных перед отправкой
    }

	public class DDeliveryServiceProviderSettings : IShippingServiceProviderSettings
	{
		public bool UpdateShippingAddressesAutomatically { get; set; }
        public bool RegisterOrderInProviderService { get; set; }
	    public bool ChangeOrderStatusToShippedAfterAutomaticRegistration { get; set; }

	    public bool SupportAddresesSynchronization => true;
	    public bool ShowAddressTab => false;
	    public bool MuteNotificationAfterAddressesUpdated { get; set; }
        
	    public string ApiKey { get; set; }
        public string CssUrl { get; set; }
		public IList<DDeliveryShippingType> SupportedShippingTypes { get; set; }

		public int DimensionSide1 { get; set; }
		public int DimensionSide2 { get; set; }
		public int DimensionSide3 { get; set; }
		public double DefaultWeight { get; set; }

		public decimal AdditionalPrice { get; set; }
		public decimal PriceCoefficient { get; set; }

		public bool RoundPrice { get; set; }
		public int DaysAddition { get; set; }

		public bool ExcludePickupPrice { get; set; }
		public bool IsTestMode { get; set; }

        public DDeliveryDeclaredPriceMode DeclaredPriceMode { get; set; }
        public int FixedDeclaredPrice { get; set; }
        [Obsolete("Use DeclaredPriceMode instead")]
		public bool IsDeclaredPriceTransmissionDisabled { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool ShowSelectorInModalWindow { get; set; }

        public DDeliveryServiceProviderSettings() { }

		public decimal GetDeclaredPrice(IReadOnlyCollection<IPurchasedItem> items) => items.Sum(i => i.Price);
		
        public double GetWeight(IReadOnlyCollection<IPurchasedItem> items)
		{
			var weight = items.Sum(i => i.ItemWeight * i.Quantity);
			if (weight < 0.05) weight = DefaultWeight;
			return weight;
		}

		public decimal GetPaymentPrice(IEnumerable<IPurchasedItem> details)
		{
			return 0m;
		}

		public decimal GetTotalShippingPrice(DDeliveryCalculatorResult calculatorResult, IReadOnlyCollection<Discount> discounts, IReadOnlyCollection<IPurchasedItem> items)
		{
		    if (calculatorResult == null) return 0m;
            var coef = Math.Abs(PriceCoefficient) < 0.01m ? 1 : PriceCoefficient;
            var price = ExcludePickupPrice
				? calculatorResult.TotalPrice - calculatorResult.PickupPrice
				: calculatorResult.TotalPrice;

			price = price * coef + AdditionalPrice;

			if (RoundPrice)
			{
				price = Math.Round(price, 0);
			}
            
			if (price > 0 && discounts != null)
			{
				foreach (var discount in discounts.Where(d => d.ValueForShipping.HasValue))
				{
				    price = discount.ValueForShipping.Apply(price, items.Sum(i => i.Quantity), items.Sum(x => x.ItemPartsQuantity));
				}
			}

			price = Math.Round(price, 2);

			return price;
		}

		public void ApplyTo(IReadOnlyCollection<DDeliveryCalculatorResult> results, IReadOnlyCollection<Discount> discounts, IReadOnlyCollection<IPurchasedItem> items)
		{
			foreach (var result in results)
			{
                if (result == null) continue;
				result.TotalPriceCorrected = GetTotalShippingPrice(result, discounts, items);
				result.DeliveryTimeMinCorrected = GetShippingTime(result.DeliveryTimeMin);
				result.DeliveryTimeAvgCorrected = GetShippingTime(result.DeliveryTimeAvg);
				result.DeliveryTimeMaxCorrected = GetShippingTime(result.DeliveryTimeMax);
			}
		}

		private int GetShippingTime(int deliveryTime)
		{
			return deliveryTime + DaysAddition;
		}
	}
}