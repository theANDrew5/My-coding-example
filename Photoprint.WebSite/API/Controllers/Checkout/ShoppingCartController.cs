using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Photoprint.Core;
using Photoprint.Core.Models;
using Photoprint.Core.Models.Poly1C;
using Photoprint.Core.Models.ASystem;
using Photoprint.Core.Models.CustomWorks;
using Photoprint.Core.Models.MaterialPrices;
using Photoprint.Core.Services;
using Photoprint.WebSite.Shared;


namespace Photoprint.WebSite.API.Controllers.Discounts
{
    public class ShoppingCartController : BaseApiController
    {
        private readonly IFrontendMaterialService _frontendMaterialService;
        private readonly IPhotolabService _photolabService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IFrontendTaxService _frontendTaxService;
        private readonly IFrontendShippingService _frontendShippingService;
        private readonly IFrontendMaterialTypeService _frontendMaterialTypeService;
        private readonly IAffiliateUserService _affiliateUserService;
        private readonly IUserService _userService;
        private readonly ILanguageService _languageService;
        private readonly IFrontendCustomWorkService _frontendCustomWorkService;
        private readonly IFrontendMaterialPriceService _frontendMaterialPriceService;
        private readonly IFrontendSuppliersService _frontendSuppliersService;
        private readonly IFrontendSuppliersCategoryService _frontendSuppliersCategoryService;
        private readonly IFrontendSuppliersSettingsService _frontendSuppliersSettingsService;

        public ShoppingCartController(IAuthenticationService authenticationService,
                                      IFrontendMaterialService frontendMaterialService,
                                      IPhotolabService photolabService,
                                      IShoppingCartService shoppingCartService,
                                      IFrontendTaxService frontendTaxService,
                                      IFrontendShippingService frontendShippingService,
                                      IFrontendMaterialTypeService frontendMaterialTypeService,
                                      IAffiliateUserService affiliateUserService,
                                      IUserService userService,
                                      ILanguageService languageService,
                                      IFrontendCustomWorkService frontendCustomWorkService,
                                      IFrontendSuppliersService frontendSuppliersService,
                                      IFrontendMaterialPriceService frontendMaterialPriceService,
                                      IFrontendSuppliersCategoryService frontendSuppliersCategoryService,
                                      IFrontendSuppliersSettingsService frontendSuppliersSettingsService)
            : base(authenticationService)
        {
            _frontendMaterialService = frontendMaterialService;
            _photolabService = photolabService;
            _shoppingCartService = shoppingCartService;
            _frontendTaxService = frontendTaxService;
            _frontendShippingService = frontendShippingService;
            _frontendMaterialTypeService = frontendMaterialTypeService;
            _affiliateUserService = affiliateUserService;
            _userService = userService;
            _frontendSuppliersCategoryService = frontendSuppliersCategoryService;
            _languageService = languageService;
            _frontendCustomWorkService = frontendCustomWorkService;

            _frontendSuppliersService = frontendSuppliersService;
            _frontendMaterialPriceService = frontendMaterialPriceService;
            _frontendSuppliersSettingsService = frontendSuppliersSettingsService;
        }

        private enum ApiCartOperationType
        {
            Create,
            Read,
            Update,
            Delete
        }
        private enum ApiCartResponseStatus
        {
            Success,
            Fail
        }

        public class TaxPriceRequest
        {
            public int FrontendId { get; set; }
            public string Items { get; set; }
            public decimal DeliveryPrice { get; set; }
            public int? ShippingId { get; set; }
            public int? AddressId { get; set; }
            public string ZipCode { get; set; }
        }
        [HttpPost]
        [Route("api/cart/tax")]
        public HttpResponseMessage GetTaxPrice(TaxPriceRequest request)
        {
            if (request != null)
            {
                var lab = _photolabService.GetById(request.FrontendId);
                if (lab != null)
                {
                    var items = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<ShoppingCartItemSmall>>(request.Items);
                    var shipping = request.ShippingId != null ? _frontendShippingService.GetById<Shipping>(request.ShippingId.Value) : null;
                    var tax = _frontendTaxService.GetSalesTaxPrice(items, request.DeliveryPrice, lab, shipping, request.ZipCode);
                    return Request.CreateResponse(HttpStatusCode.OK, tax);
                }
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad request");
        }

        [HttpPost]
        [HttpOptions]
        [EnableCors(origins: "*", headers: "*", methods: "POST, OPTIONS")]
        [Route("api/cart/price")]
        public HttpResponseMessage GetPrice(MaterialPriceRequest request)
        {
            // ! Примечание по методу !
            // Данный метод существует для того, чтобы расчитать стоимость товара с выбранными настройками
            // для калькулятора и редактора на основе существующей корзины. То есть, когда вы уже создаете заказ,
            // то этот метод вообще не используется, имейте это в ввиду!

            if (request == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "request not found");

            var material = _frontendMaterialService.GetById(request.MaterialId);
            var photolab = material != null ? _photolabService.GetById(material.PhotolabId) : null;
            if (material == null || photolab == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "material or photolab not found");
            }

            var materialPrice = _frontendMaterialPriceService.GetPrice(photolab, material, request);
            if (materialPrice == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "shopping cart item not found");
            }

