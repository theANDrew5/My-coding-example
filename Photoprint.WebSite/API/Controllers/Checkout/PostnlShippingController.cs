using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Photoprint.Core.Models;
using Photoprint.Core.Services;

namespace Photoprint.WebSite.API
{
    public class PostnlShippingController : BaseApiController
    {
        private readonly IPostnlServices _postnlServices;
        private readonly IShippingService _shippingService;

        public PostnlShippingController(IAuthenticationService authenticationService, IPostnlServices postnlServices, IShippingService shippingService) : base(authenticationService)
        {
            _postnlServices = postnlServices;
            _shippingService = shippingService;
        }

        [HttpGet]
        [Route("api/shippings/postnl/getCountries")]
        public HttpResponseMessage GetCountries(int shippingId)
        {
            var postal = _shippingService.GetById<Postal>(shippingId);

            return postal?.ShippingServiceProviderType != ShippingServiceProviderType.Postnl
                ? Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid shipping id")
                : Request.CreateResponse(HttpStatusCode.OK, _postnlServices.GetCountries(postal.ServiceProviderSettings));
        }

        [HttpGet]
        [Route("api/shippings/postnl/getRegions")]
        public HttpResponseMessage GetRegions(int shippingId, string postalCode, string country)
        {
            var postal = _shippingService.GetById<Postal>(shippingId);

            try
            {
                return postal?.ShippingServiceProviderType != ShippingServiceProviderType.Postnl
                    ? Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid shipping id")
                    : Request.CreateResponse(HttpStatusCode.OK, _postnlServices.GetRegions(postal.ServiceProviderSettings, postalCode, country));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet]
        [Route("api/shippings/postnl/getCities")]
        public HttpResponseMessage GetCities(int shippingId, string postalCode, string country, string region = null)
        {
            var postal = _shippingService.GetById<Postal>(shippingId);

            try
            {
                return postal?.ShippingServiceProviderType != ShippingServiceProviderType.Postnl
                    ? Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid shipping id")
                    : Request.CreateResponse(HttpStatusCode.OK, _postnlServices.GetCities(postal.ServiceProviderSettings, postalCode, country, region));
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpGet]
        [Route("api/shippings/postnl/getStreets")]
        public HttpResponseMessage GetStreets(int shippingId, string postalCode, string country, string region, string city)
        {
            var postal = _shippingService.GetById<Postal>(shippingId);

            try
            {
                return postal?.ShippingServiceProviderType != ShippingServiceProviderType.Postnl
                    ? Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid shipping id")
                    : Request.CreateResponse(HttpStatusCode.OK, _postnlServices.GetStreets(postal.ServiceProviderSettings, postalCode, country, region, city));
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpGet]
        [Route("api/shippings/postnl/getHouses")]
        public HttpResponseMessage GetHouses(int shippingId, string postalCode, string country, string region, string city, string street)
        {
            var postal = _shippingService.GetById<Postal>(shippingId);

            try
            {
                return postal?.ShippingServiceProviderType != ShippingServiceProviderType.Postnl
                    ? Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid shipping id")
                    : Request.CreateResponse(HttpStatusCode.OK, _postnlServices.GetHouses(postal.ServiceProviderSettings, postalCode, country, region, city, street));
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpGet]
        [Route("api/shippings/postnl/getStorages")]
        public HttpResponseMessage GetStorages(int shippingId, string postalCode, string country, string city)
        {
            var postal = _shippingService.GetById<Postal>(shippingId);

            try
            {
                return postal?.ShippingServiceProviderType != ShippingServiceProviderType.Postnl
                    ? Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid shipping id")
                    : Request.CreateResponse(HttpStatusCode.OK, _postnlServices.GetStorages(postal.ServiceProviderSettings, postalCode, country, city));
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpGet]
        [Route("api/shippings/postnl/getStorage")]
        public HttpResponseMessage GetStorage(int shippingId, int locationCode)
        {
            var postal = _shippingService.GetById<Postal>(shippingId);

            try
            {
                return postal?.ShippingServiceProviderType != ShippingServiceProviderType.Postnl
                    ? Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid shipping id")
                    : Request.CreateResponse(HttpStatusCode.OK); //_postnlServices.GetStorages() )
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }
    }
}