using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photoprint.Core;
using Photoprint.Core.Models;

namespace Photoprint.WebSite.Admin.Controls
{
    public partial class DDeliveryV2Selector : BaseShippingSelectorControl
    {
        private const decimal _maxPrice = 999999; // Максимальная цена для того, чтобы виджет иницилизировался
        protected bool PreparingIsSuccessful { get; set; } = true;

        protected Postal CurrentPostal { get; set; }
        protected double TotalWeight { get; set; }
        protected string ProductsListJson { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        private const string _orderTitle = "OrderInfo";
        private string GetProductsJSON(IReadOnlyCollection<IPurchasableItem> shippableItems)
        {
            try
            {
                if (shippableItems == null || shippableItems.Count == 0)
                {
                    PreparingIsSuccessful = false;
                    return string.Empty;
                };

                var summaryCount = shippableItems.Sum(x => x.Quantity);
                if (summaryCount <= 0)
                {
                    PreparingIsSuccessful = false;
                    return string.Empty;
                };

                var summaryPrice = shippableItems.Sum(x => x.Price);
                return JsonConvert.SerializeObject(new List<object>(1) { new
                    {
                        name = _orderTitle,
                        count = 1,
                        price = (summaryPrice <= _maxPrice ? summaryPrice : _maxPrice)
                    }
                });
            }
            catch
            {
                PreparingIsSuccessful = false;
            }
            return string.Empty;
        }


        public override void InitControl(Shipping shipping, Order order, IReadOnlyCollection<IPurchasedItem> items)
        {
            if (Page.IsPostBack) return;
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (shipping == null) throw new ArgumentNullException(nameof(shipping));
            if (items == null) throw new ArgumentNullException(nameof(items));

            var postal = shipping as Postal;
            if (postal == null) throw new PhotoprintSystemException("Shipping must be postal", string.Empty);

            CurrentPostal = postal;
            CurrentOrder = order;
            TotalWeight = items.Sum(x => x.TotalWeight);
            ProductsListJson = GetProductsJSON(items);
        }

        public override OrderAddress GeOrderAddress()
        {
            var result = Request.Form["saferouteWidgetResult"];
            if (string.IsNullOrWhiteSpace(result)) return null;

            var resultObj = JObject.Parse(result);
            
            var type = resultObj["delivery"]?["type"]?.Value<int>() ?? 0;
            var address = new OrderAddress
            {
                Country = "Россия",
                City = resultObj["city"]?["name"]?.Value<string>() ?? string.Empty,
                FirstName = resultObj["contacts"]["fullName"]?.Value<string>() ?? string.Empty,
                Phone = resultObj["contacts"]["phone"]?.Value<string>() ?? string.Empty,
                PostalCode = resultObj["contacts"]["address"]["zipCode"]?.Value<string>() ?? string.Empty,
                DeliveryProperties = new DeliveryAddressProperties
                {
                    DDeliveryV2AddressInfo = new DDeliveryV2AddressInfo
                    {
                        DeliveryType = type,
                        CityToId = resultObj["city"]?["id"]?.Value<int>() ?? 0,
                        CityToFias = resultObj["city"]?["fias"]?.Value<string>() ?? "",
                        CityToKladr = resultObj["city"]?["kladr"]?.Value<string>() ?? "",
                        StreetTo = resultObj["contacts"]?["address"]?["street"]?.Value<string>() ?? string.Empty,
                        HouseTo = resultObj["contacts"]?["address"]?["house"]?.Value<string>() ?? string.Empty,
                        FlatTo = resultObj["contacts"]?["address"]?["flat"]?.Value<string>() ?? string.Empty,
                        PriceCalcResult = new DDeliveryV2CalculatorResult()
                    }
                }
            };

            switch (type)
            {
                case 1: // ПВЗ
                    address.AddressLine1 = resultObj["delivery"]?["point"]?["address"]?.Value<string>() ?? string.Empty;
                    address.Description = resultObj["delivery"]?["point"]?["description"]?.Value<string>() ?? string.Empty;

                    if (resultObj["delivery"]?["is_my_delivery"]?.Value<bool>() ?? false)
                    {
                        int.TryParse(resultObj["delivery"]?["deliveryCompanyId"]?.Value<string>().Replace("my_", string.Empty), out var companyId);
                        int.TryParse(resultObj["delivery"]?["point"]?["id"]?.Value<string>().Replace("my_", string.Empty), out var pointId);
                        address.DeliveryProperties.DDeliveryV2AddressInfo.DeliveryCompanyId = companyId;
                        address.DeliveryProperties.DDeliveryV2AddressInfo.PointId = pointId;
                    }
                    else
                    {
                        address.DeliveryProperties.DDeliveryV2AddressInfo.DeliveryCompanyId = resultObj["delivery"]?["deliveryCompany_id"]?.Value<int>() ?? 0;
                        address.DeliveryProperties.DDeliveryV2AddressInfo.PointId = resultObj["delivery"]?["point"]?["id"]?.Value<int>() ?? 0;
                    }

                    address.DeliveryProperties.DDeliveryV2AddressInfo.PriceCalcResult.PriceDelivery =
                    address.DeliveryProperties.DDeliveryV2AddressInfo.PriceCalcResult.TotalPrice =
                        resultObj["delivery"]?["point"]?["price_delivery"]?.Value<decimal>() ?? 0m;
                    break;
                case 2: // Курьер
                case 3: // Почта
                    if (type == 2) address.AddressLine1 = ($"{address.DeliveryProperties.DDeliveryV2AddressInfo.StreetTo} {address.DeliveryProperties.DDeliveryV2AddressInfo.HouseTo} {address.DeliveryProperties.DDeliveryV2AddressInfo.FlatTo}").Trim();

                    if (resultObj["delivery"]?["is_my_delivery"]?.Value<bool>() ?? false)
                    {
                        int.TryParse(resultObj["delivery"]?["delivery_company_id"]?.Value<string>().Replace("my_", string.Empty), out var companyId);
                        address.DeliveryProperties.DDeliveryV2AddressInfo.DeliveryCompanyId = companyId;
                    }
                    else
                    {
                        address.DeliveryProperties.DDeliveryV2AddressInfo.DeliveryCompanyId = resultObj["delivery"]?["delivery_company_id"]?.Value<int>() ?? 0;
                    }

                    address.DeliveryProperties.DDeliveryV2AddressInfo.PriceCalcResult.PriceDelivery = resultObj["delivery"]?["price_delivery"]?.Value<decimal>() ?? 0m;
                    address.DeliveryProperties.DDeliveryV2AddressInfo.PriceCalcResult.PriceSorting = resultObj["delivery"]?["price_sorting"]?.Value<decimal>() ?? 0m;
                    address.DeliveryProperties.DDeliveryV2AddressInfo.PriceCalcResult.TotalPrice = resultObj["delivery"]?["total_price"]?.Value<decimal>() ?? 0m;
                    break;
                default: return null;
            }
            return address;
        }

        public override bool ValidateInput()
        {
            return !string.IsNullOrWhiteSpace(Request.Form["saferouteWidgetResult"]);
        }
    }
}