            return Request.CreateResponse(HttpStatusCode.OK, materialPrice);
        }

        [HttpGet, HttpOptions, Route("api/cart/itemsCount")]
        [EnableCors("*", "*", "GET")]
        public int GetItemsCount(int frontendId = 0)
        {
            if (WebSiteGlobal.LoggedInUser == null) return 0;

            var photolab = frontendId > 0 ? _photolabService.GetById(frontendId) : WebSiteGlobal.CurrentPhotolab;
            return _shoppingCartService.GetCart(photolab)?.Items?.Count(i => i.IsCanBeOrdered) ?? 0;
        }


        public class CreateCartItemRequest
        {
            public int LanguageId { get; set; }
            public int MaterialId { get; set; }
            public int Quantity { get; set; }
            public int PartsQuantity { get; set; }
            public bool IsEditable { get; set; }
            public IReadOnlyList<int> MaterialTypesIds { get; set; }
            public IReadOnlyList<CustomWorkItemState> CustomWorkItems { get; set; }
        }
        public class CreateASystemCartItemRequest
        {
            public int LanguageId { get; set; }
            public int MaterialId { get; set; }
            public decimal TotalPrice { get; set; }
            public ASystemTemplate ASystemTemplate { get; set; }
        }
        public class CreatePoly1CCartItemRequest
        {
            public int LanguageId { get; set; }
            public int MaterialId { get; set; }
            public decimal TotalPrice { get; set; }
            public Poly1CCalculatedState PolyState { get; set; }
            public int Quantity { get; set; }
        }
        public class CreateAxiomCartItemRequest
        {
            public int LanguageId { get; set; }
            public int MaterialId { get; set; }
            public decimal TotalPrice { get; set; }
            public AxiomTemplate AxiomTemplate { get; set; }
        }
        public class CreateHelloPrintCartItemRequest
        {
            public int LanguageId { get; set; }
            public int MaterialId { get; set; }
            public decimal TotalPrice { get; set; }
            public HelloPrintTemplate HelloPrintTemplate { get; set; }
        }
        [HttpPost]
        [Route("api/cart/createItem")]
        public HttpResponseMessage CreateCartItem(CreateCartItemRequest cartItem)
        {
            var lang = _languageService.GetById(cartItem.LanguageId);
            var photolab = lang == null ? WebSiteGlobal.CurrentPhotolab : _photolabService.GetById(lang.PhotolabId.GetValueOrDefault());

            var cart = GetCart(photolab);
            var item = GetShoppingCartItem(cartItem, photolab, lang);
            if (item == null || cart == null)
                return Response(ApiCartOperationType.Create, ApiCartResponseStatus.Fail, "Item not found");
            if (WebSiteGlobal.LoggedInUser == null || WebSiteGlobal.LoggedInUser.Id != (cart.UserOwner?.Id ?? 0))
                return Response(ApiCartOperationType.Create, ApiCartResponseStatus.Fail, "Logged user is not valid");
            cart.Items.Add(item);

            _shoppingCartService.SaveCart(cart);

            return Response(ApiCartOperationType.Create, ApiCartResponseStatus.Success, null, cart);
        }
        [HttpPost]
        [Route("api/cart/createASystemItem")]
        public HttpResponseMessage CreateASystemCartItem(CreateASystemCartItemRequest cartItem)
        {
            var lang = _languageService.GetById(cartItem.LanguageId);
            var photolab = lang == null ? WebSiteGlobal.CurrentPhotolab : _photolabService.GetById(lang.PhotolabId.GetValueOrDefault());

            var cart = GetCart(photolab);
            var item = GetASystemShoppingCartItem(cartItem, photolab, lang);
            if (item == null || cart == null)
                return Response(ApiCartOperationType.Create, ApiCartResponseStatus.Fail, "Item not found");
            if (WebSiteGlobal.LoggedInUser == null || WebSiteGlobal.LoggedInUser.Id != (cart.UserOwner?.Id ?? 0))
                return Response(ApiCartOperationType.Create, ApiCartResponseStatus.Fail, "Logged user is not valid");
            cart.Items.Add(item);

            _shoppingCartService.SaveCart(cart);

            return Response(ApiCartOperationType.Create, ApiCartResponseStatus.Success, null, cart);
        }
        [HttpPost]
        [Route("api/cart/createPoly1CItem")]
        public HttpResponseMessage CreatePoly1CCartItem(CreatePoly1CCartItemRequest cartItem)
        {
            var lang = _languageService.GetById(cartItem.LanguageId);
            var photolab = lang == null ? WebSiteGlobal.CurrentPhotolab : _photolabService.GetById(lang.PhotolabId.GetValueOrDefault());

            var cart = GetCart(photolab);
            var item = GetPoly1CShoppingCartItem(cartItem, photolab, lang);
            if (item == null || cart == null)
                return Response(ApiCartOperationType.Create, ApiCartResponseStatus.Fail, "Item not found");
            if (WebSiteGlobal.LoggedInUser == null || WebSiteGlobal.LoggedInUser.Id != (cart.UserOwner?.Id ?? 0))
                return Response(ApiCartOperationType.Create, ApiCartResponseStatus.Fail, "Logged user is not valid");
            cart.Items.Add(item);

            _shoppingCartService.SaveCart(cart);

            return Response(ApiCartOperationType.Create, ApiCartResponseStatus.Success, null, cart);
        }
        [HttpPost]
        [Route("api/cart/createAxiomItem")]
        public HttpResponseMessage CreateHelloPrintCartItem(CreateAxiomCartItemRequest cartItem)
        {
            var lang = _languageService.GetById(cartItem.LanguageId);
            var photolab = lang == null ? WebSiteGlobal.CurrentPhotolab : _photolabService.GetById(lang.PhotolabId.GetValueOrDefault());
            var cart = GetCart(photolab);
            var item = GetAxiomShoppingCartItem(cartItem, photolab, lang);
            if (item == null || cart == null)
                return Response(ApiCartOperationType.Create, ApiCartResponseStatus.Fail, "Item not found");
            if (WebSiteGlobal.LoggedInUser == null || WebSiteGlobal.LoggedInUser.Id != (cart.UserOwner?.Id ?? 0))
                return Response(ApiCartOperationType.Create, ApiCartResponseStatus.Fail, "Logged user is not valid");
            cart.Items.Add(item);
            _shoppingCartService.SaveCart(cart);
            return Response(ApiCartOperationType.Create, ApiCartResponseStatus.Success, null, cart);
        }
        [HttpPost]
        [Route("api/cart/createHelloPrintItem")]
        public HttpResponseMessage CreateHelloPrintCartItem(CreateHelloPrintCartItemRequest cartItem)
        {
            var lang = _languageService.GetById(cartItem.LanguageId);
            var photolab = lang == null ? WebSiteGlobal.CurrentPhotolab : _photolabService.GetById(lang.PhotolabId.GetValueOrDefault());
            var cart = GetCart(photolab);
            var item = GetHelloPrintShoppingCartItem(cartItem, photolab, lang);
            if (item == null || cart == null)
                return Response(ApiCartOperationType.Create, ApiCartResponseStatus.Fail, "Item not found");
            if (WebSiteGlobal.LoggedInUser == null || WebSiteGlobal.LoggedInUser.Id != (cart.UserOwner?.Id ?? 0))
                return Response(ApiCartOperationType.Create, ApiCartResponseStatus.Fail, "Logged user is not valid");
            cart.Items.Add(item);
            _shoppingCartService.SaveCart(cart);
            return Response(ApiCartOperationType.Create, ApiCartResponseStatus.Success, null, cart);
        }
        private ShoppingCartItem GetShoppingCartItem(CreateCartItemRequest request, Photolab photolab, Language language)
        {

            var materialType = _frontendMaterialTypeService.GetByMaterialId(request.MaterialId, language);
            var material = _frontendMaterialService.GetById(request.MaterialId, language);
            if (material == null) return null;
            var circulation = material.CirculationSettings.GetCirculationByQuantity(request.Quantity);
            circulation.ItemPartsCount = material.CirculationSettings.IsComplexProduct ? request.PartsQuantity : 1;

            var quantityIsChangable = materialType.IsQuantityChangable || materialType.IsCartQuantityChangable;
            var materialShoppingCartItem = new MaterialShoppingCartItem(materialType, material, circulation, null, quantityIsChangable, request.MaterialTypesIds)
            {
                IsCreatedByApi = true
            };

            var additionalPriceItems = _frontendCustomWorkService.GetAdditionalPriceItemByState(request.CustomWorkItems.AsList(), new ProductInfoData(material), photolab, materialShoppingCartItem, out _, out var estimatedItemProductionTimeInHours);
            materialShoppingCartItem.Properties.AdditionalPriceItems.AddRange(additionalPriceItems);
            _frontendCustomWorkService.RecalculateAdditionalPriceItems(materialShoppingCartItem);

            materialShoppingCartItem.Properties.AdditionalPriceItemsIsProcessed = true;
            materialShoppingCartItem.Properties.EstimatedItemProductionTimeInHours = estimatedItemProductionTimeInHours;
            materialShoppingCartItem.Properties.CirculationSettings = material.Properties.CirculationSettings;
            materialShoppingCartItem.Properties.IsEditable = request.IsEditable;
            materialShoppingCartItem.Properties.BitrixProductId = material.BitrixId;
            materialShoppingCartItem.Properties.RoundItemPriceToIntegers = photolab.Properties.RoundProductPriceToIntegers;
            switch (material.EditorType)
            {
                case EditorType.Photobooks:
                    materialShoppingCartItem.Properties["photobookTitle"] = materialShoppingCartItem.Title;
                    materialShoppingCartItem.Properties["photobookDescription"] = string.Empty;
                    materialShoppingCartItem.Properties["photobookPagesCount"] = request.PartsQuantity.ToString();
                    break;
                case EditorType.Mockups:
                    materialShoppingCartItem.Properties.MockupEditorResult = new MockupEditorResult { Files = new List<MockupEditorFileResult>() };
                    break;
                case EditorType.PhotoEditor:
                    materialShoppingCartItem.Properties.PhotoEditorResult = new PhotoEditorResult { prints = new List<PhotoEditorPrintInfo>() };
                    break;
                case EditorType.PrintOnDemandSolutions:
                    materialShoppingCartItem.Properties.PrintOnDemandOrderResult = new Core.Models.Editors.PrintOnDemandOrderResult();
                    break;
                case EditorType.BookOnDemand:
                    materialShoppingCartItem.Properties.BookOnDemandEditorResult = new BookOnDemandEditorResult();
                    break;
            }

            return materialShoppingCartItem;
        }
        private ShoppingCartItem GetASystemShoppingCartItem(CreateASystemCartItemRequest request, Photolab photolab, Language language)
        {
            if (request.ASystemTemplate == null) return null;
            var materialType = _frontendMaterialTypeService.GetByMaterialId(request.MaterialId, language);
            var material = _frontendMaterialService.GetById(request.MaterialId, language);
            if (material == null) return null;


            var circulation = new Circulation(request.ASystemTemplate.Quantity, request.TotalPrice);

            var properties = new ShoppingCartItemProperty();
            properties.ASystemTemplateState = request.ASystemTemplate;
            var materialShoppingCartItem = new MaterialShoppingCartItem(materialType, material, circulation, properties)
            {
                IsCreatedByApi = true


            };

            materialShoppingCartItem.Properties.IsEditable = false;
            materialShoppingCartItem.Properties.BitrixProductId = material.BitrixId;
            materialShoppingCartItem.Properties.RoundItemPriceToIntegers = photolab.Properties.RoundProductPriceToIntegers;
            switch (material.EditorType)
            {
                case EditorType.Photobooks:
                    materialShoppingCartItem.Properties["photobookTitle"] = materialShoppingCartItem.Title;
                    materialShoppingCartItem.Properties["photobookDescription"] = string.Empty;
                    materialShoppingCartItem.Properties["photobookPagesCount"] = "0";
                    break;
                case EditorType.Mockups:
                    materialShoppingCartItem.Properties.MockupEditorResult = new MockupEditorResult { Files = new List<MockupEditorFileResult>() };
                    break;
                case EditorType.PhotoEditor:
                    materialShoppingCartItem.Properties.PhotoEditorResult = new PhotoEditorResult { prints = new List<PhotoEditorPrintInfo>() };
                    break;
                case EditorType.PrintOnDemandSolutions:
                    materialShoppingCartItem.Properties.PrintOnDemandOrderResult = new Core.Models.Editors.PrintOnDemandOrderResult();
                    break;
                case EditorType.BookOnDemand:
                    materialShoppingCartItem.Properties.BookOnDemandEditorResult = new BookOnDemandEditorResult();
                    break;
            }

            return materialShoppingCartItem;
        }
        private ShoppingCartItem GetPoly1CShoppingCartItem(CreatePoly1CCartItemRequest request, Photolab photolab, Language language)
        {
            if (request.PolyState == null) return null;
            var materialType = _frontendMaterialTypeService.GetByMaterialId(request.MaterialId, language);
            var material = _frontendMaterialService.GetById(request.MaterialId, language);
            if (material == null) return null;
            var circulation = new Circulation(request.Quantity, request.TotalPrice);

            var properties = new ShoppingCartItemProperty();
            properties.Poly1CCalculatedState = request.PolyState;
            var materialShoppingCartItem = new MaterialShoppingCartItem(materialType, material, circulation, properties, false)
            {
                IsCreatedByApi = true

            };


            materialShoppingCartItem.Properties.IsEditable = false;
            materialShoppingCartItem.Properties.BitrixProductId = material.BitrixId;
            materialShoppingCartItem.Properties.RoundItemPriceToIntegers = photolab.Properties.RoundProductPriceToIntegers;
            switch (material.EditorType)
            {
                case EditorType.Photobooks:
                    materialShoppingCartItem.Properties["photobookTitle"] = materialShoppingCartItem.Title;
                    materialShoppingCartItem.Properties["photobookDescription"] = string.Empty;
                    materialShoppingCartItem.Properties["photobookPagesCount"] = "0";
                    break;
                case EditorType.Mockups:
                    materialShoppingCartItem.Properties.MockupEditorResult = new MockupEditorResult { Files = new List<MockupEditorFileResult>() };
                    break;
                case EditorType.PhotoEditor:
                    materialShoppingCartItem.Properties.PhotoEditorResult = new PhotoEditorResult { prints = new List<PhotoEditorPrintInfo>() };
                    break;
                case EditorType.PrintOnDemandSolutions:
                    materialShoppingCartItem.Properties.PrintOnDemandOrderResult = new Core.Models.Editors.PrintOnDemandOrderResult();
                    break;
                case EditorType.BookOnDemand:
                    materialShoppingCartItem.Properties.BookOnDemandEditorResult = new BookOnDemandEditorResult();
                    break;
            }

            return materialShoppingCartItem;
        }
        private ShoppingCartItem GetHelloPrintShoppingCartItem(CreateHelloPrintCartItemRequest request, Photolab photolab, Language language)
        {
            if (request.HelloPrintTemplate == null) return null;
            var materialType = _frontendMaterialTypeService.GetByMaterialId(request.MaterialId, language);
            var material = _frontendMaterialService.GetById(request.MaterialId, language);
            if (material == null) return null;


            var circulation = new Circulation(request.HelloPrintTemplate.Amount, request.TotalPrice);

            var properties = new ShoppingCartItemProperty
            {
                HelloPrintTemplateState = request.HelloPrintTemplate
            };
            var materialShoppingCartItem = new MaterialShoppingCartItem(materialType, material, circulation, properties)
            {
                IsCreatedByApi = true
            };

            materialShoppingCartItem.Properties.IsEditable = false;
            materialShoppingCartItem.Properties.BitrixProductId = material.BitrixId;
            materialShoppingCartItem.Properties.RoundItemPriceToIntegers = photolab.Properties.RoundProductPriceToIntegers;
            switch (material.EditorType)
            {
                case EditorType.Photobooks:
                    materialShoppingCartItem.Properties["photobookTitle"] = materialShoppingCartItem.Title;
                    materialShoppingCartItem.Properties["photobookDescription"] = string.Empty;
                    materialShoppingCartItem.Properties["photobookPagesCount"] = "0";
                    break;
                case EditorType.Mockups:
                    materialShoppingCartItem.Properties.MockupEditorResult = new MockupEditorResult { Files = new List<MockupEditorFileResult>() };
                    break;
                case EditorType.PhotoEditor:
                    materialShoppingCartItem.Properties.PhotoEditorResult = new PhotoEditorResult { prints = new List<PhotoEditorPrintInfo>() };
                    break;
                case EditorType.PrintOnDemandSolutions:
                    materialShoppingCartItem.Properties.PrintOnDemandOrderResult = new Core.Models.Editors.PrintOnDemandOrderResult();
                    break;
                case EditorType.BookOnDemand:
                    materialShoppingCartItem.Properties.BookOnDemandEditorResult = new BookOnDemandEditorResult();
                    break;
            }

            return materialShoppingCartItem;
        }
        private ShoppingCartItem GetAxiomShoppingCartItem(CreateAxiomCartItemRequest request, Photolab photolab, Language language)
        {
            if (request.AxiomTemplate == null) return null;
            var materialType = _frontendMaterialTypeService.GetByMaterialId(request.MaterialId, language);
            var material = _frontendMaterialService.GetById(request.MaterialId, language);
            if (material == null) return null;

            var circulation = new Circulation(request.AxiomTemplate.Tirazh.Default, request.TotalPrice);

            var properties = new ShoppingCartItemProperty();
            properties.AxiomTemplateState = request.AxiomTemplate;
            var materialShoppingCartItem = new MaterialShoppingCartItem(materialType, material, circulation, properties)
            {
                IsCreatedByApi = true
            };

            materialShoppingCartItem.Properties.IsEditable = false;
            materialShoppingCartItem.Properties.BitrixProductId = material.BitrixId;
            materialShoppingCartItem.Properties.RoundItemPriceToIntegers = photolab.Properties.RoundProductPriceToIntegers;
            switch (material.EditorType)
            {
                case EditorType.Photobooks:
                    materialShoppingCartItem.Properties["photobookTitle"] = materialShoppingCartItem.Title;
                    materialShoppingCartItem.Properties["photobookDescription"] = string.Empty;
                    materialShoppingCartItem.Properties["photobookPagesCount"] = "0";
                    break;
                case EditorType.Mockups:
                    materialShoppingCartItem.Properties.MockupEditorResult = new MockupEditorResult { Files = new List<MockupEditorFileResult>() };
                    break;
                case EditorType.PhotoEditor:
                    materialShoppingCartItem.Properties.PhotoEditorResult = new PhotoEditorResult { prints = new List<PhotoEditorPrintInfo>() };
                    break;
                case EditorType.PrintOnDemandSolutions:
                    materialShoppingCartItem.Properties.PrintOnDemandOrderResult = new Core.Models.Editors.PrintOnDemandOrderResult();
                    break;
                case EditorType.BookOnDemand:
                    materialShoppingCartItem.Properties.BookOnDemandEditorResult = new BookOnDemandEditorResult();
                    break;
            }

            return materialShoppingCartItem;
        }

