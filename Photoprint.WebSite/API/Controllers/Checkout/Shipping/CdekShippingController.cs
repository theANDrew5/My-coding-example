using Newtonsoft.Json.Linq;
using NLog;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using Photoprint.WebSite.API.Models.Delivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Xml;
using Photoprint.Core;

namespace Photoprint.WebSite.API
{
    public class CdekShippingController : BaseApiController
    {
        private const string _shippingIdTag = "postalId";
        private const string _photolabIdTag = "photolabId";
        private const string _actionTag = "isdek_action";
        private const string _countryTag = "country";
        private const string _cityTag = "city";
        private const string _cityIdTag = "cityId";

        private const string _getPvzAction = "getPVZ";
        private const string _getLangAction = "getLang";
        private const string _getCityAction = "getCity";
        private const string _getCityByIdAction = "getCityById";
        
        private readonly IPhotolabService _photolabService;
        private readonly IFrontendShippingService _frontendShippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICdekServices _cdekService;

        private readonly object _language = new
        {
            LANG = new
            {
                YOURCITY = "Ваш город",
                COURIER = "Курьер",
                PICKUP = "Самовывоз",
                TERM = "Срок",
                PRICE = "Стоимость",
                DAY = "дн.",
                RUB = "руб.",
                NODELIV = "Нет доставки",
                CITYSEATCH = "Поиск города",
                CITYSEARCH = "Поиск города",
                ALL = "Все",
                PVZ = "Пункты выдачи",
                MOSCOW = "Москва",
                RUSSIA = "Россия",
                COUNTING = "Идет расчет",

                NO_AVAIL = "Нет доступных способов доставки",
                CHOOSE_TYPE_AVAIL = "Выберите способ доставки",
                CHOOSE_OTHER_CITY = "Выберите другой населенный пункт",

                EST = "есть",

                L_ADDRESS = "Адрес пункта выдачи заказов",
                L_TIME = "Время работы",
                L_WAY = "Как к нам проехать",
                L_CHOOSE = "Выбрать",

                H_LIST = "Список пунктов выдачи заказов",
                H_PROFILE = "Способ доставки",
                H_CASH = "Расчет картой",
                H_DRESS = "С примеркой",
                H_SUPPORT = "Служба поддержки"
            }
        };

        public CdekShippingController(IAuthenticationService authenticationService,
                                      IFrontendShippingService frontendShippingService,
                                      IPhotolabService photolabService,
                                      IShoppingCartService shoppingCartService,
                                      ICdekServices cdekService) : base(authenticationService)
        {
            _frontendShippingService = frontendShippingService;
            _photolabService = photolabService;
            _shoppingCartService = shoppingCartService;
            _cdekService = cdekService;
        }

        [HttpGet]
        [Route("api/shippings/cdek")]
        public HttpResponseMessage CdekAction()
        {
            var request = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, y => y.Value);
            var action = string.Empty;
            Shipping shipping = null;
            if (request.ContainsKey(_shippingIdTag) && int.TryParse(request[_shippingIdTag], out var shippingId))
                shipping = _frontendShippingService.GetById<Shipping>(shippingId);
            else
            {
                if (!request.ContainsKey(_photolabIdTag) || !int.TryParse(request[_photolabIdTag], out var photolabId))
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Must prowide postal or phptplab ID");
                var photolab = _photolabService.GetById(photolabId);
                shipping = _frontendShippingService.GetList<Shipping>(photolab)
                    .FirstOrDefault(s => s.ShippingServiceProviderType == ShippingServiceProviderType.Cdek);
            }

