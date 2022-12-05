
using PhoneConverter;
using Photoprint.Core;
using Photoprint.Core.InputModels;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using Photoprint.WebSite.API.Models.Delivery;
using Photoprint.WebSite.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Photoprint.WebSite.Shared;

namespace Photoprint.WebSite.API.Controllers
{
    public class DeliveryController : BaseApiController
    {
        private readonly IFrontendShippingService _frontendShippingService;
        private readonly IPhotolabService _photolabService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IShippingCalculatorService _shippingCalculatorService;
        private readonly IResourceManager _resourceManager;
        private readonly IOrderService _orderService;
        private readonly IFrontendDiscountService _frontendDiscountService;
        private readonly IFrontendUserCompanyService _userCompanyService;
        private readonly IUserService _userService;
        private readonly IPhoneRulesService _phoneRulesService;
        private readonly IAffiliateUserService _affiliateUserService;
        private readonly IFrontendUserCompanyService _frontendUserCompanyService;
        private readonly IFrontendOrderService _frontendOrderService;
        private readonly IFrontendSuppliersSettingsService _frontendSuppliersSettingsService;
        private readonly ILanguageService _languageService;
        private readonly IShippingProviderResolverService _shippingProviderResolverService;
        private readonly IPhotolabSettingsService _photolabSettingsService;
        private readonly IOrderCommentService _orderCommentService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IOrderRelationService _orderRelationService;

        public DeliveryController(IAuthenticationService authenticationService,
                                  IFrontendShippingService frontendShippingService,
                                  IPhotolabService photolabService,
                                  IShoppingCartService shoppingCartService,
                                  IShippingCalculatorService shippingCalculatorService,
                                  IResourceManager resourceManager,
                                  IOrderService orderService,
                                  IFrontendDiscountService frontendDiscountService,
                                  IFrontendUserCompanyService userCompanyService,
                                  IUserService userService,
                                  IPhoneRulesService phoneRulesService,
                                  IAffiliateUserService affiliateUserService,
                                  IFrontendUserCompanyService frontendUserCompanyService,
                                  IFrontendOrderService frontendOrderService,
                                  IFrontendSuppliersSettingsService frontendSuppliersSettingsService,
                                  ILanguageService languageService,
                                  IShippingProviderResolverService shippingProviderResolverService, 
                                  IPhotolabSettingsService photolabSettingsService,
                                  IOrderCommentService orderCommentService,
                                  IOrderDetailService orderDetailService,
                                  IOrderRelationService orderRelationService) : base(authenticationService)
        {
            _frontendShippingService = frontendShippingService;
            _photolabService = photolabService;
            _shoppingCartService = shoppingCartService;
            _shippingCalculatorService = shippingCalculatorService;
            _resourceManager = resourceManager;
            _orderService = orderService;
            _frontendDiscountService = frontendDiscountService;
            _userCompanyService = userCompanyService;
            _userService = userService;
            _phoneRulesService = phoneRulesService;
            _affiliateUserService = affiliateUserService;
            _frontendUserCompanyService = frontendUserCompanyService;
            _frontendOrderService = frontendOrderService;
            _frontendSuppliersSettingsService = frontendSuppliersSettingsService;
            _languageService = languageService;
            _shippingProviderResolverService = shippingProviderResolverService;
            _photolabSettingsService = photolabSettingsService;
            _orderCommentService = orderCommentService;
            _orderDetailService = orderDetailService;
            _orderRelationService = orderRelationService;
        }

        [HttpPost]
        [Route("api/delivery/initData")]
        public HttpResponseMessage GetDeliveryManagerData(BaseDeliveryRequest request)
        {
            var result = TryGetData(request, out _, out var user, out var photolab, out var cart, out var language);
            if (!result.IsSuccess) return result.Request;
            if (cart.Items.Count == 0)
            {
                var lastOrder = _orderService.GetLastOrder(photolab);
                if (lastOrder != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { RedirectUrl = new UrlManager { CurrentOrderId = lastOrder.Id }.GetHRefUrl(SiteLinkType.UserOrder) });
                }
            }

            var ordinaryItemsPrice = cart.Items
                .Where(x => x is MaterialShoppingCartItem || x is ProductShoppingCartItem)
                .Sum(shoppingCart => shoppingCart.Price);

