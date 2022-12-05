using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class ShippingAddressPrice
    {
        public int? Id { get; }
        public int PhotolabId { get; }
        public ShippingPrices PriceList { get; }

        public ShippingAddressPrice(int? id, int photolabId, ShippingPrices priceList)
        {
            Id = id;
            PhotolabId = photolabId;
            PriceList = priceList;
        }
    }


	[Serializable]
	public class ShippingPrice
	{
        public ShippingPrice() { }

		public ShippingPrice(double weight, decimal price, double additionalWeight, decimal additionalPrice)
		{
			Weight = weight;
			Price = (double)price;
			AdditionalOneKgPrice = (double)additionalPrice;
			AdditionalWeight = additionalWeight;
		}

		public ShippingPrice(ShippingPrice sourcePrice): this(sourcePrice.Weight, (decimal)sourcePrice.Price, sourcePrice.AdditionalWeight, (decimal)sourcePrice.AdditionalOneKgPrice) { }

		public double Weight { get; set; }
		public double Price { get; set; }
		public double AdditionalOneKgPrice { get; set; }

	    private double _additionalWeight = 1;
	    public double AdditionalWeight
	    {
			get => _additionalWeight;
			set => _additionalWeight = value;
	    }

        public bool TryGetPrice(double itemWeight, out decimal price)
		{
			if (itemWeight <= Weight)
			{
				price = (decimal)Price;
				return true;
			}

			if ((decimal)AdditionalOneKgPrice >= 0.001m && AdditionalWeight > 0)
			{
				var overWeights = Convert.ToDecimal(Math.Ceiling((itemWeight - Weight) / AdditionalWeight));
				price = (decimal)Price + overWeights * (decimal)AdditionalOneKgPrice;
				return true;
			}

			price = int.MaxValue;
			return false;
		}

	    public bool IsDefault => Weight < double.Epsilon && Price < double.Epsilon && AdditionalOneKgPrice < double.Epsilon && AdditionalWeight < double.Epsilon;

		public override string ToString()
		{
			return string.Format("weight: {0} price: {1} +weight: {2} +price: {3}", Weight, Price, AdditionalWeight, AdditionalOneKgPrice);
		}
	}

    [JsonObject]
	public class ShippingPrices : IEnumerable<ShippingPrice>
	{
		public ShippingPrices()
		{
			Prices = new List<ShippingPrice>();
		}

		public ShippingPrices(ShippingPrices priceList)
		{
			if (priceList == null) throw new ArgumentNullException(nameof(priceList));
			Prices = new List<ShippingPrice>();

			foreach (var sourcePrice in priceList.Prices)
			{
				var target = new ShippingPrice(sourcePrice);
				Prices.Add(target);
			}
		}

		public List<ShippingPrice> Prices { get; set; }
	    public double MaximumWeight { get; set; }
	    public bool IsAvailableWeightConstrain => MaximumWeight > 0;
        public bool IsFree => Prices.All(p => p.IsDefault) || Prices.Count==0;
        public void Add(double weight, decimal price, double additionalWeight, decimal additionalPrice)
		{
			Prices.Add(new ShippingPrice(weight, price, additionalWeight, additionalPrice));
			//Prices.Sort((a, b) => a.Weight - b.Weight > 0 ? 0 : 1);
		}

		public IEnumerator<ShippingPrice> GetEnumerator()
		{
			return Prices.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
    }
}