            if (shipping == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Postal not found");

            if (request.ContainsKey(_actionTag)) action = request[_actionTag];
            switch (action)
            {
                case _getPvzAction:
                    if (request.ContainsKey(_countryTag))
                    {
                        var result = _cdekService.GetPvzByCountry(shipping, request[_countryTag]);
                        return result != null
                            ? Request.CreateResponse(HttpStatusCode.OK, new { pvz = result })
                            : Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                    if (request.ContainsKey(_cityIdTag))
                    {
                        if (string.IsNullOrWhiteSpace(request[_cityIdTag]))
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        var result = _cdekService.GetPvzByCityId(shipping, request[_cityIdTag]);
                        return result != null
                            ? new HttpResponseMessage()
                            {
                                Content = new StringContent(result, Encoding.UTF8, "application/xml")
                            }
                            : Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                case _getLangAction:
                    return Request.CreateResponse(HttpStatusCode.OK, _language);
                case _getCityAction:
                {
                    if (!request.ContainsKey(_cityTag))
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    var city = _cdekService.GetCity(shipping, request[_cityTag]);
                    return city != null
                        ? Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            id = city.Id,
                            city = city.Title,
                            region = city.Region,
                            country = city.Country
                        })
                        : Request.CreateResponse(HttpStatusCode.NotFound);
                }
                case _getCityByIdAction:
                {
                    if (!request.ContainsKey(_cityIdTag))
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    var city = _cdekService.GetCityById(shipping, request[_cityIdTag]);
                    return city != null
                        ? Request.CreateResponse(HttpStatusCode.OK, city.Title) : Request.CreateResponse(HttpStatusCode.NotFound);
                }
                default:
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

#if DEBUG
        //Оставил для отладки
        private HttpResponseMessage GetPvz(string country) //Все точки
        {
            if (string.IsNullOrWhiteSpace(country)) return Request.CreateResponse(HttpStatusCode.OK);

            using (var client = new HttpClient())
            {
                var document = new XmlDocument();
                document.LoadXml(client.GetStringAsync("https://integration.cdek.ru" + "/pvzlist.php").Result);

                if (document.DocumentElement == null) return Request.CreateResponse(HttpStatusCode.OK);

                var resultNode = document.DocumentElement.SelectSingleNode("//PvzList");
                if (resultNode == null || !resultNode.HasChildNodes) return Request.CreateResponse(HttpStatusCode.OK);

                var result = new
                {
                    PVZ = new Dictionary<string, Dictionary<string, object>>(),
                    CITY = new Dictionary<string, string>(),
                    REGIONS = new Dictionary<string, string>(),
                    CITYFULL = new Dictionary<string, string>(),
                    COUNTRIES = Array.Empty<string>()
                };

                foreach (XmlNode warehouseNode in resultNode.ChildNodes)
                {
                    if (warehouseNode?.Attributes == null) continue;

                    if (country != "all" && country != warehouseNode.Attributes["CountryName"].Value)
                        continue;

                    var pvzCode = warehouseNode.Attributes["Code"].Value;
                    var cityCode = warehouseNode.Attributes["CityCode"].Value;
                    var cityAttribute = warehouseNode.Attributes["City"].Value;
                    if (cityAttribute.Contains('('))
                        cityAttribute = cityAttribute.Substring(0, cityAttribute.IndexOf('(')).Trim();
                    if (cityAttribute.Contains(','))
                        cityAttribute = cityAttribute.Substring(0, cityAttribute.IndexOf(',')).Trim();

                    object weightLim = null;
                    if (warehouseNode.HasChildNodes)
                    {
                        var weightAttrs = warehouseNode.SelectSingleNode("WeightLimit")?.Attributes;
                        if (weightAttrs != null)
                            weightLim = new
                            {
                                MIN = float.Parse(weightAttrs["WeightMin"]?.Value ?? throw new InvalidOperationException()),
                                MAX = float.Parse(weightAttrs["WeightMax"]?.Value ?? throw new InvalidOperationException())
                            };
                    }

                    var pvz = new
                    {
                        Name = warehouseNode.Attributes?["Name"]?.Value,
                        WorkTime = warehouseNode.Attributes?["WorkTime"]?.Value,
                        Address = warehouseNode.Attributes?["Address"]?.Value,
                        Phone = warehouseNode.Attributes?["Phone"]?.Value,
                        Note = warehouseNode.Attributes?["Note"]?.Value,
                        cX = warehouseNode.Attributes?["coordX"]?.Value,
                        cY = warehouseNode.Attributes?["coordY"]?.Value,
                        Dressing = warehouseNode.Attributes?["IsDressingRoom"]?.Value,
                        Cash = warehouseNode.Attributes?["HaveCashless"]?.Value,
                        Station = warehouseNode.Attributes?["NearestStation"]?.Value,
                        Site = warehouseNode.Attributes?["Site"]?.Value,
                        Metro = warehouseNode.Attributes?["MetroStation"]?.Value,
                        AddressComment = warehouseNode.Attributes?["AddressComment"]?.Value,
                        WeightLim = weightLim,
                        Picture = warehouseNode.SelectNodes("OfficeImage")?.Cast<XmlNode>().Select(x => x.Attributes?["url"].Value),
                        Path = warehouseNode.SelectSingleNode("OfficeHowGo")?.Attributes?["url"]?.Value
                    };

                    if (!result.PVZ.ContainsKey(cityCode))
                        result.PVZ.Add(cityCode, new Dictionary<string, object> { { pvzCode, pvz } });
                    else
                        result.PVZ[cityCode].Add(pvzCode, pvz);

                    if (!result.CITY.ContainsKey(cityCode))
                    {
                        result.CITY.Add(cityCode, warehouseNode.Attributes?["City"].Value);
                        result.CITYFULL.Add(cityCode, $"{warehouseNode.Attributes?["CountryName"].Value} {warehouseNode.Attributes?["RegionName"].Value} {cityAttribute}");
                        result.REGIONS.Add(cityCode, $"{warehouseNode.Attributes?["RegionName"].Value}, {warehouseNode.Attributes?["CountryName"].Value}");
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    pvz = result
                });
            }
        }  
#endif
        

    }
}