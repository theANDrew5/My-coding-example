using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photoprint.Core;
using Photoprint.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Photoprint.WebSite.Controls
{
    public partial class DDeliveryV2Selector : BaseShippingSelectorControl
    {
        private const decimal _maxPrice = 999999; // Максимальная цена для того, чтобы виджет иницилизировался
        protected bool PreparingIsSuccessful { get; set; } = true;

        protected Postal CurrentPostal { get; set; }
        protected double TotalWeight { get; set; }
        protected string ProductsListJson { get; set; }

        protected void Page_Load(object sender, EventArgs e) { }

        protected override void InitSelectedShipping(Shipping shipping)
        {
            _ = shipping ?? throw new ArgumentNullException(nameof(shipping));
            if (!(shipping is Postal postal)) throw new PhotoprintSystemException("Shipping must be postal", string.Empty);

            CurrentPostal = postal;
            TotalWeight = ShippableItems.Sum(x => x.TotalWeight);
            if (postal.ServiceProviderSettings is DDeliveryV2ServiceProviderSettings settings && TotalWeight < settings.DefaultWeight)
            {
                TotalWeight = settings.DefaultWeight;
            }

            ProductsListJson = GetProductsJSON();
        }

        private const string _orderTitle = "OrderInfo";
        private string GetProductsJSON()
        {
            try
            {
                if (ShippableItems?.Any() != true)
                {
                    PreparingIsSuccessful = false;
                    return string.Empty;
                }

                var summaryCount = ShippableItems.Sum(x => x.Quantity);
                if (summaryCount <= 0)
                {
                    PreparingIsSuccessful = false;
                    return string.Empty;
                }

                var summaryPrice = ShippableItems.Sum(x => x.Price);
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

        public override OrderAddress GetSelectedOrderAddress()
        {
            var result = Request.Form["saferouteWidgetResult"];
            if (string.IsNullOrWhiteSpace(result)) return null;

            var resultObj = JObject.Parse(result);
            if (!resultObj["delivery"].HasValues || !resultObj["city"].HasValues || !resultObj["contacts"].HasValues) return null;

            var type = resultObj["delivery"]?["type"]?.Value<int>() ?? 0;
            var nameSplit = Regex.Split(resultObj["city"]?["name"]?.Value<string>() ?? " ,  ", ", ");
            OrderAddress = new OrderAddress
            {
                Country = nameSplit.Length>1? nameSplit[0] : "Россия",
                City = nameSplit.Length>1? nameSplit[1] : nameSplit[0],
                FirstName = resultObj["contacts"]["fullName"]?.Value<string>() ?? string.Empty,
                Phone = resultObj["contacts"]["phone"]?.Value<string>() ?? string.Empty,
                PostalCode = resultObj["contacts"]["address"]["zipCode"]?.Value<string>() ?? string.Empty,
                DeliveryProperties = new DeliveryAddressProperties
                {
                    DDeliveryV2AddressInfo = new DDeliveryV2AddressInfo
                    {
                        DeliveryType = type,
                        CityToId = resultObj["city"]?["id"]?.Value<int>() ?? 0,
                        CityToFias = resultObj["city"]?["fias"]?.Value<string>() ?? string.Empty,
                        CityToKladr = resultObj["city"]?["kladr"]?.Value<string>() ?? string.Empty,
                        StreetTo = resultObj["contacts"]?["address"]?["street"]?.Value<string>() ?? string.Empty,
                        HouseTo = resultObj["contacts"]?["address"]?["building"]?.Value<string>() ?? string.Empty,
                        FlatTo = resultObj["contacts"]?["address"]?["apartment"]?.Value<string>() ?? string.Empty,
                        PriceCalcResult = new DDeliveryV2CalculatorResult()
                    }
                }
            };

            switch (type)
            {
                case 1: // ПВЗ
                    OrderAddress.AddressLine1 = resultObj["delivery"]?["point"]?["address"]?.Value<string>() ?? string.Empty;
                    OrderAddress.Description = resultObj["delivery"]?["point"]?["description"]?.Value<string>() ?? string.Empty;

                    if (resultObj["delivery"]?["is_my_delivery"]?.Value<bool>() ?? false)
                    {
                        int.TryParse(resultObj["delivery"]?["deliveryCompanyId"]?.Value<string>().Replace("my_", string.Empty), out var companyId);
                        int.TryParse(resultObj["delivery"]?["point"]?["id"]?.Value<string>().Replace("my_", string.Empty), out var pointId);
                        OrderAddress.DeliveryProperties.DDeliveryV2AddressInfo.DeliveryCompanyId = companyId;
                        OrderAddress.DeliveryProperties.DDeliveryV2AddressInfo.PointId = pointId;
                    }
                    else
                    {
                        OrderAddress.DeliveryProperties.DDeliveryV2AddressInfo.DeliveryCompanyId = resultObj["delivery"]?["delivery_company_id"]?.Value<int>() ?? 0;
                        OrderAddress.DeliveryProperties.DDeliveryV2AddressInfo.PointId = resultObj["delivery"]?["point"]?["id"]?.Value<int>() ?? 0;
                    }

                    OrderAddress.DeliveryProperties.DDeliveryV2AddressInfo.PriceCalcResult.PriceDelivery =
                    OrderAddress.DeliveryProperties.DDeliveryV2AddressInfo.PriceCalcResult.TotalPrice =
                         resultObj["delivery"]?["totalPrice"]?.Value<decimal>() ?? 0m;
                    break;
                case 2: // Курьер
                case 3: // Почта
                    if (type == 2) OrderAddress.AddressLine1 = ($"{OrderAddress.DeliveryProperties.DDeliveryV2AddressInfo.StreetTo} {OrderAddress.DeliveryProperties.DDeliveryV2AddressInfo.HouseTo} {OrderAddress.DeliveryProperties.DDeliveryV2AddressInfo.FlatTo}").Trim();

                    if (resultObj["delivery"]?["is_my_delivery"]?.Value<bool>() ?? false)
                    {
                        int.TryParse(resultObj["delivery"]?["deliveryCompanyId"]?.Value<string>().Replace("my_", string.Empty), out var companyId);
                        OrderAddress.DeliveryProperties.DDeliveryV2AddressInfo.DeliveryCompanyId = companyId;
                    }
                    else
                    {
                        OrderAddress.DeliveryProperties.DDeliveryV2AddressInfo.DeliveryCompanyId = resultObj["delivery"]?["deliveryCompanyId"]?.Value<int>() ?? 0;
                    }

                    OrderAddress.DeliveryProperties.DDeliveryV2AddressInfo.PriceCalcResult.PriceDelivery = resultObj["delivery"]?["totalPrice"]?.Value<decimal>() ?? 0m;
                    OrderAddress.DeliveryProperties.DDeliveryV2AddressInfo.PriceCalcResult.PriceSorting = resultObj["delivery"]?["price_sorting"]?.Value<decimal>() ?? 0m;
                    OrderAddress.DeliveryProperties.DDeliveryV2AddressInfo.PriceCalcResult.TotalPrice = resultObj["delivery"]?["totalPrice"]?.Value<decimal>() ?? 0m;
                    break;
                default: return null;
            }

            return OrderAddress;
        }

        public override bool ValidateInput() => !string.IsNullOrWhiteSpace(Request.Form["saferouteWidgetResult"]);
    }
}