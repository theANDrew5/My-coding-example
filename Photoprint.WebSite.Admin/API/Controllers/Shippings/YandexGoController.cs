using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
namespace Photoprint.WebSite.Admin.API
{
    public class YandexGoController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IShippingService _shippingService;
        private readonly IYandexGoClient _yandexGoClient;
        private readonly IPhotolabService _photolabService;
        private readonly IShippingProviderResolverService _shippingProviderResolver;
        public YandexGoController(IAuthenticationService authenticationService,
                                  IOrderService orderService,
                                  IYandexGoClient yandexGoClient,
                                  IShippingService shippingService,
                                  IShippingProviderResolverService shippingProviderResolver,
                                  IPhotolabService photolabService) : base(authenticationService)
        {
            _orderService = orderService;
            _yandexGoClient = yandexGoClient;
            _shippingService = shippingService;
            _shippingProviderResolver = shippingProviderResolver;
            _photolabService = photolabService;
        }

        [HttpPost]
        [Route("api/shippings/yandexGo/registerOrder")]
        public HttpResponseMessage RegisterOrder(int? orderId)
        {
            if (AuthenticationService.LoggedInUser == null) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            if (!orderId.HasValue) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "orderId not found");
            try
            {
                var order = _orderService.GetById(orderId.Value);
                var photolab = _photolabService.GetById(order.PhotolabId);
                var shipping = _shippingService.GetById<Postal>(order.ShippingId);
                var yandexGoService = _shippingProviderResolver.GetProvider(shipping);

                yandexGoService.GetCreateOrderRegistration(photolab, order, shipping.ServiceProviderSettings);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/shippings/yandexGo/setTime")]
        public HttpResponseMessage SetTime(int? orderId, DateTime due)
        {
            if (AuthenticationService.LoggedInUser == null) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            if (due < DateTime.Now) return Request.CreateResponse(HttpStatusCode.BadRequest, RM.GetString("Shipping.YandexGo.ErrorCreateInPast"));
            if (due > DateTime.Now.AddDays(3)) return Request.CreateResponse(HttpStatusCode.BadRequest, RM.GetString("Shipping.YandexGo.ErrorCreateDelayed3Days"));
            if (!orderId.HasValue) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "orderId not found");
            var order = _orderService.GetById(orderId.Value);
            if (order == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");

            order.DeliveryAddress.DeliveryProperties.YandexGoOrderAddressInfo ??= new YandexGoOrderAddressInfo();
            order.DeliveryAddress.DeliveryProperties.YandexGoOrderAddressInfo.Due = due;
            _orderService.Update(order);
            return Request.CreateResponse(HttpStatusCode.OK, order.DeliveryAddress.DeliveryProperties.YandexGoOrderAddressInfo.Due.ToString("HH:mm dd.MM.yyyy"));
        }

        [HttpGet]
        [Route("api/shippings/yandexGo/acceptOrder")]
        public HttpResponseMessage AcceptOrder(int? orderId, string token)
        {
            if (AuthenticationService.LoggedInUser == null) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            if (!orderId.HasValue) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");
            if (string.IsNullOrEmpty(token)) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "token not found");

            var order = _orderService.GetById(orderId.Value);
            if (order?.Properties?.ShippingRegistrationResult is YandexGoRegistrationResult registrationResult)
            {
                var response = _yandexGoClient.ConfirmOrder(registrationResult.OrderId, registrationResult.Version, token, out var error);
                if (response == null) return Request.CreateResponse(HttpStatusCode.BadRequest, error.Message);

                _orderService.UpdateTrackingNumber(order, response.Id);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, "not found YandexGoRegistrationResult");
        }

        [HttpGet]
        [Route("api/shippings/yandexGo/getStatus")]
        public HttpResponseMessage GetStatus(int? orderId, string token)
        {
            if (AuthenticationService.LoggedInUser == null) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            if (!orderId.HasValue) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");

            var order = _orderService.GetById(orderId.Value);
            if (order?.Properties?.ShippingRegistrationResult is YandexGoRegistrationResult result)
            {
                var response = _yandexGoClient.GetOrderInfo(result.OrderId, token, out var error);
                if (response == null) return Request.CreateResponse(HttpStatusCode.BadRequest, error);
                var status = RM.GetString($"Shipping.YandexGo.{response.Status}");
                return Request.CreateResponse(HttpStatusCode.OK, new { status, response.AvailableCancelState });
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [Route("api/shippings/yandexGo/cancelOrder")]
        public HttpResponseMessage CancelOrder(int? orderId, string token)
        {
            if (AuthenticationService.LoggedInUser == null) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            if (!orderId.HasValue) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");
            try
            {
                var order = _orderService.GetById(orderId.Value);
                if (order.Properties.ShippingRegistrationResult is YandexGoRegistrationResult result)
                {
                    var response = _yandexGoClient.CancelOrder(result.OrderId, result.Version, token, out var error);
                    if (response == null) return Request.CreateResponse(HttpStatusCode.BadRequest, error.Message);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}