        public class CartApiRequest
        {
            public int Id { get; set; }
            public int Count { get; set; }
            public int? PhotolabId { get; set; }
            public decimal Price { get; set; }
            public string Title { get; set; }
        }
        [HttpPost]
        [Route("api/cart/createCustomItem")]
        public HttpResponseMessage CreateCustomCartItem(CartApiRequest cartItem)
        {
            var operation = ApiCartOperationType.Create;
            var photolab = cartItem.PhotolabId == null ? null : _photolabService.GetById(cartItem.PhotolabId.GetValueOrDefault());
            var cart = GetCart(photolab);

            foreach (var shoppingCartItem in cart.Items)
            {
                if (shoppingCartItem.IsCustomItem && shoppingCartItem.Title == cartItem.Title)
                {
                    return Response(operation, ApiCartResponseStatus.Fail, "this item is already in basket");
                }
            }

            var item = new ShoppingCartItem(new CartApiCustomItem(cartItem), cartItem.Title, cartItem.Count, null, true)
            {
                IsCustomItem = true
            };

            cart.Items.Add(item);
            _shoppingCartService.SaveCart(cart);

            return Response(operation, ApiCartResponseStatus.Success);
        }
        private class CartApiCustomItem : IPurchasableItem
        {
            public double ItemWeight { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int Length { get; set; }
            public double TotalWeight { get; }
            public int Quantity { get; }
            public int ItemId { get; }
            public string Title { get; }
            public decimal Price { get; }
            public decimal AdditionalPrice { get; }
            public bool IsExportToPhotoexpertItem { get; }

            public CartApiCustomItem(CartApiRequest apiItem)
            {
                Title = apiItem.Title;
                Price = apiItem.Price;
                Quantity = apiItem.Count;
            }
        }
        public class ProposalItemDto
        {
            public int ItemId { get; set; }
            public int Quantity { get; set; }
            public string Title { get; set; }
            public decimal Price { get; set; }
            public decimal PrintPrice { get; set; }
            public string Description { get; set; }
            public string GFPrint { get; set; }
            public int? PhotolabId { get; set; }
        }


