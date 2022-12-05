using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Photoprint.Core;
using Photoprint.Core.Models;
using Photoprint.Core.Models.CustomWorks;
using Photoprint.Core.Services;
using Photoprint.WebSite.API.Models;

namespace Photoprint.WebSite.API.Controllers
{
    public class CatalogCalcController : BaseApiController
    {
        private readonly IPhotolabService _photolabService;
        private readonly IFrontendSuppliersService _frontendSuppliersService;
        private readonly IFrontendSuppliersCategoryService _frontendSuppliersCategoryService;
        private readonly IFrontendSuppliersCirculationService _frontendSuppliersCirculationService;
        private readonly IFrontendSuppliersFixedCirculationService _frontendSuppliersFixedCirculationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUserService _userService;
        private readonly ISuppliersPrintingService _suppliersPrintingService;
        private readonly IFrontendCustomWorkService _frontendCustomWorkService;
        private readonly IAffiliateUserService _affiliateUserService;
        private readonly IFrontendCustomWorkService _customWorkService;
        private readonly IFrontendSuppliersSettingsService _frontendSuppliersSettingsService;
        private readonly IFrontendDiscountService _frontendDiscountService;

        public CatalogCalcController(IAuthenticationService authenticationService, IPhotolabService photolabService, 
            IFrontendSuppliersCirculationService frontendSuppliersCirculationService, IFrontendSuppliersFixedCirculationService frontendSuppliersFixedCirculationService, IFrontendSuppliersService frontendSuppliersService, 
            IFrontendSuppliersCategoryService frontendSuppliersCategoryService, IShoppingCartService shoppingCartService, IUserService userService,
            IAffiliateUserService affiliateUserService, IFrontendSuppliersSettingsService frontendSuppliersSettingsService, IFrontendCustomWorkService frontendCustomWorkService, IFrontendDiscountService frontendDiscountService, ISuppliersPrintingService suppliersPrintingService) : base(authenticationService)
        {
            _photolabService = photolabService;
            _frontendSuppliersService = frontendSuppliersService;
            _frontendSuppliersCategoryService = frontendSuppliersCategoryService;
            _shoppingCartService = shoppingCartService;
            _userService = userService;
            _affiliateUserService = affiliateUserService;
            _frontendSuppliersSettingsService = frontendSuppliersSettingsService;
            _frontendCustomWorkService = frontendCustomWorkService;
            _frontendDiscountService = frontendDiscountService;
            _suppliersPrintingService = suppliersPrintingService;
            _customWorkService = frontendCustomWorkService;
            _frontendSuppliersCirculationService = frontendSuppliersCirculationService;
            _frontendSuppliersFixedCirculationService = frontendSuppliersFixedCirculationService;
        }

