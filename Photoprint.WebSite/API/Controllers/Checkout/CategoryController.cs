using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Photoprint.Core.Services;

namespace Photoprint.WebSite.API.Controllers.Discounts
{
	public class CategoryController : BaseApiController
    {
		private readonly IPhotolabService _photolabService;
        private readonly ILanguageService _languageService;
		private readonly IFrontendMaterialTypeService _frontendMaterialTypeService;
		private readonly IFrontendMaterialService _frontendMaterialService;

        public CategoryController(IAuthenticationService authenticationService, IFrontendMaterialService frontendMaterialService, IPhotolabService photolabService,
			IFrontendMaterialTypeService frontendMaterialTypeService, ILanguageService languageService) 
			: base(authenticationService)
		{
			_photolabService = photolabService;
		    _languageService = languageService;
			_frontendMaterialTypeService = frontendMaterialTypeService;
            _frontendMaterialService = frontendMaterialService;
		}

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "GET")]
        [Route("api/category/getIdsByNames")]
        public HttpResponseMessage GetMaterialTypeAndMaterialNameIdsByName(int languageId, string materialTypeName, string materialName = null)
        {
            GetMaterialTypeAndMaterialNameIdsByNameResponse result = null;

            var language = _languageService.GetById(languageId);
            if (language == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "language not found");

            var photolab = _photolabService.GetById(language.PhotolabId.GetValueOrDefault());
            if (photolab == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "photolab not found");

            // проверяем material type url, если 0 - берем первую возможную категорию
            var materialTypeId = 0;
            if (!string.IsNullOrWhiteSpace(materialTypeName))
            {
                if (int.TryParse(materialTypeName, out var parsedMaterialTypeId))
                {
                    materialTypeId = parsedMaterialTypeId;
                }
                else
                {
                    materialTypeId = _frontendMaterialTypeService.GetByUrl(materialTypeName, language)?.Id ?? 0;
                }
            }

            if (materialTypeId == 0)
            {
                var materialTypes = _frontendMaterialTypeService.GetSmallList(photolab);
                if (materialTypes.Count == 0) return Request.CreateResponse(HttpStatusCode.BadRequest, "categories not found");

                materialTypeId = materialTypes.First().Id;
            }
            
            var materials = _frontendMaterialService.GetList(materialTypeId, language);
            if (materials.Count == 0) return Request.CreateResponse(HttpStatusCode.BadRequest, "materials in category not found");

            // проверяем material url, если 0 - берем первый возможный товар из категории
            var materialId = 0;
            if (!string.IsNullOrWhiteSpace(materialName))
            {
                if (int.TryParse(materialName, out var parsedMaterialId))
                {
                    materialId = parsedMaterialId;
                }
                else
                {
                    materialId = materials.FirstOrDefault(m => m.UrlName == materialName)?.Id ?? 0;
                }
            }

            if (materialId == 0)
            {
                materialId = materials.First().Id;
            }

            result = new GetMaterialTypeAndMaterialNameIdsByNameResponse
            {
                MaterialTypeId = materialTypeId,
                MaterialId = materialId
            };
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        private class GetMaterialTypeAndMaterialNameIdsByNameResponse
        {
            public int MaterialTypeId { get; set; }
            public int MaterialId { get; set; }
        }
    }
}