        public class GfShoppingCartItemDto
        {
            public int Id { get; set; }
            public int SubproductId { get; set; }
            public string Title { get; set; }
            public string ProductUrl { get; set; }
            public int Quantity { get; set; }
            public decimal ItemPrice { get; set; }
            public decimal Price { get; set; }
            public string PreviewUrl { get;set; }
        }
        public class GFShoppingCartDto
        {
            public GFShoppingCartDto()
            {
                Items = new List<GfShoppingCartItemDto>();
            }
            public List<GfShoppingCartItemDto> Items { get; set; }
            public decimal TotalPrice { get; set; }
            public decimal MinPrice { get; set; }
        }
        [HttpGet]
        [Route("api/cart/get")]
        public HttpResponseMessage GetCartItemList()
        {
            return Response(ApiCartOperationType.Read, ApiCartResponseStatus.Success);
        }

        [HttpPost]
        [Route("api/cart/update")]
        public HttpResponseMessage UpdateCartItem(CartApiRequest cartItem)
        {
            var operation = ApiCartOperationType.Update;
            var item = GetItemById(cartItem.Id, out ShoppingCart cart, cartItem.PhotolabId.GetValueOrDefault());
            if (item == null || cart == null)
                return Response(operation, ApiCartResponseStatus.Fail, "Item not found");
            if (WebSiteGlobal.LoggedInUser == null || WebSiteGlobal.LoggedInUser.Id != (cart.UserOwner?.Id ?? 0))
                return Response(operation, ApiCartResponseStatus.Fail, "Logged user is not valid");

            item.Quantity = cartItem.Count;
            _shoppingCartService.UpdateItem(item);

            return Response(operation, ApiCartResponseStatus.Success);
        }
        [HttpGet]
        [Route("api/cart/gfItems")]
        public HttpResponseMessage GetGfShoppingCartItems()
        {
            var result = new GFShoppingCartDto();
            var supplierSettings = _frontendSuppliersSettingsService.GetSuppliersSettings(WebSiteGlobal.CurrentPhotolab);
            result.MinPrice = supplierSettings?.MinPrice ?? 0;
            var cart = GetCart();
            if (cart == null) return Request.CreateResponse(HttpStatusCode.OK, result);
            var gfShoppingCartItems = cart.Items.Where(x => x is GFShoppingCartItem);
            if (gfShoppingCartItems.Count()>0)
            {
               
                result.TotalPrice = gfShoppingCartItems.Sum(x => x.Price);
                foreach (GFShoppingCartItem cartItem in gfShoppingCartItems)
                {
                    var item = new GfShoppingCartItemDto();
                    item.Title = cartItem.Title;
                    item.ItemPrice = cartItem.ItemPrice;
                    item.Price = cartItem.Price;
                    item.Quantity = cartItem.Quantity;
                    var itemValuePair = GetGFProductUrl(cartItem.GFProductId);
                    item.Id = cartItem.ItemId;
                    item.SubproductId = cartItem.GFProductId;
                    item.PreviewUrl = itemValuePair?.Item1 ?? string.Empty;
                    item.ProductUrl = itemValuePair?.Item2 ?? string.Empty;
                    result.Items.Add(item);

                }

            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        [Route("api/cart/updateProposalItem")]
        public HttpResponseMessage UpdateProposalItem(ProposalItemDto item)
        {
            var operation = ApiCartOperationType.Update;
            var cartItem = GetItemById(item.ItemId, out ShoppingCart cart, item.PhotolabId.GetValueOrDefault());
            if (cartItem == null || cart == null)
                return Response(operation, ApiCartResponseStatus.Fail, "Item not found");
            if (WebSiteGlobal.LoggedInUser == null || WebSiteGlobal.LoggedInUser.Id != (cart.UserOwner?.Id ?? 0))
                return Response(operation, ApiCartResponseStatus.Fail, "Logged user is not valid");
            cartItem.Name = item.Title;
            var gfPrint = 0;
            var prints = _frontendSuppliersService.GetPrintsByPhotolab(WebSiteGlobal.CurrentPhotolab);
            var printTitle = RM.GetString(RS.Suppliers.Project111.CartPrintType) + " " + RM.GetString(RS.Suppliers.Project111.NoPrint);
            if (prints != null & prints.Count > 0)
            {
                var selectedPrint = prints.FirstOrDefault(x => (x.Description + " (" + x.Title + ")").Equals(item.GFPrint));
                gfPrint = selectedPrint?.Id ?? 0;
                if (gfPrint>0)
                    printTitle = RM.GetString(RS.Suppliers.Project111.CartPrintType) + $"{selectedPrint.Description} ({selectedPrint.Title}) ({SiteUtils.GetPriceFormated(item.PrintPrice, WebSiteGlobal.CurrentPhotolab)})";
            }
            cartItem.AdditionalPriceItems.LastOrDefault().Title = printTitle;
            cartItem.ItemPrice = item.Price / item.Quantity;
            cartItem.AdditionalPrice = item.PrintPrice * item.Quantity;
            cartItem.Quantity = item.Quantity;
            cartItem.Properties.GFDescription = item.Description;
            _shoppingCartService.UpdateProposalItem(cartItem, gfPrint);
            return Response(operation, ApiCartResponseStatus.Success);
        }

        [HttpGet]
        [Route("api/cart/delete")]
        public HttpResponseMessage RemoveCartItem(int id, int photolabId)
        {
            var operation = ApiCartOperationType.Delete;
            var item = GetItemById(id, out ShoppingCart cart, photolabId);
            if (item == null || cart == null || WebSiteGlobal.LoggedInUser == null || WebSiteGlobal.LoggedInUser.Id != (cart.UserOwner?.Id ?? 0))
            {
                return Response(operation, ApiCartResponseStatus.Fail, "Item not found");
            }
            if (WebSiteGlobal.LoggedInUser == null || WebSiteGlobal.LoggedInUser.Id != (cart.UserOwner?.Id ?? 0))
            {
                return Response(operation, ApiCartResponseStatus.Fail, "Logged user is not valid");
            }

            cart.Items.Remove(item);
            _shoppingCartService.SaveCart(cart);
            
            return Response(operation, ApiCartResponseStatus.Success);
        }

        public class CartApiDeleteItemsRequest
        {
            public int PhotolabId { get; set; }
            public IReadOnlyCollection<int> ItemsIds { get; set; }
        }
        [HttpPost]
        [Route("api/cart/deleteItems")]
        public HttpResponseMessage RemoveCartItems(CartApiDeleteItemsRequest request)
        {
            var operation = ApiCartOperationType.Delete;
            if (request.ItemsIds == null || request.ItemsIds.Count == 0)
            {
                return Response(operation, ApiCartResponseStatus.Success);
            }

            var photolab = _photolabService.GetById(request.PhotolabId) ?? WebSiteGlobal.CurrentPhotolab;
            var cart = GetCart(photolab);
            if (cart == null || WebSiteGlobal.LoggedInUser == null || WebSiteGlobal.LoggedInUser.Id != (cart.UserOwner?.Id ?? 0))
            {
                return Response(operation, ApiCartResponseStatus.Fail, "Items not found");
            }
            if (WebSiteGlobal.LoggedInUser == null || WebSiteGlobal.LoggedInUser.Id != (cart.UserOwner?.Id ?? 0))
            {
                return Response(operation, ApiCartResponseStatus.Fail, "Logged user is not valid");
            }

            cart.Items.RemoveAll(t => request.ItemsIds.Contains(t.ItemId));
            _shoppingCartService.SaveCart(cart);            
            return Response(operation, ApiCartResponseStatus.Success);
        }

        [HttpPost]
        [Route("api/cart/cloneItem")]
        public HttpResponseMessage CloneCartItem(int itemId, int photolabId)
        {
            try
            {
                var item = GetItemById(itemId, out ShoppingCart cart, photolabId);
                if (item==null || cart == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Item not found");
                }
                if (WebSiteGlobal.LoggedInUser == null || WebSiteGlobal.LoggedInUser.Id != (cart.UserOwner?.Id ?? 0))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "Logged user is not valid");
                }

                _shoppingCartService.CloneItem(item, out var newItem);
                cart.Items.Add(newItem);
                _shoppingCartService.SaveCart(cart);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,e.Message);

            }
        }

