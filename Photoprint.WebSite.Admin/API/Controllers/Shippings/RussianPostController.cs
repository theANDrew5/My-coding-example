using System.Net;
using System.Net.Http;
using System.Web.Http;
using JetBrains.Annotations;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using Photoprint.WebSite.Admin.API.Models;

namespace Photoprint.WebSite.Admin.API
{
    public class RussianPostController : BaseApiController
    {
        private readonly IRussianPostProviderService _service;
        private readonly IOrderService _orderService;
        private readonly IOrderAddressService _orderAddressService;
        private readonly IPhotolabService _photolabService;
        private readonly IShippingService _shippingService;
        public RussianPostController(
            IAuthenticationService authenticationService,
            IShippingProviderResolverService shippingProviderResolverService,
            IOrderService orderService,
            IOrderAddressService orderAddressService,
            IPhotolabService photolabService,
            IShippingService shippingService): base(authenticationService)
        {
            _service = shippingProviderResolverService.GetProvider(ShippingServiceProviderType.RussianPost) as IRussianPostProviderService;
            _orderService = orderService;
            _orderAddressService = orderAddressService;
            _photolabService = photolabService;
            _shippingService = shippingService;

        }

        public class OrderRequest
        {
            public int OrderId { get; set;}
            public UserPostOffice OrderPostal { get; set; }
        }
        public class OrderRegisterRespoce
        {
            public string ShippingRegistrationId { get; set;}
            public string TrackingNumber { get; set; }
        }
        [HttpPost]
        [Route("api/shippings/russianPost/registerOrder")]
        public HttpResponseMessage RegisterOrder(OrderRequest request)
        {
            if (request == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, RPResponseExceptionPhrase.BadRequest);
            }
            var order = _orderService.GetById(request.OrderId);
            if (order == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, RPResponseExceptionPhrase.OrderNotFound);
            }

            var photolab = _photolabService.GetById(order.PhotolabId);
            if (photolab == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, RPResponseExceptionPhrase.PhotolabNotFound);
            }

            var rpProps = order.DeliveryAddress.DeliveryProperties.RussianPostAddressInfo;
            if (rpProps is null || request.OrderPostal is null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, RPResponseExceptionPhrase.OrderRegisterError);
            }

            if (rpProps.PostalFrom is null)
            {
                rpProps.PostalFrom = request.OrderPostal;
                _orderAddressService.Update(order.DeliveryAddress);
            }

            var shipping = _shippingService.GetById<Postal>(order.ShippingId); 
            _service.GetCreateOrderRegistration(photolab ,order, shipping.ServiceProviderSettings);
            var id = (order.Properties.ShippingRegistrationResult as RussianPostRegistrationResult)?.ShippingRegistrationId;
            if (string.IsNullOrWhiteSpace(id))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    RPResponseExceptionPhrase.OrderRegisterError);
            var result = new OrderRegisterRespoce
            {
                ShippingRegistrationId = id,
                TrackingNumber = order.TrackingNumber
            };
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpDelete]
        [Route("api/shippings/russianPost/unRegisterOrder")]
        public HttpResponseMessage UnRegisterOrder(OrderRequest request)
        {
            if (request == null || request?.OrderId == null )
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, RPResponseExceptionPhrase.BadRequest);
            }
            var order = _orderService.GetById(request.OrderId);
            if (order == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, RPResponseExceptionPhrase.OrderNotFound);
            }

            var photolab = _photolabService.GetById(order.PhotolabId);
            if (photolab == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, RPResponseExceptionPhrase.PhotolabNotFound);
            }

            var shipping = _shippingService.GetById<Postal>(order.ShippingId); 
            _service.GetDeleteOrderRegistration(photolab ,order, shipping.ServiceProviderSettings);
            var id = (order.Properties.ShippingRegistrationResult as RussianPostRegistrationResult)?.ShippingRegistrationId;
            if (string.IsNullOrWhiteSpace(id))
                return Request.CreateResponse(HttpStatusCode.OK, id);
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, RPResponseExceptionPhrase.OrderUnRegisterError);
        }


        public class OrderStatusRespoce
        {
            public string CurrentOperation { get; set;}
            public string CurrentOperationAttr { get; set; }
        }
        [HttpGet]
        [Route("api/shippings/russianPost/shippingStatus")]
        public HttpResponseMessage GetShippingStatus(int orderId)
        {
            var order = _orderService.GetById(orderId);
            if (order == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, RPResponseExceptionPhrase.OrderNotFound);
            }

            var photolab = _photolabService.GetById(order.PhotolabId);
            if (photolab == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, RPResponseExceptionPhrase.PhotolabNotFound);
            }

            var shipping = _shippingService.GetById<Postal>(order.ShippingId); 
            var status = _service.GetOrderStatus(photolab ,order, shipping.ServiceProviderSettings);
            _orderService.SetStatus(order, status);
            var props = order.Properties.ShippingRegistrationResult as RussianPostRegistrationResult;
            props.CurrentOperation ??= "Отправление ещё не принято в транспортной компании";
            props.CurrentOperationAttr ??= "";
            _orderService.Update(order);
            
            var responce = new OrderStatusRespoce
            {
                CurrentOperation = props.CurrentOperation,
                CurrentOperationAttr = props.CurrentOperationAttr
            };
            return Request.CreateResponse(HttpStatusCode.OK, responce);
        }


        [HttpGet]
        [Route("api/shippings/russianPost/postals")]
        public HttpResponseMessage GetPostals(int orderId)
        {
            var order = _orderService.GetById(orderId);
            if (order == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, RPResponseExceptionPhrase.OrderNotFound);
            var photolab = _photolabService.GetById(order.PhotolabId);
            if (photolab == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    RPResponseExceptionPhrase.PhotolabNotFound);
            var shipping = _shippingService.GetById<Postal>(order.ShippingId); 
            if (shipping == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, RPResponseExceptionPhrase.BadRequest);
            var settings = shipping.ServiceProviderSettings as RussianPostServiceProviderSettings;
            var result = _service.GetUserPostOffices(shipping, settings);
            return result == null ? Request.CreateErrorResponse(HttpStatusCode.BadRequest, "") : Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}