        [HttpGet]
        [Route("api/gfcalc/init")]
        public HttpResponseMessage GetInitState(int photolabId, int productId)
        {
            var photolab = _photolabService.GetById(photolabId);
            if (photolab != null)
            {
                var product = _frontendSuppliersService.GetProductById(photolab, productId);
                if (product != null)
                {
                    var printingGroup = product.CurrentPrintingGroupId != null ? _suppliersPrintingService.GetGroupById(photolab, product.CurrentPrintingGroupId.Value) : null;
                    var availableWorks = _customWorkService.GetEnabledList(printingGroup, photolab);
                    var rules = _customWorkService.GetCustomWorkToRules(photolab.Id).GetRulesByListCustomWorks(availableWorks, true);
                    
                    var currentWorksStates = new List<CustomWorkStateModel>();

                    var additionalPriceByItemLocalization = new AdditionalPriceByItemLocalization
                    {
                        PerItemText = RM.GetString(RS.Order.CustomWorkItems.PerItem),
                        PerItemPartText = RM.GetString(RS.Order.CustomWorkItems.PerItemPart)
                    };
                    currentWorksStates.AddRange(_customWorkService.GetCustomWorksStateModels(availableWorks, new ProductInfoData(product), CustomWorkItemViewContext.Editor, photolab, photolab.DefaultLanguage, additionalPriceByItemLocalization));
                    
                    var result = new CustomWorkSelectorModel 
                    {
                        Works = currentWorksStates,
                        Rules = rules
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "photolab not found");
        }

        [HttpPost]
        [HttpOptions]
        [EnableCors(origins: "*", headers: "*", methods: "POST, OPTIONS")]
        [Route("api/gfcalc/price")]
        public HttpResponseMessage GetPrice(GiftPriceRequest request)
        {
            if (request == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "request not found");

            if (request.ProductQuantities == null || request.ProductQuantities.Count == 0)
            {
                var nullDto = new GiftPriceDTO
                {
                    RequestId = request.RequestId,
                    ProductId = request.ProductId,
                    Total = 0.0m,
                    ItemTablePrices = Array.Empty<decimal>(),
                    PriceParameters = Array.Empty<string>(),
                    Price = 0.0m,
                    ProductPrice = 0.0m,
                    Discount = 0.0m,
                    ItemsPrices = Enumerable.Empty<CustomWorkItemPriceDTO>(),

                };
                return Request.CreateResponse(HttpStatusCode.OK, nullDto);
            }

            var photolab = _photolabService.GetById(request.PhotolabId);
            if (photolab == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "photolab not found");

            var product = _frontendSuppliersService.GetProductById(photolab, request.ProductId);
            if (product == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "product not found");

            var subproduct = _frontendSuppliersService.GetSubproductsByParent(photolab, product, true).FirstOrDefault() ?? _frontendSuppliersService.GetSubproductsByParent(photolab, product, false).FirstOrDefault();
            var supplierSettings = _frontendSuppliersSettingsService.GetSuppliersSettings(photolab);

            var dto = new GiftPriceDTO
            {
                RequestId = request.RequestId,
                ProductId = product.Id,
                ItemTablePrices = Array.Empty<decimal>(),
                PriceParameters = Array.Empty<string>(),
                Total = 0.0m,
                Price = 0.0m,
                ProductPrice = 0.0m,
                Discount = 0.0m,
                ItemsPrices = null
            };
            var itemsDict = new Dictionary<int, CustomWorkItemPriceDTO>();
            var itemTablePricesList = new List<decimal>();
            foreach (var subPrQuantity in request.ProductQuantities)
            {
                //if (subPrQuantity == 0) continue;

                var previewQuantity = subPrQuantity > 0 ? subPrQuantity : 1;
                ShoppingCartItem item = null;
                try { item = GetItemByProductPriceRequest(request.SelectedItems, request.PrintId, previewQuantity, supplierSettings, product, subproduct, photolab); } catch { }
                if (item == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "shopping cart item not found");

                var allCustomWorkItemsPrices = GetCustomWorkItemPrices(item, request.AllItems, request.SelectedItems, previewQuantity, product, photolab);
                _frontendCustomWorkService.RecalculateAdditionalPriceItems(item);

                if (photolab.Properties.ShowPriceWithDiscountInCalculate)
                {
                    var cart = _shoppingCartService.GetCart(AuthenticationService.LoggedInUser, photolab);
                    var items = new List<IPurchasedItem>(cart.Items.Count + 1);
                    items.AddRange(cart.Items);
                    items.Add(item);

                    _frontendDiscountService.FindDiscountsForOrder(photolab, AuthenticationService.LoggedInUser, items);
                }
                var discountPrice = item.DiscountsPriceTotal;
                var total = item.Price - discountPrice;
                var productPrice = item.PriceWithoutCustomWorks;
                var avgPrice = (total - productPrice) / previewQuantity; // средняя цена опций
                itemTablePricesList.Add(item.ItemPrice + (item.CustomWorkPrice-discountPrice) / previewQuantity);

                if (subPrQuantity == 0) continue;
                dto.Total += Math.Round(total, photolab.NumberDecimalDigits);
                dto.Price += Math.Round(item.Price, photolab.NumberDecimalDigits);
                dto.ProductPrice += Math.Round(productPrice, photolab.NumberDecimalDigits);
                dto.Discount += Math.Round(discountPrice, photolab.NumberDecimalDigits);
                

                foreach (var itemCWPrice in allCustomWorkItemsPrices)
                {
                    if (itemCWPrice == null) continue;
                    if (!itemsDict.ContainsKey(itemCWPrice.Id))
                    {
                        itemsDict.Add(itemCWPrice.Id, itemCWPrice);
                    }
                    else
                    {
                        itemsDict[itemCWPrice.Id].CalculatedValue += itemCWPrice.CalculatedValue;
                        itemsDict[itemCWPrice.Id].TotalPrice += itemCWPrice.TotalPrice;
                    }
                }
            }
            var priceParameters = new List<string>() { ExtensionMethods.GetPrefixCurrencySymbolFormated(photolab).ToString(), ExtensionMethods.GetPostfixCurrencySymbolFormated(photolab).ToString(), photolab.PriceFormat.DecimalSeparator, photolab.PriceFormat.GroupSeparator };
            dto.ItemsPrices = itemsDict.Values;
            dto.PriceParameters = priceParameters;
            dto.ItemTablePrices = itemTablePricesList.ToArray();
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }
        
        private ShoppingCartItem GetItemByProductPriceRequest(IEnumerable<CustomWorkState> reqSelectedItems, int printId, int quantity, SuppliersSettings suppliersSettings, GFProduct product, GFSubproduct subproduct, Photolab lab)
        {
            var settings = _frontendSuppliersSettingsService.GetSuppliersSettings(lab);
            var circulationRule = _frontendSuppliersCirculationService.GetCirculationRuleById(lab, product.CirculationId ?? settings.CirculationId);
            
            var circulation = circulationRule.GetPriceRuleByQuantity(product.SupplierType, printId, quantity);
            if (circulation == null) return null;
         
            var item = new GFShoppingCartItem(product.Id, quantity)
            {
                GFPrintId = printId,
                GFCirculationSettings = circulationRule
            };
            var basePrice = subproduct.Properties.EnduserPrice;
            var fixedRule = _frontendSuppliersFixedCirculationService.GetCirculationRuleByCirculationId(lab, product.FixedCirculationId);
            if (fixedRule?.FixedRules != null && fixedRule.FixedRules.Count > 0)
            {
                basePrice = fixedRule.FixedRules.FirstOrDefault(x => x.Quantity == quantity)?.ItemPrice ?? basePrice;
            }
            item.ItemPrice = circulationRule.GetPriceByQuantity(suppliersSettings.RoundType, basePrice, product.SupplierType, item.GFPrintId, out decimal generalAdditionalPrice, quantity, true);
            item.AdditionalPrice = generalAdditionalPrice;

            // считаем стоимость опций
            var selectedItems = reqSelectedItems?.SelectMany(x => x.Items).AsList();
            var additionalPriceItems = _frontendCustomWorkService.GetAdditionalPriceItemByState(selectedItems, new ProductInfoData(product), lab, item, out _, out _);
            if (additionalPriceItems.Count > 0)
            {
                item.Properties.AdditionalPriceItems.AddRange(additionalPriceItems);
            }

            return item;
        }
        
        private IReadOnlyCollection<CustomWorkItemPriceDTO> GetCustomWorkItemPrices(ShoppingCartItem item, IEnumerable<CustomWorkState> reqAllItems, IEnumerable<CustomWorkState> reqSelectedItems, int quantity, GFProduct product, Photolab photolab)
        {
            if (reqAllItems == null || !reqAllItems.Any()) return Array.Empty<CustomWorkItemPriceDTO>();
            if (item == null) return Array.Empty<CustomWorkItemPriceDTO>();

            var allItems = reqAllItems.SelectMany(x => x.Items).AsList();
            var calculatedItems = item.AdditionalPriceItems.AsList();
            var additionalItems = _frontendCustomWorkService.GetAdditionalPriceItemByState(calculatedItems, allItems, new ProductInfoData(product), photolab, item, out _, out _);
            var seletedItemsIds = reqSelectedItems?.SelectMany(x => x.Items).Where(x => x.ItemId.HasValue).Select(x => x.ItemId.Value).AsList();
            _frontendCustomWorkService.RecalculateAdditionalPriceItems(seletedItemsIds, additionalItems, item.PriceWithoutDependences, item.PriceWithoutCustomWorks);

            var itemsResult = new List<CustomWorkItemPriceDTO>(reqAllItems.Count());
            foreach (var additionalItem in additionalItems)
            {
                var itemTotalPrice = AdditionalPriceItem.CalculatePrice(additionalItem, quantity, 1);
                itemsResult.Add(new CustomWorkItemPriceDTO
                {
                    Id = additionalItem.ItemId,
                    CalculatedValue = additionalItem.ArbitrarySizeInfoResult?.CalculatedValueFromSelectedSize,
                    TotalPrice = Math.Round(itemTotalPrice, photolab.NumberDecimalDigits)
                });
            }

            return itemsResult;
        }
    }
}