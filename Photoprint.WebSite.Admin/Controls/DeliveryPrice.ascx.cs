using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using Photoprint.Core;
using Photoprint.Core.Models;

namespace Photoprint.WebSite.Admin.Controls
{
	public partial class DeliveryPrice : BaseControl
	{
		private ShippingPrices _priceList;

	    public bool IsVisibleTitle { get; set; } = true;

	    protected DeliveryPrice()
		{
			Load += DeliveryPriceLoad;
		}

		private void DeliveryPriceLoad(object sender, EventArgs e)
		{
			if (_priceList == null) throw new ArgumentException("You should call InitControl method first");
		}

		protected List<ShippingPrice> FixedPriceList { get; set; }
		protected ShippingPrice OverWeightPrice { get; set; }
        protected double MaximumWeight { get; set; }

        public void InitControl(ShippingPrices prices)
		{
			if (prices == null)
			{
				prices = new ShippingPrices();
			}
			_priceList = prices;

            var orderedPriceList = (from p in _priceList
                orderby p.Weight ascending, p.Price ascending
                select p).ToList();

            FixedPriceList = _priceList.Prices.Count > 1
			                 	? orderedPriceList.Take(orderedPriceList.Count - 1).ToList()
			                 	: orderedPriceList;

			OverWeightPrice = _priceList.Prices.Count == 0 ? null : orderedPriceList.Last();

            MaximumWeight = prices.MaximumWeight;

            if (!Page.IsPostBack)
            {
                chkFree.Checked = _priceList.IsFree;
            }
        }

		public ShippingPrices GetShippingPrices()
        {
        	var prices = new ShippingPrices();

			var fixedWeightRulesCount = (from key in Request.Form.AllKeys
			                                  where key.StartsWith("txtWeight")
			                                  select key).Count();

			if (!chkFree.Checked)
            {
                for (var i = 0; i < fixedWeightRulesCount; i++)
                {
                    var weightKey = "txtWeight" + i;
                    var priceKey = "txtPrice" + i;

                    var weight = Request.Form[weightKey].TryParseToDouble()
                        .MetricToPhotolabUnits(WebSiteGlobal.CurrentFrontend, MeasurementType.Weight);
                    var price = Request.Form[priceKey].TryParseToDecimal();

                    if (weight > 0.000001)
                    {
                        prices.Add(weight, price, 0, 0);
                    }
                }

                if (!prices.Any())
                    prices.Add(0, 0, 0, 0);

                var overWeight = (from p in prices select p.Weight).Max();
                var overPrice = Convert.ToDecimal((from p in prices select p.Price).Max());
			
                var overWeightFixedPrice = Request.Form["overWeightFixedPrice"].TryParseToDecimal();
                var overWeightAdditionalWeight = Request.Form["overWeightAdditionalWeight"].TryParseToDouble().MetricToPhotolabUnits(WebSiteGlobal.CurrentFrontend, MeasurementType.Weight);
                var overWeightFloatPrice = Request.Form["overWeightFloatPrice"].TryParseToDecimal();

                overWeightFixedPrice = overWeightFloatPrice >= 0.0001m ? overPrice : overWeightFixedPrice;

                prices.Add(overWeight, overWeightFixedPrice, overWeightAdditionalWeight, overWeightFloatPrice);
            }
            
            var isConstrainMaxWeight = !Request.Form["isConstrainMaxWeight"].IsEmpty();
		    if (isConstrainMaxWeight)
		    {		      
		        prices.MaximumWeight = Request.Form["txtMaxWeightConstrain"].TryParseToDouble();
		    }
            return prices;
        }
    }
}