            if (ordinaryItemsPrice > 0
                && photolab.Properties.MinimumShoppingCartItemsPrice - ordinaryItemsPrice > 0.1m
                && !photolab.Properties.IsWarningOnlyMinimumShoppingCartItemsPrice)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { RedirectUrl = new UrlManager().GetHRefUrl(SiteLinkType.ShoppingCart) });
            }

            var settings = _photolabSettingsService.GetSettings<DeliveryWindowSettings>(photolab) ?? new DeliveryWindowSettings();

            return Request.CreateResponse(HttpStatusCode.OK, new DeliveryInitResponse(photolab, language, cart, settings));
        }

        [HttpPost]
        [Route("api/delivery/shippings")]
        public HttpResponseMessage GetAvailableShippingTypes(DeliveryGetAvailableShippingTypessRequest request)
        {
            var result = TryGetData(request, out var instaceGuid, out var user, out var photolab, out var cart, out var language);
            if (!result.IsSuccess) return result.Request;

            if (string.IsNullOrEmpty(request.City?.Title))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, DeliveryResponseExceptionPhrase.CityNotFound);

            var settings = _photolabSettingsService.GetSettings<DeliveryWindowSettings>(photolab);
            var officesInPickpoints = !settings?.PickpointsSettings?.IsOfficesInAnotherBlock ?? true;

            var defaultShippingId = photolab.Properties.DefaultShippingId;

            var availableShippingByTypes = new Dictionary<DeliveryDisplayType, AvailableShippingResult.PrefinalData>();
            var shippingInfoModels = _frontendShippingService.GetShippingInfoToDeliveryModelsByCity(instaceGuid, photolab, request.City, cart.Items, language);

            if (settings.UseShippingFromPreviousOrder)
            {
                var prevShippingId = shippingInfoModels.FirstOrDefault(s => s.Id == user.Properties.LastShippingId)?.Id;
                defaultShippingId = prevShippingId ?? defaultShippingId;
            }

            foreach (var model in shippingInfoModels)
            {
                var displayType = DeliveryDisplayTypeResolver.GetDeliveryDisplayType(model, officesInPickpoints);
                if (displayType == DeliveryDisplayType.DeliveryPlaginPickPoint || displayType == DeliveryDisplayType.DeliveryPlaginSafeRoute)
                {
                    availableShippingByTypes.Add(displayType, new AvailableShippingResult.PrefinalData(model.Id) { IsDefaultType = defaultShippingId == model.Id });
                }
                else
                {
                    if (model.AddressesCount <= 0) continue;
                    if (!availableShippingByTypes.ContainsKey(displayType))
                    {
                        availableShippingByTypes.Add(displayType, new AvailableShippingResult.PrefinalData(model.Id)
                        {
                            AddressCount = model.AddressesCount,
                            IsDefaultType = defaultShippingId == model.Id
                        });
                    }
                    else
                    {
                        var availableShippingType = availableShippingByTypes[displayType];
                        availableShippingType.ShippingList.Add(model.Id);
                        availableShippingType.AddressCount += model.AddressesCount;
                        availableShippingType.IsDefaultType =  defaultShippingId == model.Id;
                    }
                }
            }

            var response = availableShippingByTypes.Select(x => new AvailableShippingResult(
                type: x.Key,
                shippingIds: x.Value.ShippingList,
                title: _resourceManager.GetString("Delivery.Title." + x.Key, language, photolab),
                titleNote: string.Format(_resourceManager.GetString("Delivery.Description." + x.Key, language, photolab), x.Value.AddressCount),
                addressCount: x.Value.AddressCount,
                maxWeight: x.Value.MaxWeight,
                isDefault: x.Value.IsDefaultType));

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


        [HttpPost]
        [Route("api/delivery/plugins/initData")]
        public HttpResponseMessage GetPluginShippingData(DeliveryGetPluginShippingDataRequest request)
        {
            var result = TryGetData(request, out _, out var user, out var photolab, out var cart, out _);
            if (!result.IsSuccess) return result.Request;

            var shippings = _frontendShippingService.GetAvailableList<Shipping>(photolab, cart.Items);
            if (shippings.Count == 0)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, DeliveryResponseExceptionPhrase.ShippingNotFound);
            }

            //
            // Внимание - на один сайт может быть только один плагин одного типа!!!
            // поэтому тут всегда отдается первый попавшийся
            //

            BasePluginShippingData response = null;
            foreach (var shipping in shippings)
            {
                var shippingDisplayType = DeliveryDisplayTypeResolver.GetDeliveryDisplayType(shipping, true);
                if (shippingDisplayType != request.Type) continue;

                switch (request.Type)
                {
                    case DeliveryDisplayType.DeliveryPlaginPickPoint:
                        var ppSettings = shipping.ServiceProviderSettings as PickpointServiceProviderSettings;
                        return Request.CreateResponse(HttpStatusCode.OK, new PickPointPluginShippingData
                        {
                            ShippingId = shipping.Id,
                            IKN = ppSettings?.IKN ?? string.Empty
                        });

                    case DeliveryDisplayType.DeliveryPlaginSafeRoute:

                        const int _maxPrice = 999999; // Максимальная цена для того, чтобы виджет иницилизировался
                        decimal summaryPrice = 0;
                        double totalWeight = 0;

                        cart.Items.ForEach(i =>
                        {
                            summaryPrice += i.Price;
                            totalWeight += i.TotalWeight;
                        });

                        var productObjs = new List<SafeRouteProductDto>(1)
                        {
                            new SafeRouteProductDto
                            {
                                name = "OrderInfo",
                                count = 1,
                                price = summaryPrice <= _maxPrice ? summaryPrice : _maxPrice
                            }
                        };

                        if (shipping.ServiceProviderSettings is DDeliveryV2ServiceProviderSettings settings && totalWeight < settings.DefaultWeight)
                        {
                            totalWeight = settings.DefaultWeight;
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, new SafeRoutePluginShippingData
                        {
                            ShippingId = shipping.Id,
                            Products = productObjs,
                            TotalWeight = Math.Round(totalWeight, 3),
                            IsShippingPricePaidSeparately = shipping.IsShippingPricePaidSeparately
                        });
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        [Route("api/delivery/addresses")]
        public HttpResponseMessage GetShippingAddresses(DeliveryGetShippingAddressesRequest request)
        {
            var result = TryGetData(request, out var instaceGuid, out var user, out var photolab, out var cart, out var language);
            if (!result.IsSuccess) return result.Request;


            if (string.IsNullOrEmpty(request.City?.Title?.Trim()))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, DeliveryResponseExceptionPhrase.CityNotFound);
            }

            var shippingsInfo = _frontendShippingService.GetSelectedShippingsSmallWithAddresses(instaceGuid, photolab,
                request.City, cart.Items, request.ShippingIds, language);

            HttpResponseMessage response;
            switch (request.Type)
            {
                case DeliveryDisplayType.Courier:
                    response = Request.CreateResponse(HttpStatusCode.OK, shippingsInfo.SelectMany(si => 
                        si.Value.Select(sa => new CourierPointDto(si.Key, sa, language))));
                    break;
                case DeliveryDisplayType.Pickpoint:
                    response = Request.CreateResponse(HttpStatusCode.OK, shippingsInfo.SelectMany(si => 
                        si.Value.Select(sa => new DistributionPointDto(si.Key, sa, language))));
                    break;
                case DeliveryDisplayType.Office:
                    response = Request.CreateResponse(HttpStatusCode.OK, shippingsInfo.SelectMany(si => 
                        si.Value.Select(sa => new DistributionPointDto(si.Key, sa, language))));
                    break;
                default:
                    response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Wrong display type");
                    break;
            }
            return response;
        }


        [HttpGet]
        [Route("api/delivery/userData")]
        public HttpResponseMessage GetUserData(int frontendId)
        {
            var user = AuthenticationService.LoggedInUser;
            if (user?.IsAnonymous != false)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, DeliveryResponseExceptionPhrase.UserUnauthorized);
            }

            var photolab = _photolabService.GetById(frontendId);
            if (photolab == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, DeliveryResponseExceptionPhrase.PhotolabNotFound);
            }

            var settings = _photolabSettingsService.GetSettings<DeliveryWindowSettings>(photolab);
            var userCompany = _userCompanyService.GetByUser(user);

            var userDataResponse = new DeliveryGetUserDataResponse
            {
                CanBeOrderByUserCompany = userCompany != null && userCompany.Role != UserCompanyRole.Guest,
                Recipient = new DeliveryRecipient
                {
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone.ToString()
                }
            };

            var lastOrder = _orderService.GetListByUser(user, photolab.Id, OrderFilter.All, 0, 10).FirstOrDefault(x => x.DeliveryAddress != null && x.Shipping != null);
            if (lastOrder != null)
            {
                userDataResponse.LastShippingId = lastOrder.ShippingId;
                userDataResponse.LastShippingType = ((int)lastOrder.ShippingType).ToString();
                var deliveryAddress = lastOrder.DeliveryAddress;
                userDataResponse.LastAddress = deliveryAddress.ToString();

                userDataResponse.Recipient.Comment = lastOrder.ShippingComment;
                if (!string.IsNullOrWhiteSpace(deliveryAddress.FirstName))
                {
                    userDataResponse.Recipient.FirstName = deliveryAddress.FirstName;
                }

                if (!string.IsNullOrWhiteSpace(deliveryAddress.MiddleName))
                {
                    userDataResponse.Recipient.MiddleName = deliveryAddress.MiddleName;
                }

                if (!string.IsNullOrWhiteSpace(deliveryAddress.LastName))
                {
                    userDataResponse.Recipient.LastName = deliveryAddress.LastName;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, userDataResponse);
        }

        [HttpPost]
        [Route("api/delivery/price")]
        public HttpResponseMessage GetPrice(DeliveryGetPriceRequest request)
        {
            var result = TryGetData(request, out _, out var user, out var photolab, out var cart, out _);
            if (!result.IsSuccess) return result.Request;

            if (request.Addresses.Count == 0)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, DeliveryResponseExceptionPhrase.ShippingNotFound);
            }

            var resultPrices = new List<DeliveryGetPriceModelPriceReponse>(request.Addresses.Count);
            var appliedDiscount = _frontendDiscountService.FindDiscountsForOrder(photolab, user, cart.Items, null, false);

            foreach (var address in request.Addresses)
            {
                var shipping = _frontendShippingService.GetById<Shipping>(address.ShippingId);
                if (shipping is null)
                {
                    resultPrices.Add(new DeliveryGetPriceModelPriceReponse
                    {
                        ShippingId = address.ShippingId,
                        ShippingAddressId = address.Id,
                        CalculationResult = JObject.FromObject(new DeliveryPriceCalculationResult())
                    });
                    continue;
                }

                var selectedAddress = _frontendShippingService.GetSelectedShippingAddress(address);
                if (selectedAddress == null)
                {
                    resultPrices.Add(new DeliveryGetPriceModelPriceReponse
                    {
                        ShippingId = address.ShippingId,
                        ShippingAddressId = address.Id,
                        CalculationResult = JObject.FromObject(new DeliveryPriceCalculationResult())
                    });
                    continue;
                }

                var calcResult = _shippingCalculatorService.GetShippingPrice(shipping,
                    new OrderAddress(selectedAddress), cart.Items, appliedDiscount);
                resultPrices.Add(new DeliveryGetPriceModelPriceReponse
                {
                    ShippingId = address.ShippingId,
                    ShippingAddressId = address.Id,
                    CalculationResult = JObject.FromObject(calcResult)
                });
            }
            return Request.CreateResponse(HttpStatusCode.OK, resultPrices);
        }


        [HttpPost]
        [Route("api/delivery/orderCreate")]
        public HttpResponseMessage CreateOrderFromDeliveryPage(DeliveryCreateOrderRequest request)
        {
            var result = TryGetData(request, out _, out var user, out var photolab, out var cart, out var language);
            if (!result.IsSuccess) return result.Request;

            if (request.DeliveryModel?.ShippingData == null || request.DeliveryModel?.UserData == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, DeliveryResponseExceptionPhrase.BadRequest);

            var companyAccount = photolab.CompanyAccount;
            if (companyAccount == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, DeliveryResponseExceptionPhrase.CompanyAccountNotFound);

            var selectedShipping = _frontendShippingService.GetById<Shipping>(request.DeliveryModel.ShippingData.ShippingId);
            var selectedAddress = _frontendShippingService.GetSelectedOrderAddress(request.DeliveryModel, selectedShipping);
            if (selectedShipping == null || selectedAddress == null)
            {
                var response = new DeliveryCreateOrderResponse
                {
                    IsSuccessful = false,
                    Message = new DeliveryMessage(DeliveryMessageType.Error, RM.GetString(RS.Delivery.TotalPriceBlock.OrderCreateShippingAddressError, photolab, language))
                };
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }

            bool isTeamUser = user.IsInAnyCompanyOrFrontendTeam(WebSiteGlobal.CurrentCompanyAccount);
            if (!isTeamUser)
            {
                var updateUser = !photolab.Properties.IsDisabledCheckoutProfileUpdater;
                var isUserFullNameEmpty = string.IsNullOrWhiteSpace(user.FirstName) && string.IsNullOrWhiteSpace(user.LastName);
                if (updateUser || isUserFullNameEmpty)
                {
                    if (isUserFullNameEmpty ||
                        photolab.Properties.IsCyrillicNamesEnabled ||
                        selectedShipping.ShippingServiceProviderType == ShippingServiceProviderType.Photomax ||
                        selectedShipping.ShippingServiceProviderType == ShippingServiceProviderType.NovaposhtaV2)
                    {
                        var regex = new Regex(@"^[a-zA-Z][a-zA-Z0-9-_\s]*$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

                        var regFirstName = string.IsNullOrEmpty(user.FirstName) || regex.IsMatch(user.FirstName);
                        var regLastName = string.IsNullOrEmpty(user.LastName) || regex.IsMatch(user.LastName);
                        var regMiddleName = string.IsNullOrEmpty(user.MiddleName) || regex.IsMatch(user.MiddleName);

                        if (isUserFullNameEmpty || regFirstName || regLastName || regMiddleName || user.LastName == null || user.MiddleName == null)
                        {
                            user.FirstName = selectedAddress.FirstName;
                            user.LastName = selectedAddress.LastName;
                            user.MiddleName = selectedAddress.MiddleName;
                            _userService.Update(user);
                        }
                    }

                    if (updateUser || user.IsPhoneEmpty())
                    {
                        var phoneRules = _phoneRulesService.GetByPhotolab(photolab).RulesSet.InputRules.Distinct().ToArray();
                        PhoneConverterGateway.Instance.TryParsePhoneNumber(selectedAddress.Phone, out PhoneNumber phoneNumber, out bool isSpamNumber, phoneRules);
                        if (isSpamNumber)
                        {
                            var response = new DeliveryCreateOrderResponse
                            {
                                IsSuccessful = false,
                                Message = new DeliveryMessage(DeliveryMessageType.Error, RM.GetString(RS.General.Error.ShippingPhoneIncorrect))
                            };
                            return Request.CreateResponse(HttpStatusCode.OK, response);
                        }

                        var input = new PhotolabClientInput(user.Id, user.Email, selectedAddress.Phone, phoneNumber, companyAccount, user.Properties)
                        {
                            DisplayName = user.DisplayName,
                            FirstName = user.FirstName,
                            HomeAddress = user.HomeAddress,
                            HomePhone = user.HomePhone,
                            LastName = user.LastName
                        };
                        _userService.Update(input, "WebSite Delivery");
                    }
                }
            }

            try
            {
                var shoppingCartItems = cart.Items;
                //с айфона не редеректит если со страницы платежного шлюза нажать назад и попробовать создать заказ еще раз
                if (!shoppingCartItems.Any())
                {
                    var ordersResponse = new DeliveryCreateOrderResponse
                    {
                        IsSuccessful = true,
                        RedirectUrl = new UrlManager().GetHRefUrl(SiteLinkType.UserOrders)
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, ordersResponse);
                }
                // DIGILABS
                string orderNumber = null;
                var itemJobId = shoppingCartItems?.Where(i => !string.IsNullOrWhiteSpace(i.Properties?.JobId)).FirstOrDefault()?.Properties.JobId;
                if (itemJobId != null) orderNumber = itemJobId;
                //

                var affiliateUser = _affiliateUserService.GetAffiliateForOrder(companyAccount, user);
                var company = request.DeliveryModel.UserData.FromUserCompany ? _frontendUserCompanyService.GetByUser(user)?.UserCompany : null;
                var orderComment = request.DeliveryModel.UserData.OrderCommentary;

                var orderCommentInputList = new List<OrderCommentInput>();
                var sourceOrderIds = new List<int>();
                foreach (var item in shoppingCartItems)
                {
                    if (!string.IsNullOrWhiteSpace(item.SystemDescription))
                    {
                        orderCommentInputList.Add(new OrderCommentInput(null, null, null, item.SystemDescription, false, true));
                    }

                    if (!string.IsNullOrWhiteSpace(item.UserDescription))
                    {
                        orderCommentInputList.Add(new OrderCommentInput(null, null, null, item.UserDescription, false));
                    }
                    if (item.Properties.SourceOrderId != null)
                    {
                        if (!sourceOrderIds.Contains(item.Properties.SourceOrderId.Value))
                        {
                            sourceOrderIds.Add(item.Properties.SourceOrderId.Value);
                        }
                    }
                    
                }

                if (!string.IsNullOrWhiteSpace(orderComment))
                {
                    var ip = SiteUtils.GetIpAddress(new HttpRequestWrapper(HttpContext.Current.Request));
                    orderCommentInputList.Add(new OrderCommentInput(null, null, ip, orderComment, true));
                }

                var order = _frontendOrderService.Create(photolab, user, shoppingCartItems, selectedShipping, selectedAddress,
                    OrderPaymentStatus.NotPaid, language, affiliateUser, orderNumber, WebSiteGlobal.IsMobile, company,
                    properties =>
                    {
                        var (gaClientId, yaClientId) = SiteUtils.GetWebAnalyticsClientId(GetHttpContext(Request));
                        properties.GoogleAnalyticsClientId = gaClientId;
                        properties.YandexMetrikaClientId = yaClientId;
                        properties.AdditionalNotificationEmail = request.DeliveryModel.UserData.AdditionalEmail;
                        properties.AdditionalNotificationPhone = request.DeliveryModel.UserData.AdditionalPhone;
                        return properties;
                    }
                    , OrderSource.Site, orderCommentInputList);
                if (sourceOrderIds.Count > 0)
                {
                    foreach (var sourceOrderId in sourceOrderIds)
                    {
                        var orderPhotolab = _photolabService.GetById(order.PhotolabId);
                        var relatedOrder = _orderService.GetById(sourceOrderId);
                        if (relatedOrder == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "related order not found");
                        var relatedOrderPhotolab = _photolabService.GetById(relatedOrder.PhotolabId);

                        if (orderPhotolab.CompanyAccountId == relatedOrderPhotolab.CompanyAccountId)
                        {
                            var bindResult = _orderRelationService.BindOrders(order, relatedOrder, OrderRelationType.Clone);
                            if (!bindResult)
                            {
                                Request.CreateResponse(HttpStatusCode.BadRequest);
                            }
                        }
                    }
                }

                var response = new DeliveryCreateOrderResponse
                {
                    IsSuccessful = true,
                    RedirectUrl = new UrlManager { CurrentOrderId = order.Id }.GetHRefUrl(SiteLinkType.UserOrderPayment)
                };

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (PhotoprintValidationException ex)
            {
                var suppliersMinPrice = _frontendSuppliersSettingsService.GetSuppliersSettings(photolab).MinPrice ?? 0;
                var validationPrice = Math.Max(photolab.Properties.MinimumShoppingCartItemsPrice, suppliersMinPrice);
                var response = new DeliveryCreateOrderResponse
                {
                    IsSuccessful = false,
                    Message = new DeliveryMessage(DeliveryMessageType.Error, string.Format(RM.GetString(ex.ResourceKey), Utility.GetPrice(validationPrice, photolab)))
                };

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }


        private class DeliveryCreateOrderResponse
        {
            public DeliveryMessage Message { get; set; }
            public bool IsSuccessful { get; set; }
            public string RedirectUrl { get; set; }
        }

        private (bool IsSuccess, HttpResponseMessage Request) TryGetData(BaseDeliveryRequest request,
            out Guid instaceGuid, out User user,
            out Photolab photolab, out ShoppingCart cart, out Language language)
        {
            user = AuthenticationService.LoggedInUser;
            photolab = _photolabService.GetById(request?.FrontendId ?? 0);
            cart = _shoppingCartService.GetCart(user, photolab);
            instaceGuid = Guid.TryParse(request?.InstanceGuid, out var parsedGuid) ? parsedGuid : Guid.NewGuid();
            language = photolab.Languages.FirstOrDefault(x => x.Id == request?.LanguageId) ?? photolab.DefaultLanguage;


            if (request is null)
                return (false, Request.CreateErrorResponse(HttpStatusCode.BadRequest, DeliveryResponseExceptionPhrase.BadRequest));

            if (user is null)
                return (false, Request.CreateErrorResponse(HttpStatusCode.BadRequest, DeliveryResponseExceptionPhrase.UserNotFound));

            if (user.IsAnonymous == true)
                return (false, Request.CreateErrorResponse(HttpStatusCode.Forbidden, DeliveryResponseExceptionPhrase.UserUnauthorized));

            if (photolab is null)
                return (false, Request.CreateErrorResponse(HttpStatusCode.BadRequest, DeliveryResponseExceptionPhrase.PhotolabNotFound));

            if (cart is null)
                return (false, Request.CreateErrorResponse(HttpStatusCode.BadRequest, DeliveryResponseExceptionPhrase.CartIsEmpty));

            return (true, null);
        }

    }
}
