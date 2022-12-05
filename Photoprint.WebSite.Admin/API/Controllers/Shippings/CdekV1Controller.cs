using System.Net;
using System.Net.Http;
using System.Web.Http;
using Photoprint.Core.Models;
using Photoprint.Core.Services;

namespace Photoprint.WebSite.Admin.API.Controllers.Shippings
{
    public class CdekV1Controller: BaseApiController
    {
        private readonly ICdekServices _cdekServices;
        private readonly IShippingService _shippingService;
        public CdekV1Controller(IAuthenticationService authenticationService, IShippingProviderResolverService providerResolver, IShippingService shippingService) : base(authenticationService)
        {
            _cdekServices = providerResolver.GetProvider(ShippingServiceProviderType.Cdek) as ICdekServices;
            _shippingService = shippingService;
        }

        [HttpGet]
        [Route("api/shipppings/cdek/v1/getCityCode")]
        public HttpResponseMessage GetCityCode(int shippingId, string cityName)
        {
            var shipping = _shippingService.GetById<Shipping>(shippingId);
            if (shipping == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Shipping not found");

            if (string.IsNullOrWhiteSpace(cityName))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ExceptionPhrase.BadRequest);

            var city = _cdekServices.GetSendCity(shipping, cityName);

            return Request.CreateResponse(city?.Id ?? string.Empty);
        }
    }
}