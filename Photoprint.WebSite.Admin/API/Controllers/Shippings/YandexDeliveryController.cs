using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using Photoprint.WebSite.Admin.API.Models;

namespace Photoprint.WebSite.Admin.API.Controllers.Shippings
{
    public class YandexDeliveryController : BaseApiController
    {
        private readonly IShippingService _shippingService;
        private readonly IShippingProviderResolverService _shippingProviderResolver;
        private readonly IYandexDeliveryService _service;
        public YandexDeliveryController(
            IAuthenticationService authenticationService,
            IShippingService shippingService,
            IShippingProviderResolverService shippingProviderResolver) : base(authenticationService)
        {
            _shippingService = shippingService;
            _shippingProviderResolver = shippingProviderResolver;
            _service = _shippingProviderResolver.GetProvider(ShippingServiceProviderType.YandexDelivery) as IYandexDeliveryService;
        }

        public class GeoIdRequest
        {
            public int ShippingId { get; set; }
            public Address CityAddress { get; set; }
        }

        [HttpPost]
        [Route("api/shippings/yandexDelivery/getAddressesByTerm")]
        public HttpResponseMessage GetGeoId(GeoIdRequest request)
        {
            var shipping = _shippingService.GetById<Postal>(request.ShippingId);
            if (shipping == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Shipping is null");
            if (request.CityAddress == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Address is null");
            var result = _service.GetAddressesByTerm(shipping, new CityAddress(request.CityAddress));
            return result != null
                ? Request.CreateResponse(HttpStatusCode.OK, new AddressHierarchyDto(result, true, true))
                : Request.CreateErrorResponse(HttpStatusCode.BadRequest, "GeoId not found");
        }

        public class ShippingsRequest
        {
            public int ShippingId { get; set; }
            public AddressRequestDTO SelectedAddressDto { get; set; }
        }

        [HttpPost]
        [Route("api/shippings/yandexDelivery/getDeliveryShippings")]
        public HttpResponseMessage GetDeliveryShippings(ShippingsRequest request)
        {
            var shipping = _shippingService.GetById<Postal>(request.ShippingId);
            if (shipping == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Shipping is null");
            var shippingAddress = new ShippingAddress
            {
                ShippingId = shipping.Id,
                Country = request.SelectedAddressDto.Country,
                Region = request.SelectedAddressDto.Region,
                City = request.SelectedAddressDto.City,
                Street = request.SelectedAddressDto.Street,
                House = request.SelectedAddressDto.House,
                PostalCode = request.SelectedAddressDto.PostalCode,
                DeliveryProperties =
                    JsonConvert.DeserializeObject<DeliveryAddressProperties>(request.SelectedAddressDto.DeliveryProperties.ToString())
            };
            var result = _service.GetDeliveryShippings(shipping, shippingAddress);
            return result != null
                ? Request.CreateResponse(HttpStatusCode.OK, result)
                : Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Shippings not found");
        }

        public class PickupPointsRequest
        {
            public int ShippingId { get; set; }
            public YandexDeliveryShipping DeliveryShipping { get; set; }
        }

        [HttpPost]
        [Route("api/shippings/yandexDelivery/pickpoints")]
        public HttpResponseMessage GetPickPoints(PickupPointsRequest request)
        {
            var shipping = _shippingService.GetById<Postal>(request.ShippingId);
            if (shipping == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Shipping is null");
            var result = _service.GetPickpointsList(shipping, request.DeliveryShipping);
            return result != null
                ? Request.CreateResponse(HttpStatusCode.OK, new AddressHierarchyDto(result, false, true))
                : Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Pickup points not found");
        }
    }
}