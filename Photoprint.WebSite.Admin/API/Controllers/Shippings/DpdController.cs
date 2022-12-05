using Photoprint.Core.Models;
using Photoprint.Core.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Photoprint.WebSite.Admin.API
{
    public class DpdController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IShippingService _shippingService;
        private readonly IPhotolabService _photolabService;
        private readonly IDpdClient _dpdClient;
        private readonly IShippingProviderResolverService _shippingProviderResolver;


        public DpdController(IAuthenticationService authenticationService,
                             IOrderService orderService,
                             IShippingService shippingService,
                             IShippingProviderResolverService shippingProviderResolver,
                             IPhotolabService photolabService,
                             IDpdClient dpdClient) : base(authenticationService)
        {
            _orderService = orderService;

            _shippingService = shippingService;
            _shippingProviderResolver = shippingProviderResolver;
            _photolabService = photolabService;
            _dpdClient = dpdClient;
        }

        [HttpPost]
        [Route("api/shippings/dpd/registerOrder")]
        public HttpResponseMessage RegisterOrder(int? orderId)
        {
            if (AuthenticationService.LoggedInUser == null) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            if (!orderId.HasValue) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "orderId not found");
            try
            {
                var order = _orderService.GetById(orderId.Value, true);
                var photolab = _photolabService.GetById(order.PhotolabId);
                var shipping = _shippingService.GetById<Postal>(order.ShippingId);
                var dpdService = _shippingProviderResolver.GetProvider(shipping);
                dpdService.GetCreateOrderRegistration(photolab, order, shipping.ServiceProviderSettings);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/shippings/dpd/setDate")]
        public HttpResponseMessage SetDate(int? orderId, DateTime date)
        {
            if (AuthenticationService.LoggedInUser == null) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            if (date.Date < DateTime.Now.Date) return Request.CreateResponse(HttpStatusCode.BadRequest, RM.GetString("Shipping.YandexGo.ErrorCreateInPast"));
            if (!orderId.HasValue) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "orderId not found");

            var order = _orderService.GetById(orderId.Value);
            if (order == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");

            order.DeliveryAddress.DeliveryProperties.DpdAddressInfo.DatePickup = date;
            _orderService.UpdateProperties(order);

            return Request.CreateResponse(HttpStatusCode.OK, date.ToString("dd-MM-yyyy"));
        }

        [HttpPost]
        [Route("api/shippings/dpd/getOrderRegStatus")]
        public HttpResponseMessage GetOrderRegStatus(int? orderId, [FromBody] DpdAuth dpdAuth)
        {
            if (AuthenticationService.LoggedInUser == null) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            if (!orderId.HasValue) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");
            if (dpdAuth == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "dpdAuth not found");

            var response = _dpdClient.GetOrderRegStatus(orderId.ToString(), dpdAuth, out var errorMessage);
            if (response == null) return Request.CreateResponse(HttpStatusCode.BadRequest, errorMessage);
            response.ErrorMessage = string.IsNullOrEmpty(response.ErrorMessage) ? "" : response.ErrorMessage;
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = response.Status.ToString(), response.ErrorMessage });
        }

        [HttpPost]
        [Route("api/shippings/dpd/trackingOrder")]
        public HttpResponseMessage TrackingOrder(int? orderId, [FromBody] DpdAuth dpdAuth)
        {
            if (AuthenticationService.LoggedInUser == null) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            if (!orderId.HasValue) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");
            if (dpdAuth == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "dpdAuth not found");

            var response = _dpdClient.GetStatesByClientOrder(orderId.ToString(), dpdAuth, out var errorMessage);
            if (response == null) return Request.CreateResponse(HttpStatusCode.BadRequest, errorMessage);
            var builder = new StringBuilder();
            const string pattern = @"<li>Время перехода состояния {0}</li> 
                              <li>Состояние посылки после перехода {1}</li> 
                              <li>Код терминала DPD {2}</li>
                              <li>Город терминала DPD {3}</li> 
                              <li>Наименование инцидента {4}</li>";

            foreach (var state in response.States)
            {
                if (state == null) continue;
                builder.AppendFormat(pattern, state.TransitionTime, state.NewState, state.TerminalCode, state.TerminalCity, state.IncidentName);
                builder.AppendLine("<li>-------------------</li>");
            }
            return Request.CreateResponse(HttpStatusCode.OK, builder.ToString());
        }

        [HttpPost]
        [Route("api/shippings/dpd/cancelOrder")]
        public HttpResponseMessage CancelOrder(int? orderId, [FromBody] DpdAuth dpdAuth)
        {
            if (AuthenticationService.LoggedInUser == null) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            if (!orderId.HasValue) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");
            if (dpdAuth == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "dpdAuth not found");

            var response = _dpdClient.CancelOrder(orderId.ToString(), dpdAuth, out var errorMessage);
            if (response == null) return Request.CreateResponse(HttpStatusCode.BadRequest, errorMessage);
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Status = response.Status.ToString(),
                ErrorMessage = response.ErrorMessage ??= string.Empty
            });
        }
    }
}