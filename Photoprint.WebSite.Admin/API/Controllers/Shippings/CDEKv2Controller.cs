using AngleSharp.Io;
using Photoprint.Core;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace Photoprint.WebSite.Admin.API
{
    public class CDEKv2Controller : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IShippingService _shippingService;
        private readonly IPhotolabService _photolabService;
        private readonly IShippingProviderResolverService _shippingProviderResolver;
        private readonly ICdekV2ProviderService _cdekService;
        private const string _patternError = "<li style='color: red' ><b>Ошибка: {0}</b></li><li><b>{1}</b></li>";

        public CDEKv2Controller(IAuthenticationService authenticationService,
                             IOrderService orderService,
                             IShippingService shippingService,
                             IShippingProviderResolverService shippingProviderResolver,
                             IPhotolabService photolabService) : base(authenticationService)
        {
            _orderService = orderService;
            _shippingService = shippingService;
            _shippingProviderResolver = shippingProviderResolver;
            _photolabService = photolabService;
            _cdekService = (ICdekV2ProviderService)shippingProviderResolver.GetProvider(ShippingServiceProviderType.CDEKv2);
        }

        [HttpGet]
        [Route("api/shippings/cdek/register")]
        public HttpResponseMessage RegisterOrder(int? orderId)
        {
            if (AuthenticationService.LoggedInUser == null) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            if (!orderId.HasValue) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "orderId not found");
            try
            {
                var order = _orderService.GetById(orderId.Value, true);
                var photolab = _photolabService.GetById(order.PhotolabId);
                var shipping = _shippingService.GetById<Postal>(order.ShippingId);
                _cdekService.GetCreateOrderRegistration(photolab, order, shipping.ServiceProviderSettings);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Route("api/shippings/cdek/register")]
        public HttpResponseMessage CancelOrder(int? orderId)
        {
            if (AuthenticationService.LoggedInUser == null) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            if (!orderId.HasValue) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "orderId not found");
            try
            {
                var order = _orderService.GetById(orderId.Value, true);
                var photolab = _photolabService.GetById(order.PhotolabId);
                var shipping = _shippingService.GetById<Postal>(order.ShippingId);
                _cdekService.GetDeleteOrderRegistration(photolab, order, shipping.ServiceProviderSettings);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/shippings/cdek/orderInfo")]
        public HttpResponseMessage OrderInfo(int orderId)
        {
            if (AuthenticationService.LoggedInUser == null) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            var order = _orderService.GetById(orderId);
            if (order is null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");

            var info = _cdekService.GetOrderInfo(order);

            return !string.IsNullOrWhiteSpace(info.error)
                ? Request.CreateErrorResponse(HttpStatusCode.BadRequest, info.error)
                : Request.CreateResponse(HttpStatusCode.OK, info.status);
        }

        #region Calculate Delivery Price
        public class CalcRequestDto
        {
            public CDEKv2Auth CdekAuth { get; set; }
            public string PostalCodeA { get; set; }
            public string PostalCodeB { get; set; }
            public int Weight { get; set; }
            public int TariffCode { get; set; }

        }
        #endregion

        #region Order Barccode

        [HttpGet]
        [Route("api/shippings/cdek/formingBarcode")]
        public HttpResponseMessage FormingAnOrderBarcode(int orderId, string format, string lang)
        {
            if (AuthenticationService.LoggedInUser == null)
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            var order = _orderService.GetById(orderId);
            if (order is null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");

            var request = _cdekService.RequestFormingBarcode(order, format, lang);

            if (!string.IsNullOrWhiteSpace(request.error))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.error);

            var result = request.requestResult;
            return !string.IsNullOrWhiteSpace(result.uuid) 
                ? Request.CreateResponse(HttpStatusCode.OK, new {Info = result.info, Uuid = result.uuid}) 
                : Request.CreateResponse(HttpStatusCode.OK, result.info);
        }

        [HttpGet]
        [Route("api/shippings/cdek/getOrderBarcode")]
        public HttpResponseMessage GetOrderBarcode(int orderId, string uuid)
        {
            if (AuthenticationService.LoggedInUser == null)
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            var order = _orderService.GetById(orderId);
            if (order is null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");

            var request = _cdekService.GetFormedBarcode(order, uuid);

            if (!string.IsNullOrWhiteSpace(request.error))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.error);

            var result = request.requestResult;
            return !string.IsNullOrWhiteSpace(result.url) 
                ? Request.CreateResponse(HttpStatusCode.OK, new {Info = result.info, Url = result.url}) 
                : Request.CreateResponse(HttpStatusCode.OK, result.info);
        }
        #endregion

        #region Order Receipt
        [HttpGet]
        [Route("api/shippings/cdek/formingReceipt")]
        public HttpResponseMessage FormingAnOrderReceipt(int orderId)
        {
            if (AuthenticationService.LoggedInUser == null)
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            var order = _orderService.GetById(orderId);
            if (order is null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");

            var request = _cdekService.RequestFormingReceipt(order);

            if (!string.IsNullOrWhiteSpace(request.error))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.error);

            var result = request.requestResult;
            return !string.IsNullOrWhiteSpace(result.uuid) 
                ? Request.CreateResponse(HttpStatusCode.OK, new {Info = result.info, Uuid = result.uuid}) 
                : Request.CreateResponse(HttpStatusCode.OK, result.info);
        }

        [HttpGet]
        [Route("api/shippings/cdek/getOrderReceipt")]
        public HttpResponseMessage GetOrderReceipt(int orderId, string uuid)
        {
            if (AuthenticationService.LoggedInUser == null)
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            var order = _orderService.GetById(orderId);
            if (order is null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");

            var request = _cdekService.GetFormedReceipt(order, uuid);

            if (!string.IsNullOrWhiteSpace(request.error))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.error);

            var result = request.requestResult;
            return !string.IsNullOrWhiteSpace(result.url) 
                ? Request.CreateResponse(HttpStatusCode.OK, new {Info = result.info, Url = result.url}) 
                : Request.CreateResponse(HttpStatusCode.OK, new {Info = result.info});
        }
        #endregion

        [HttpGet]
        [Route("api/shippings/cdek/getPdf")]
        public HttpResponseMessage GetPdf(int orderId, string url)
        {
            if (AuthenticationService.LoggedInUser == null)
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            var order = _orderService.GetById(orderId);
            if (order is null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");

            using var stream = new MemoryStream();
            _cdekService.GetFile(order, url, stream, out var error);
            if (!string.IsNullOrWhiteSpace(error))
                Request.CreateErrorResponse(HttpStatusCode.BadRequest, error);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(stream.ToArray())
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return response;
        }

        #region Call Courier

        [HttpPost]
        [Route("api/shippings/cdek/intakes")]
        public HttpResponseMessage CallCourier(int orderId, [FromBody] CDEKv2CallCourier request)
        {
            if (AuthenticationService.LoggedInUser == null)
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            var order = _orderService.GetById(orderId);
            if (order is null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");

            var info = _cdekService.CallCourier(order, request);

            return !string.IsNullOrWhiteSpace(info.error)
                ? Request.CreateErrorResponse(HttpStatusCode.BadRequest, info.error)
                : Request.CreateResponse(HttpStatusCode.OK, info.requestResult);
        }

        [HttpDelete]
        [Route("api/shippings/cdek/intakes")]
        public HttpResponseMessage CancelCourier(int orderId)
        {
            if (AuthenticationService.LoggedInUser == null)
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            var order = _orderService.GetById(orderId);
            if (order is null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");

            var info = _cdekService.CancelCourier(order);

            return !string.IsNullOrWhiteSpace(info.error)
                ? Request.CreateErrorResponse(HttpStatusCode.BadRequest, info.error)
                : Request.CreateResponse(HttpStatusCode.OK, info.requestResult);
        }

        [HttpGet]
        [Route("api/shippings/cdek/intakes")]
        public HttpResponseMessage InfoCourier(int orderId)
        {
            if (AuthenticationService.LoggedInUser == null)
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            var order = _orderService.GetById(orderId);
            if (order is null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");

            var info = _cdekService.GetCourierInfo(order);

            return !string.IsNullOrWhiteSpace(info.error)
                ? Request.CreateErrorResponse(HttpStatusCode.BadRequest, info.error)
                : Request.CreateResponse(HttpStatusCode.OK, info.requestResult);
        }

        [HttpPost]
        [Route("api/shippings/cdek/validate")]
        public HttpResponseMessage ValidateCallCourier([FromBody] CDEKv2CallCourier callCourier)
        {
            if (AuthenticationService.LoggedInUser == null) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");

            if (DateTime.TryParse(callCourier.IntakeDate, out var date)
                && TimeSpan.TryParse(callCourier.IntakeTimeFrom, out var start)
                && TimeSpan.TryParse(callCourier.IntakeTimeTo, out var end))
            {
                if (date.Date < DateTime.Today)
                    return Request.CreateResponse(HttpStatusCode.OK, new { isValid = false, message = "Проверте дату ожидания курьера: заявка не может быть создана в прошлом" });
                if (date > DateTime.Now.AddYears(1))
                    return Request.CreateResponse(HttpStatusCode.OK, new { isValid = false, message = "Дата ожидания курьера не может быть больше текущей более чем на 1 год." });
                if (start == end)
                    return Request.CreateResponse(HttpStatusCode.OK, new { isValid = false, message = "Время \"начала ожидания\" и \"окончания ожидания\" не должны совпадать" });
                if (start > end)
                    return Request.CreateResponse(HttpStatusCode.OK, new { isValid = false, message = "Время \"начала ожидания\" должно быть раньше времени \"окончания ожидания\"" });

                return Request.CreateResponse(HttpStatusCode.OK, new { isValid = true });

            }

            return Request.CreateResponse(HttpStatusCode.OK, new { isValid = false, message = "Проверьте корректность ввода даты и времени" });
        }
        #endregion

        private StringBuilder NormalizeErrorFormat(CDEKv2Request[] requests)
        {
            var sb = new StringBuilder();

            foreach (var request in requests)
            {
                if (request == null) continue;
                foreach (var error in request.Errors)
                {
                    if (error == null) continue;
                    sb.AppendFormat(_patternError, error.Code, error.Message);
                }
            }
            return sb;
        }
    }
}