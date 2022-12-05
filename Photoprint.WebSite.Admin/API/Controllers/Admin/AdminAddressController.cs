using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Photoprint.Core;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using Photoprint.WebSite.Admin.API.Models;

namespace Photoprint.WebSite.Admin.API
{
    public class AdminAddressController:  BaseApiController
    {
        private readonly IPhotolabService _photolabService;
        private readonly ISuggestFactory _suggestFactory;
        private readonly ICityAddressFactory _cityAddressFactory;
        private readonly IAddressFactory _addressFactory;
        public AdminAddressController(
            IAuthenticationService authenticationService,
            IPhotolabService photolabService,
            ISuggestFactory suggestFactory,
            ICityAddressFactory cityAddressFactory,
            IAddressFactory addressFactory) : base(authenticationService)
        {
            _photolabService = photolabService;
            _suggestFactory = suggestFactory;
            _cityAddressFactory = cityAddressFactory;
            _addressFactory = addressFactory;
        }

        [HttpPost]
        [Route("api/admin/address/suggestCity")]
        public HttpResponseMessage GetSuggestedCity(AddressRequest<CitySuggestData> request)
        {
            var (isSuccess, responseMessage) = TryGetRequestParams(request, out var reqParams);
            if (!isSuccess) return responseMessage;

            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _suggestFactory.GetCitySuggests(reqParams.Language, request.Data, reqParams.Photolab));
            }
            catch (PhotoprintSystemException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpPost]
        [Route("api/admin/address/cityInfo")]
        public HttpResponseMessage GetCitiesInfo(AddressRequest<CityInfoData> request)
        {
            var (isSuccess, responseMessage) = TryGetRequestParams(request, out var reqParams);
            if (!isSuccess) return responseMessage;

            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _cityAddressFactory.GetCityAddresses(reqParams.Language, request.Data, reqParams.Photolab));
            }
            catch (PhotoprintSystemException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpPost]
        [Route("api/admin/address/suggestAddress")]
        public HttpResponseMessage GetSuggestedAddress(AddressRequest<SuggestAddressData> request)
        {
            var (isSuccess, responseMessage) = TryGetRequestParams(request, out var reqParams);
            if (!isSuccess) return responseMessage;

            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _suggestFactory.GetAddressSuggests(reqParams.Language, request.Data, reqParams.Photolab));
            }
            catch (PhotoprintSystemException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpPost]
        [Route("api/admin/address/addressInfo")]
        public HttpResponseMessage GetAddressInfo(AddressRequest<AddressInfoData> request)
        {
            var (isSuccess, responseMessage) = TryGetRequestParams(request, out var reqParams);
            if (!isSuccess) return responseMessage;

            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _addressFactory.GetAddress(reqParams.Language, request.Data, reqParams.Photolab));
            }
            catch (PhotoprintSystemException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        private (bool IsSuccess, HttpResponseMessage Request) TryGetRequestParams(IAddressRequest request,
            out RequestParams reqParams)
        {
            reqParams = new RequestParams();

            if (request is null || !request.HasData)
                return (false, Request.CreateErrorResponse(HttpStatusCode.BadRequest, ExceptionPhrase.BadRequest));
            
            reqParams.Photolab = _photolabService.GetById(request.FrontendId);
            if (reqParams.Photolab is null)
                return (false, Request.CreateErrorResponse(HttpStatusCode.BadRequest, ExceptionPhrase.PhotolabNotFound));

            reqParams.Language = (GeneralLanguageType)(request.LanguageId ?? 2) == GeneralLanguageType.Russian
                ? SystemLanguage.Russian : SystemLanguage.English;
            return (true, null);
        }

        private class RequestParams
        {
            public Photolab Photolab { get; set; }
            public SystemLanguage Language { get; set; }
        }
    }
}