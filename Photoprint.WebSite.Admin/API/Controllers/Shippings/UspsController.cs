using Photoprint.Core.Models;
using Photoprint.Core.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Photoprint.WebSite.Admin.API
{
    public class UspsController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IShippingService _shippingService;
        private readonly IPhotolabService _photolabService;
        private readonly IShippingProviderResolverService _shippingProviderResolver;
        public UspsController(IAuthenticationService authenticationService,
                              IOrderService orderService,
                              IShippingService shippingService,
                              IPhotolabService photolabService,
                              IShippingProviderResolverService shippingProviderResolver) : base(authenticationService)
        {
            _orderService = orderService;
            _shippingService = shippingService;
            _photolabService = photolabService;
            _shippingProviderResolver = shippingProviderResolver;
        }

        [HttpPost]
        [Route("api/shippings/usps/registerOrder")]
        public HttpResponseMessage RegisterOrder(int? orderId)
        {
            if (AuthenticationService.LoggedInUser == null) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            if (!orderId.HasValue) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "orderId not found");

            try
            {
                var order = _orderService.GetById(orderId.Value, true);
                var photolab = _photolabService.GetById(order.PhotolabId);
                var shipping = _shippingService.GetById<Postal>(order.ShippingId);
                var uspsService = _shippingProviderResolver.GetProvider(shipping);
                uspsService.GetCreateOrderRegistration(photolab, order, shipping.ServiceProviderSettings);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}