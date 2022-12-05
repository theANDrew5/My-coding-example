using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Photoprint.Core;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using Photoprint.WebSite.Shared;

namespace Photoprint.WebSite.API
{
    public class DDeliveryV2ShippingController : BaseApiController
    {
        private const string _postalIdTag = "postalId";
        private const string _languageIdTag = "languageId";
        private const string _urlTag = "url";
        private const string _weightTag = "totalWeight";
        private const string _productPriceTag = "data.products[0].price_declared";
        private const string _cityToTag = "data.city_to";
        private const string _cityIdTag = "data.city_id";
        private const string _streetTag = "data.street";
        private const string _houseTag = "data.house";
        private const string _orderTrackIdTag = "data.order_id";
        private const string _phoneTag = "data.number";

        private const string _calcJsonAddress = "calculator.json";
        private const string _indexJsonAddress = "index-by-address.json";
        private const string _infoJsonAddress = "info.json";
        private const string _phoneJson = "phone.json";

        private const string _keyReplaceText = ":key";

        private readonly IFrontendShippingService _frontendShippingService;

        public DDeliveryV2ShippingController(IAuthenticationService authenticationService, IFrontendShippingService frontendShippingService) : base(authenticationService)
        {
            _frontendShippingService = frontendShippingService;
        }

        [HttpGet]
        [Route("api/shippings/ddeliveryV2")]
        public HttpResponseMessage DDeliveryV2Init()
        {
            var request = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, y => y.Value);
            var postalId = 0; var url = string.Empty;

            if (request.ContainsKey(_postalIdTag)) int.TryParse(request[_postalIdTag], out postalId);
            if (request.ContainsKey(_urlTag)) url = request[_urlTag];

            if (postalId <= 0 || string.IsNullOrEmpty(url)) return new HttpResponseMessage(HttpStatusCode.OK);
            
            var postal = _frontendShippingService.GetById<Postal>(postalId);
            if (!(postal.ServiceProviderSettings is DDeliveryV2ServiceProviderSettings ddeliveryV2Settings)) return new HttpResponseMessage(HttpStatusCode.OK);
            
            var widgetKey = ddeliveryV2Settings.WidgetKey;
            var shopId = ddeliveryV2Settings.ShopId;
            if (string.IsNullOrEmpty(widgetKey) || string.IsNullOrEmpty(shopId)) return new HttpResponseMessage(HttpStatusCode.OK);
            var parsedUrl = url;

            var keys = request.Keys.Where(x => x.Contains("data"));
            var count = 0;
            foreach (var reqkey in keys)
            {
                parsedUrl += count == 0 ? $"?{reqkey.Replace("data.", string.Empty)}={request[reqkey]}" : $"&{reqkey.Replace("data.", string.Empty)}={request[reqkey].Replace(" ", "%20")}";
                count++;
            }
            

            using (var client = new HttpClient())
            {
                if (parsedUrl.Contains("html"))
                {
                    var response = client.GetAsync(parsedUrl).Result;
                    return response;

                }
                var ip = SiteUtils.GetIpAddress(new HttpContextWrapper(HttpContext.Current).Request);
                parsedUrl += count == 0 ? $"?ip={ip}" : $"&ip={ip}"; 
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {widgetKey}");
                client.DefaultRequestHeaders.Add("shop-id", shopId);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                
                
                var settings = client.GetAsync(parsedUrl).Result;
                var result = new
                {
                    status = (int)settings.StatusCode,
                    data = settings.Content.ReadAsAsync<object>().Result
                };
                settings.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(result));
                return settings;

            }
        }
        [HttpPost]
        [Route("api/shippings/ddeliveryV2")]
        public HttpResponseMessage DDeliveryV2Init([FromBody] dynamic request)
        {
            var photolabRequest = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, y => y.Value);
            var postalId = 0;
            if (photolabRequest.ContainsKey(_postalIdTag)) int.TryParse(photolabRequest[_postalIdTag], out postalId);

            var postal = _frontendShippingService.GetById<Postal>(postalId);
            if (!(postal.ServiceProviderSettings is DDeliveryV2ServiceProviderSettings ddeliveryV2Settings)) return new HttpResponseMessage(HttpStatusCode.OK);
            var widgetKey = ddeliveryV2Settings.WidgetKey;
            var shopId = ddeliveryV2Settings.ShopId;
            if (string.IsNullOrEmpty(widgetKey) || string.IsNullOrEmpty(shopId)) return new HttpResponseMessage(HttpStatusCode.OK);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {widgetKey}");
                client.DefaultRequestHeaders.Add("shop-id", shopId);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                var url = request["url"].ToString();
                var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(request["data"]), System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                JObject data;
                try
                {
                    data = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                }
                catch (Exception)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = new
                    {
                        status = (int)response.StatusCode,
                        data
                    };
                    response.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(result));
                    return response;
                }
                else
                {
                    var result = new
                    {
                        status = (int)response.StatusCode,
                        code = data["code"]?.Value<int>()
                    };
                    response.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(result));
                    return response;
                }
                
            }
                
        }
        
    }
}