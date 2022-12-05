using System.Net;
using System.Net.Http;
using System.Web.Http;
using Photoprint.Core.Services;

namespace Photoprint.WebSite.Admin.API.Controllers
{
    public class UkrposhtaController : BaseApiController
    {
        private readonly IUkrposhtaServices _ukrposhtaServices;
        public UkrposhtaController(IAuthenticationService authenticationService, IUkrposhtaServices ukrposhtaServices) : base(authenticationService)
        {
            _ukrposhtaServices = ukrposhtaServices;
        }

        [HttpGet]
        [Route("api/shippings/ukrposhta/regions")]
        public HttpResponseMessage GetRegions(string stateId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _ukrposhtaServices.GetRegions(stateId));
        }
        
        [HttpGet]
        [Route("api/shippings/ukrposhta/cities")]
        public HttpResponseMessage GetCities(string stateId, string regionId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _ukrposhtaServices.GetCities(stateId, regionId));
        }
        
        [HttpGet]
        [Route("api/shippings/ukrposhta/streets")]
        public HttpResponseMessage GetAddresses(string stateId, string regionId, string cityId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _ukrposhtaServices.GetStreets(stateId, regionId, cityId));
        }
    }
}