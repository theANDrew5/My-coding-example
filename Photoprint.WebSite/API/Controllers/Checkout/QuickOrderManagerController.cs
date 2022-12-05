using Photoprint.Core.Models;
using Photoprint.Core.Services;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Photoprint.WebSite.API.Controllers.Checkout
{
    public class QuickOrderManagerController : BaseApiController
    {
        private readonly IPhotolabService _photolabService;
        private readonly ILanguageService _languageService;
        public QuickOrderManagerController(IAuthenticationService authenticationService, IPhotolabService photolabService, ILanguageService languageService)
            : base(authenticationService)
        {
            _photolabService = photolabService;
            _languageService = languageService;
        }
                
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "GET")]
        [Route("api/quickOrderManager/getResources")]
        public HttpResponseMessage GetResources(int photolabId, int? languageId = null)
        {
            var photolab = _photolabService.GetById(photolabId);
            if (photolab == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Photolab not found");
            
            Language language = null;
            if (languageId.HasValue)
            {
                language = _languageService.GetById(languageId.GetValueOrDefault());
            }
            if (language == null) 
            {
                language = photolab.DefaultLanguage; 
            }
            if (language == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Language not found");
            
            var result = new GetResourcesResponse
            {
                ButtonText = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.ButtonText, photolab, language),
                UploaderButtonText = RM.GetString(RS.Order.Case.AttachFiles, photolab, language),
                Mask = photolab.Properties?.PhoneMask ?? RM.GetString(RS.Shop.QuickOrderManager.ManagerData.Mask, photolab, language),
                Agreement = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.Agreement, photolab, language),
                Title = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.Title, photolab, language),
                Labels = new GetResourcesResponse.LabelGroup
                {
                    Fullname = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.Labels.Fullname, photolab, language),
                    Phone = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.Labels.Phone, photolab, language),
                    Email = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.Labels.Email, photolab, language),
                    Description = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.Labels.Description, photolab, language),
                    Uploader = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.Labels.Uploader, photolab, language),
                    OrderDetails = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.Labels.OrderDetails, photolab, language),                   
                },
                Placeholders = new GetResourcesResponse.PlaceholderGroup
                {
                    Fullname = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.Placeholders.Fullname, photolab, language),
                    Email = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.Placeholders.Email, photolab, language),
                    Description = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.Placeholders.Description, photolab, language)
                },
                SuccessMessage = new GetResourcesResponse.SuccessMessageGroup
                {
                    Title = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.SuccessMessage.Title, photolab, language),
                    OrderNumberText = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.SuccessMessage.OrderNumberText, photolab, language),
                    FeedBackText = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.SuccessMessage.FeedBackText, photolab, language)
                },
                
                Validation = new GetResourcesResponse.ValidationGroup
                {
                    Fullname = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.Validation.Fullname, photolab, language),
                    Phone = new GetResourcesResponse.ValidationGroup.UseCaseGroup
                    {
                        Error = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.Validation.Phone.Error, photolab, language),
                        Unavailable = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.Validation.Phone.Unavailable, photolab, language)
                    },
                    Email = new GetResourcesResponse.ValidationGroup.UseCaseGroup
                    {
                        Empty = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.Validation.Email.Empty, photolab, language),
                        Error = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.Validation.Email.Error, photolab, language),
                        Unavailable = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.Validation.Email.Unavailable, photolab, language)
                    },
                    Description = RM.GetString(RS.Shop.QuickOrderManager.ManagerData.Validation.Description, photolab, language),
                    Error = RM.GetString(RS.Order.QuickOrder.CreateError, photolab, language)
                }
            };

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        private class GetResourcesResponse
        {
            public string ButtonText { get; set; }
            public string UploaderButtonText { get; set; }
            public string Mask { get; set; }
            public string Agreement { get; set; }
            public string Title { get; set; }
            public LabelGroup Labels { get; set; }
            public PlaceholderGroup Placeholders { get; set; }
            public ValidationGroup Validation { get; set; }
            public SuccessMessageGroup SuccessMessage { get; set; }

            public class LabelGroup
            {
                public string Fullname { get; set; }
                public string Phone { get; set; }
                public string Email { get; set; }
                public string Description { get; set; }
                public string Uploader { get; set; }
                public string OrderDetails { get; set; }
            }

            public class PlaceholderGroup
            {
                public string Fullname { get; set; }
                public string Email { get; set; }
                public string Description { get; set; }
            }
            public class SuccessMessageGroup
            {
                public string Title { get; set; }
                public string OrderNumberText { get; set; }
                public string FeedBackText { get; set; }
            }
            public class ValidationGroup
            {
                public string Fullname { get; set; }
                public UseCaseGroup Phone { get; set; }
                public UseCaseGroup Email { get; set; }
                public string Description { get; set; }
                public string Error { get; set; }
                public class UseCaseGroup 
                {
                    public string Empty { get; set; }
                    public string Error { get; set; }
                    public string Unavailable { get; set; }
                }
            }
        }


        private static readonly string _bundleUrl = FileHelper.JS.QuickOrderManagerUrl;

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "GET")]
        [Route("api/quickOrderManager/getBundle")]
        public HttpResponseMessage GetQuickOrderManagerBundle()
        {
            var result = new GetQuickOrderManagerResponse
            {
                BundleUrl = _bundleUrl
            };

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        private class GetQuickOrderManagerResponse
        {
            public string BundleUrl { get; set; }
        }
    }
}