        [HttpPost]
        [Route("api/cart/handleDiscountForCart")]
        public HttpResponseMessage HandleDiscountForCart(int discountId)
        {
            try
            {
                var user = AuthenticationService.LoggedInUser;

                if (user != null && !user.IsAnonymous)
                {
                    if (user.Properties.NextOrderForbiddenDiscountsIds.Contains(discountId))
                    {
                        user.Properties.NextOrderForbiddenDiscountsIds.Remove(discountId);
                    }
                    else
                    {
                        user.Properties.NextOrderForbiddenDiscountsIds.Add(discountId);
                    }
                    _userService.UpdateProperties(user);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        private ShoppingCart GetCart(Photolab photolab = null)
        {
            var user = WebSiteGlobal.LoggedInUser ?? CreateAnonymousUser();
            return _shoppingCartService.GetCart(user, photolab ?? WebSiteGlobal.CurrentPhotolab);
        }
        private User CreateAnonymousUser()
        {
            var context = GetHttpContext(Request);
            _ = context ?? throw new ArgumentNullException(nameof(context));

            var ip = SiteUtils.GetIpAddress(context.Request);
            var (gaClientId, yaClientId) = SiteUtils.GetWebAnalyticsClientId(context);
            var affiliateUser = _affiliateUserService.GetByCode(WebSiteGlobal.CurrentCompanyAccount, Common.GetAffiliateCode(context));

            var user = _userService.CreateAnonymous(WebSiteGlobal.CurrentCompanyAccount, ip, affiliateUser, gaClientId, yaClientId);
            AuthenticationService.SignIn(user, true, WebSiteGlobal.CurrentPhotolab);
            return user;
        }

        private ShoppingCartItem GetItemById(int id, out ShoppingCart cart, int photolabId = 0)
        {
            cart = photolabId > 0 ? GetCart(_photolabService.GetById(photolabId)) : GetCart();
            if (cart?.Items == null || cart.Items.Count == 0) return null;

            return cart.Items.FirstOrDefault(item => item.ItemId == id);
        }
        private HttpResponseMessage Response(ApiCartOperationType type, ApiCartResponseStatus status, string errorText = null, ShoppingCart currentCart = null)
        {
            var result = new CartApiResponse
            {
                Operation = type.ToString().ToLower(),
                Status = status.ToString().ToLower(),
                Description = errorText,
                Items = new List<CartApiItemDto>(),
                PriceFormat = new CartApiPriceFormat(WebSiteGlobal.CurrentPhotolab)
            };
            
            var cart = currentCart ?? GetCart();
            if (cart == null) return Request.CreateResponse(HttpStatusCode.OK, result);

            result.TotalPrice = cart.TotalPrice;
            if (cart.Items == null || cart.Items.Count == 0) return Request.CreateResponse(HttpStatusCode.OK, result);

            result.TotalCount += cart.Items.Count;
            foreach (var item in cart.Items)
            {
                var itemDto = new CartApiItemDto
                {
                    Id = item.ItemId,
                    Title = item.Title,
                    Count = item.Quantity,
                    IsCustom = item.IsCustomItem,
                    TotalPrice = item.Price,
                    ASystemTemplate = item.Properties.ASystemTemplateState,
                    Poly1CState = item.Properties.Poly1CCalculatedState,
                    HelloPrintTemplate = item.Properties.HelloPrintTemplateState,
                    AxiomTemplate = item.Properties.AxiomTemplateState,
                };
                result.Items.Add(itemDto);
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        private Tuple<string,string> GetGFProductUrl(int productId)
        {
            var photolab = WebSiteGlobal.CurrentPhotolab;
            var subproduct = _frontendSuppliersService.GetSubproductById(photolab, productId);
            if (subproduct == null) return null;

            var product = _frontendSuppliersService.GetProductById(photolab, subproduct.ParentId);
            if (product?.CategoryIds == null) return null;

            var category = _frontendSuppliersCategoryService.GetById(photolab, product.CategoryIds.FirstOrDefault(), true, true);
            if (category?.Properties == null) return null;

            var productUrl =  "/catalog/" + category.Properties.Uri + "/" + product.Uri;
            var previewUrl = string.Empty;
            if (product.CoverHash != null)
            {
                var coverHash = Utility.Hash.ConvertBytesToHexString(product.CoverHash);
                previewUrl = $"/content/catalog/covers/{photolab.Id}/{productId}/{coverHash}?size=3";
            }
            
            return Tuple.Create(previewUrl, productUrl);
        }
        private class CartApiPriceFormat
        {
            public string Prefix { get; }
            public string Postfix { get; }
            public string Separator { get; }

            public CartApiPriceFormat(Photolab lab)
            {
                Prefix = Utility.GetPrefixCurrencySymbol(lab);
                Postfix = Utility.GetPostfixCurrencySymbol(lab);
                Separator = lab.DefaultLanguage.NumberFormat.NumberDecimalSeparator;
            }
        }
        private class CartApiResponse
        {
            public int TotalCount { get; set; }
            public decimal TotalPrice { get; set; }
            public string Status { get; set; }
            public string Operation { get; set; }
            public string Description { get; set; }
            public List<CartApiItemDto> Items { get; set; }
            public CartApiPriceFormat PriceFormat { get; set; }
        }
        private class CartApiItemDto
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public int Count { get; set; }
            public bool IsCustom { get; set; }
            public decimal TotalPrice { get; set; }
            public ASystemTemplate ASystemTemplate { get; set; }
            public Poly1CCalculatedState Poly1CState { get; set; }
            public HelloPrintTemplate HelloPrintTemplate { get; set; }
            public AxiomTemplate AxiomTemplate { get; set; }
        }
    }
}
