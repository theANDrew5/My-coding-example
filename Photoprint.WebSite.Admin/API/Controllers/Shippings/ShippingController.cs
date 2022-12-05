using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Photoprint.Core;
using Newtonsoft.Json;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using Photoprint.Services;
using Photoprint.WebSite.Admin.API.Models;
using Photoprint.WebSite.Admin.Controls;

namespace Photoprint.WebSite.Admin.API
{
    public class ShippingController : BaseApiController
    {
        private readonly IShippingService _shippingService;
        private readonly IShippingAddressService _shippingAddressService;
        private readonly IShippingCalculatorService _shippingCalculatorService;
        private readonly IShippingProviderResolverService _shippingProviderResolver;
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IUserService _userService;
        private readonly IPhotolabService _photolabService;
        private readonly IOrderRelationService _orderRelationService;
        private readonly IOmnivaService _omnivaService;
        private readonly ICompanyAccountService _companyAccountService;
        private readonly IPickpointService _pickpointService;
        private readonly IFrontendShippingService _frontendShippingService;
        private readonly IPhotolabSettingsService _photolabSettingsService;

        public ShippingController(
            IAuthenticationService authenticationService,
            IShippingService shippingService,
            IShippingAddressService shippingAddressService,
            IShippingCalculatorService shippingCalculatorService,
            IShippingProviderResolverService shippingProviderResolver,
            IOrderService orderService, IOrderDetailService orderDetailService,
            IUserService userService, IPhotolabService photolabService,
            IOrderRelationService orderRelationService,
            IOmnivaService omnivaService,
            IPickpointService pickpointService,
            ICompanyAccountService accountService,
            IFrontendShippingService frontendShippingService, 
            IPhotolabSettingsService photolabSettingsService)
            : base(authenticationService)
        {
            _shippingService = shippingService;
            _shippingAddressService = shippingAddressService;
            _shippingCalculatorService = shippingCalculatorService;
            _shippingProviderResolver = shippingProviderResolver;
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            _userService = userService;
            _photolabService = photolabService;
            _orderRelationService = orderRelationService;
            _omnivaService = omnivaService;
            _companyAccountService = accountService;
            _pickpointService = pickpointService;
            _frontendShippingService = frontendShippingService;
            _photolabSettingsService = photolabSettingsService;
        }

        public class UpdateShippingSettingsDTO
        {
            public DeliveryWindowSettings Settings { get; set; }
            public int PhotolabId { get; set; }
        }
        [HttpPost]
        [Route("api/shipping/settings/update")]
        public HttpResponseMessage UpdateShippingSettings(UpdateShippingSettingsDTO request)
        {
            var photolab = _photolabService.GetById(request.PhotolabId);
            if (photolab == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "photolab not found");
            }
            var user = WebSiteGlobal.LoggedInUser;
            if (!user.IsAdministrator && !user.IsFrontendAdministrator(photolab) && user.CompanyAccountId != photolab.CompanyAccountId)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "no access");
            }
            _photolabSettingsService.CreateOrUpdateSettings(photolab, request.Settings);
            return Request.CreateResponse(HttpStatusCode.OK, "settings saved");
        }
        [HttpGet]
        [Route("api/shipping/list")]
        public HttpResponseMessage GetShippingList(int photolabId, decimal? totalPrice)
        {
            var photolab = _photolabService.GetById(photolabId);
            if (photolab == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "photolab not found");

            var user = WebSiteGlobal.LoggedInUser;
            if (!user.IsAdministrator && user.CompanyAccountId != photolab.CompanyAccountId)
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "no access");

            var shippings = _shippingService.GetListWithPriceRestrictions(photolab.Id,totalPrice)
                .OrderBy(t => photolab.Properties.SortedShippingTypes.IndexOf(t.Type));
            var result = shippings.Select(s => new
            {
                s.Id,
                Title = s.AdminTitle,
                Type = (int)s.Type,
                TypeString = RM.GetString("Shipping.Type" + s.Type)
            });

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        [HttpGet]
        [Route("api/shipping/dPoints")]
        public HttpResponseMessage GetDPoints(int photolabId)

        {
            var loggedInUser = AuthenticationService.LoggedInUser;
            if (loggedInUser == null) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            var photolab = _photolabService.GetById(photolabId);

            if (photolab == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "photolab not find");

            var user = WebSiteGlobal.LoggedInUser;
            if (!user.IsAdministrator && user.CompanyAccountId != photolab.CompanyAccountId)
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "no access");

            var dpointsBindings = _shippingService.GetShippingBindings(photolab.CompanyAccount, loggedInUser)
                                          .Bindings?.FirstOrDefault(frontendBindings => frontendBindings.Key == photolab?.Id)
                                          .Value?.ShippingIds ?? Array.Empty<int>();

            var result = _shippingService.GetList<DistributionPoint>(photolab)
                .OrderBy(x => !dpointsBindings.Contains(x.Id))
                .Select(x => new DistributionPointDTO(x, WebSiteGlobal.UILanguage));

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [Route("api/shipping/shippingData")]
        public HttpResponseMessage GetShippingData(int shippingId)
        {
            var shipping = _shippingService.GetById<Shipping>(shippingId);
            if (shipping == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "shipping not found");

            var photolab = _photolabService.GetById(shipping.PhotolabId);

            var user = WebSiteGlobal.LoggedInUser;
            if (!user.IsAdministrator && user.CompanyAccountId != photolab.CompanyAccountId)
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "no access");

            var isSeparateAddress = false;
            var isClientDelivery = false;
            var isPostCodeRequired = false;
            IReadOnlyCollection<ShippingAddress> addresses = null;
            string shippingProviderType = null;
            if (shipping is Courier courier)
            {
                addresses = new List<ShippingAddress> { courier.Address };
                isClientDelivery = true;
                isSeparateAddress = true;
            }
            else if (shipping is DistributionPoint point)
            {
                addresses = new List<ShippingAddress> { point.Address };
            }
            else if (shipping is Postal postal)
            {
                addresses = postal.ShippingAddresses.ToList();
                isClientDelivery = postal.PostalType == PostalType.ToClientDelivery;
                isSeparateAddress = postal.IsMultipleAddressLines;
                isPostCodeRequired = postal.IsIndexRequired;
                shippingProviderType = postal.ShippingServiceProviderType.ToString();
            }

            var result = new
            {
                ShippingId = shipping.Id,
                IsClientDelivery = isClientDelivery,
                IsPostCodeRequired = isPostCodeRequired,
                IsSeparateAddress = isSeparateAddress,
                ShippingProviderType = shippingProviderType,
                Addresses = addresses?.Count > 0 ? new AddressHierarchyDto(addresses) : null
            };

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        [HttpGet]
        [Route("api/shipping/addressList")]
        public HttpResponseMessage GetShippingAddressList(int shippingId)
        {
            var shipping = _shippingService.GetById<Shipping>(shippingId);
            if (shipping == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "shipping not found");

            var photolab = _photolabService.GetById(shipping.PhotolabId);

            var user = WebSiteGlobal.LoggedInUser;
            if (!user.IsAdministrator && user.CompanyAccountId != photolab.CompanyAccountId)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "no access");
            }

            var isSeparateAddress = false;
            var isClientDelivery = false;
            IReadOnlyCollection<ShippingAddress> addresses = null;
            string shippingProviderType = null;
            if (shipping is Courier courier)
            {
                addresses = new List<ShippingAddress> { courier.Address };
                isClientDelivery = true;
                isSeparateAddress = true;
            }
            else if (shipping is DistributionPoint point)
            {
                addresses = new List<ShippingAddress> { point.Address };
            }
            else if (shipping is Postal postal)
            {
                isClientDelivery = postal.PostalType == PostalType.ToClientDelivery;
                isSeparateAddress = postal.ShippingServiceProviderType == ShippingServiceProviderType.Cdek;
                shippingProviderType = postal.ShippingServiceProviderType.ToString();
                addresses = postal.ShippingAddresses.ToList();
            }

            var result = new
            {
                ShippingId = shipping.Id,
                IsClientDelivery = isClientDelivery,
                IsSeparateAddress = isSeparateAddress,
                ShippingProviderType = shippingProviderType,
                Addresses = addresses?.Count > 0 ? new AddressHierarchyDto(addresses) : null
            };
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [Route("api/shipping/toggleaddress")]
        public HttpResponseMessage ToggleAddress(int shippingAddressId, bool status)
        {
            var shippingAddress = _shippingAddressService.GetById(shippingAddressId);
            shippingAddress.AvailableOnSite = status;
            var shipping = _shippingService.GetById<Shipping>(shippingAddress.ShippingId);
            _shippingAddressService.Update(shippingAddress, shipping);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("api/shipping/toggleaddresses")]
        public HttpResponseMessage ToggleAddresses(EnableShippingAddressesRequest request)
        {
            var loggedInUser = AuthenticationService.LoggedInUser;
            if (loggedInUser == null) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you are not logged in");

            var frontend = _photolabService.GetById(request.FrontendId);
            if (frontend == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "frontend is null");

            if (IsFrontendAccessDenied(loggedInUser, frontend, TeamMemberRole.Administrator, TeamMemberRole.Manager))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "access denied"); ;
            }

            if (request == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "request is empty");
            if (request.AddressesIds == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "request addressesIds is empty");
            if (request.Status == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "request status is empty");
            if (request.ShippingId == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "request shippingid is empty");

            var shipping = _shippingService.GetById<Shipping>(request.ShippingId);
            if (shipping == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "shipping not found");

            _shippingAddressService.UpdateListStatus(request.AddressesIds, shipping, request.Status);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        protected bool IsFrontendAccessDenied(User loggedInUser, Photolab frontend, params TeamMemberRole[] allowedRoles)
        {
            return loggedInUser.IsFrontendAccessDenied(frontend, allowedRoles);
        }

        public class EnableShippingAddressesRequest
        {
            public IReadOnlyCollection<int> AddressesIds { get; set; }
            public bool Status { get; set; }
            public int ShippingId { get; set; }
            public int FrontendId { get; set; }
        }
        [HttpGet]
        [Route("api/shipping/changestatus")]
        public HttpResponseMessage GetCartItem(int shippingId, bool status)
        {
            var shipping = _shippingService.GetById<Shipping>(shippingId);
            if (shipping == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "shipping not found");

            shipping.IsEnabled = status;
            _shippingService.Update(shipping);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("api/shipping/changemobilestatus")]
        public HttpResponseMessage ChangeMobileStatus(int shippingId, bool status)
        {
            var shipping = _shippingService.GetById<Shipping>(shippingId);
            if (shipping == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "shipping not found");

            shipping.IsEnabledForMobileApp = status;
            _shippingService.Update(shipping);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("api/shipping/getCalculatedPrice")]
        public HttpResponseMessage GetCartItem(int shippingId, int orderId)
        {
            var shipping = _shippingService.GetById<Shipping>(shippingId);
            if (shipping == null) Request.CreateResponse(HttpStatusCode.BadRequest, "shipping not found");

            var order = _orderService.GetById(orderId);
            var items = _orderDetailService.GetListByOrder(order).ToArray();
            var calculationResult = _shippingCalculatorService.GetShippingPriceWithoutDiscount(shipping, order.DeliveryAddress, items);
            if (!calculationResult.Success)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    DeliveryExceptionMessages.PriceCalculationError);
            return Request.CreateResponse(HttpStatusCode.OK, calculationResult);
        }
        [HttpPost]
        [Route("api/shipping/price")]
        public HttpResponseMessage GetShippingPrice(ShippingPriceRequestDTO request)
        {
            var loggedInUser = AuthenticationService.LoggedInUser;
            if (loggedInUser == null) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "you is not logged in");
            var shipping = _shippingService.GetById<Shipping>(request.ShippingAddress.ShippingId);
            var photolab = _photolabService.GetById(shipping.PhotolabId);

            if (photolab == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "photolab not find");

            var user = WebSiteGlobal.LoggedInUser;
            if (!user.IsAdministrator && user.CompanyAccountId != photolab.CompanyAccountId)
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "no access");
            if (string.IsNullOrWhiteSpace(request.weight)) request.weight = "0";
            request.weight = request.weight.Replace(',', '.').Replace(" ", string.Empty).Trim();

            var item = new TestShippableItem(double.Parse(request.weight, CultureInfo.InvariantCulture));

            var sAddress = _frontendShippingService.GetSelectedOrderAddress(
                new DeliveryFinalState()
                {
                    ShippingData = request.ShippingAddress
                }, shipping);
            var calculationResult = _shippingCalculatorService.GetShippingPriceWithoutDiscount(shipping, sAddress, new[] { item });
            if (!calculationResult.Success)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    DeliveryExceptionMessages.PriceCalculationError);
            var result = new
            {
                success = calculationResult.Success,
                price = calculationResult.Price,
                properties = calculationResult.Properties
            };
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        private ShippingAddress GetSelectedShippingAddress(Shipping shipping, AddressRequestDTO dto)
        {
            switch (shipping.Type)
            {
                case ShippingType.Postal:
                    switch (shipping.ShippingServiceProviderType)
                    {
                        case ShippingServiceProviderType.YandexDelivery:
                            return new ShippingAddress
                            {
                                ShippingId = shipping.Id,
                                Country = dto.Country,
                                Region = dto.Region,
                                City = dto.City,
                                Street = dto.Street,
                                House = dto.House,
                                PostalCode = dto.PostalCode,
                                DeliveryProperties =
                                    JsonConvert.DeserializeObject<DeliveryAddressProperties>(dto.DeliveryProperties.ToString()) ??
                                    new DeliveryAddressProperties()
                            };
                        default:
                            return dto.Id.HasValue? _shippingAddressService.GetById(dto.Id.Value):
                                    null;
                    }
                case ShippingType.Point:
                    return dto.Id.HasValue? _shippingAddressService.GetById(dto.Id.Value):
                        null;
                case ShippingType.Courier:
                case ShippingType.Unknown:
                default:
                    return new ShippingAddress
                    {
                        ShippingId = shipping.Id,
                        Country = dto.Country,
                        Region = dto.Region,
                        City = dto.City,
                        Street = dto.Street,
                        House = dto.House,
                        DeliveryProperties =
                            JsonConvert.DeserializeObject<DeliveryAddressProperties>(dto.DeliveryProperties.ToString()) ??
                            new DeliveryAddressProperties()
                    };
            }
        }

        [HttpPost]
        [Route("api/shipping/syncAddress")]
        public HttpResponseMessage SyncShippingAdresses(int shippingId)
        {
            var shipping = _shippingService.GetById<Postal>(shippingId);
            if (shipping == null) return Request.CreateResponse(HttpStatusCode.BadRequest);

            var photolab = _photolabService.GetById(shipping.PhotolabId);
            if (!AuthenticationService.LoggedInUser.IsAdministrator && !AuthenticationService.LoggedInUser.IsFrontendAdministrator(photolab))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            var providerService = _shippingProviderResolver.GetProvider(shipping);
            if (providerService == null) return Request.CreateResponse(HttpStatusCode.BadRequest);

            var result = providerService.SyncAddresses(photolab, shipping);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, result.ErrorMessage);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public class CdekRegistrationCourierCall
        {
            public string[] OrderIds { get; set; }
            public int CurrentPostalId { get; set; }
            public int CurrentOrderId { get; set; }
            public string Year { get; set; }
            public string Month { get; set; }
            public string Day { get; set; }
            public string FromHour { get; set; }
            public string FromMinute { get; set; }
            public string ToHour { get; set; }
            public string ToMinute { get; set; }
            public string DPointId { get; set; }
            public string UserId { get; set; }
            public string Commentary { get; set; }
        }

        [HttpPost, Route("api/shipping/cdek/validateCallCourierData")]
        public HttpResponseMessage CheckValidationOfData(CdekRegistrationCourierCall request)
        {
            var sb = new StringBuilder();

            DistributionPoint dPoint = null;
            User user = null;

            if (int.TryParse(request.DPointId, out var dpointId))
            {
                dPoint = _shippingService.GetById<DistributionPoint>(dpointId);
                if (dPoint == null)
                {
                    sb.Append("- Выбранная точка выдачи не найдена</br>");
                }
                else
                {
                    if (string.IsNullOrEmpty(dPoint.Address.Street) || string.IsNullOrEmpty(dPoint.Address.House) ||
                        string.IsNullOrEmpty(dPoint.Address.Flat))
                        sb.Append("- У выбранной точки выдачи не заполнены все адресные поля</br>");
                }
            }
            else
            {
                sb.Append("- У выбранной точки выдачи невалидный Id</br>");
            }

            if (int.TryParse(request.UserId, out var userId))
            {
                user = _userService.GetById(userId);
                if (user == null)
                {
                    sb.Append("- Выбранный пользователь не найден</br>");
                }
                else
                {
                    if (user.IsPhoneEmpty())
                    {
                        sb.Append("- У выбранного пользователя не указан номер телефона</br>");
                    }
                }
            }
            else
            {
                sb.Append("- У выбранного пользователя невалидный Id</br>");
            }

            var edgeDay = DateTime.Today + TimeSpan.FromDays(1);
            var date = DateInfoController.GetDateTime(request.Day, request.Month, request.Year);
            if (date < edgeDay)
                sb.Append("- Дата вызова курьера не может быть раньше чем от " + edgeDay.ToString("dd/MM/yyyy") +
                          "</br>");

            var timeStart = TimeInfoController.GetTime(request.FromHour, request.FromMinute, date);
            var timeEnd = TimeInfoController.GetTime(request.ToHour, request.ToMinute, date);
            if (timeEnd.Subtract(timeStart).Hours < 3)
                sb.Append("- Период ожидания курьера должен быть не менее трех часов</br>");

            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    "При заполнении формы возникли следующие ошибки:</br>" + sb);
            }

            var order = _orderService.GetById(request.CurrentOrderId);
            if (order == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Заказ не найден</br>");

            var postal = _shippingService.GetById<Postal>(request.CurrentPostalId);
            if (postal == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Транспортная компания не найдена</br>");

            if (_shippingProviderResolver.GetProvider(postal) is CdekProviderService cdekService)
            {
                return cdekService.RegisterCallCourierToPostal(order, dPoint, user,
                    postal.ServiceProviderSettings, timeStart, timeEnd, request.Commentary)
                    ? Request.CreateResponse(HttpStatusCode.OK, "Успешно")
                    : Request.CreateResponse(HttpStatusCode.BadRequest,
                        "Ошибка в создании заявки. Подробности в истории заказа</br>");
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, "Ошибка в создании заявки</br>");
        }

        [HttpPost, Route("api/shipping/cdek/validateCallCourierDataOrders")]
        public HttpResponseMessage CheckValidationOfDataOrders(CdekRegistrationCourierCall request)
        {
            var sb = new StringBuilder();

            DistributionPoint dPoint = null;
            User user = null;

            if (int.TryParse(request.DPointId, out var dpointId))
            {
                dPoint = _shippingService.GetById<DistributionPoint>(dpointId);
                if (dPoint == null)
                {
                    sb.Append("- Выбранная точка выдачи не найдена</br>");
                }
                else
                {
                    if (string.IsNullOrEmpty(dPoint.Address.Street) || string.IsNullOrEmpty(dPoint.Address.House) ||
                        string.IsNullOrEmpty(dPoint.Address.Flat))
                        sb.Append("- У выбранной точки выдачи не заполнены все адресные поля</br>");
                }
            }
            else
            {
                sb.Append("- У выбранной точки выдачи невалидный Id</br>");
            }

            if (int.TryParse(request.UserId, out var userId))
            {
                user = _userService.GetById(userId);
                if (user == null)
                {
                    sb.Append("- Выбранный пользователь не найден</br>");
                }
                else
                {
                    if (user.IsPhoneEmpty())
                    {
                        sb.Append("- У выбранного пользователя не указан номер телефона</br>");
                    }
                }
            }
            else
            {
                sb.Append("- У выбранного пользователя невалидный Id</br>");
            }

            var edgeDay = DateTime.Today + TimeSpan.FromDays(1);
            var date = DateInfoController.GetDateTime(request.Day, request.Month, request.Year);
            if (date < edgeDay)
                sb.Append("- Дата вызова курьера не может быть раньше чем от " + edgeDay.ToString("dd/MM/yyyy") +
                          "</br>");

            var timeStart = TimeInfoController.GetTime(request.FromHour, request.FromMinute, date);
            var timeEnd = TimeInfoController.GetTime(request.ToHour, request.ToMinute, date);
            if (timeEnd.Subtract(timeStart).Hours < 3)
                sb.Append("- Период ожидания курьера должен быть не менее трех часов</br>");

            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "При заполнении формы возникли следующие ошибки:</br>" + sb);
            }

            var orderIds = request.OrderIds.Select(x => Int32.Parse(x)).ToArray();
            var orders = _orderService.GetListByIds(orderIds);
            var order = orders.FirstOrDefault();
            if (order == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Заказ не найден</br>");

            var postal = _shippingService.GetById<Postal>(order.Shipping.Id);
            if (postal == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Транспортная компания не найдена</br>");

            if (_shippingProviderResolver.GetProvider(postal) is CdekProviderService cdekService)
            {
                return cdekService.RegisterCallCourierToPostal(order, dPoint, user,
                    postal.ServiceProviderSettings, timeStart, timeEnd, request.Commentary)
                    ? Request.CreateResponse(HttpStatusCode.OK, "Успешно")
                    : Request.CreateResponse(HttpStatusCode.BadRequest,
                        "Ошибка в создании заявки. Подробности в истории заказа</br>");
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, "Ошибка в создании заявки</br>");
        }

        public class OrdersToPostalRequest
        {
            public int[] OrderIds { get; set; }
            public int Type { get; set; }
        }

        [HttpPost, Route("api/shipping/sendOrderToPostal")]
        public HttpResponseMessage SendOrdersToPostal(OrdersToPostalRequest request)
        {
            try
            {
                if (request?.OrderIds == null || request.OrderIds.Length == 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "request is empty");
                var orders = _orderService.GetListByIds(request.OrderIds);
                var orderToSend = orders.FirstOrDefault();
                if (orderToSend != null)
                {
                    var photolab = _photolabService.GetById(orderToSend.PhotolabId);
                    var relations = _orderRelationService.GetByOrderId(orderToSend.Id);
                    var correctRelations = relations.OrderRelations.Where(x =>
                        x.Type == OrderRelationType.Link && x.OrderFirstId == orderToSend.Id);

                    if (request.Type == 2 && correctRelations.Count(x => x.Type == OrderRelationType.Link) == orders.Count - 1 &&
                        relations.OrderRelations.All(x => request.OrderIds.Contains(x.OrderSecondId)))
                    {
                        if (orderToSend.Shipping as Postal != null)
                        {
                            var commentary = $"Забрать заказы: {string.Join(",", orders.Select(o => o.CustomId))}";
                            var provider = _shippingProviderResolver.GetProvider(orderToSend.Shipping as Postal);
                            provider.GetCreateOrderRegistration(photolab, orderToSend,
                                orderToSend.Shipping.ServiceProviderSettings, commentary, (ShippingRegisterType)request.Type);
                        }
                    }
                    else
                    {
                        foreach (var order in orders)
                        {
                            if (order.Shipping as Postal != null)
                            {
                                var provider = _shippingProviderResolver.GetProvider(order.Shipping as Postal);
                                provider.GetCreateOrderRegistration(photolab, order,
                                    order.Shipping.ServiceProviderSettings);
                            }
                        }
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Заказы не найдены");
                }

                return Request.CreateResponse(HttpStatusCode.OK, "Заказы зарегистрированы");
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Ошибка в ходе регистрации");
            }
        }

        [HttpPost, Route("api/shipping/omniva/sendAddressCardToEmail")]
        public HttpResponseMessage SendOmnivaAddressCardToEmail(int? orderId, string email)
        {
            try
            {
                if (!orderId.HasValue) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");
                var order = _orderService.GetById(orderId.Value);
                var success = _omnivaService.SendOrderCardPdfToEmail(order, order.Shipping.ServiceProviderSettings, email);

                if (success)
                    return Request.CreateResponse(HttpStatusCode.OK, "address card was successfully sent");
                else
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "error while sending address card");
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "error while sending address card");
            }
        }
        [HttpGet, Route("api/shipping/pickpoint/getPickpointOrderLabelPdf")]
        public HttpResponseMessage GetPickpointOrderLabelPdf(int? orderId)
        {
            try
            {
                if (!orderId.HasValue) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");
                var order = _orderService.GetById(orderId.Value);
                var success = _pickpointService.GetPdfLabel(order, order.Shipping.ServiceProviderSettings);

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(success);
                var base64 = Convert.ToBase64String(success);

                if (base64.Length > 0)
                    return Request.CreateResponse(HttpStatusCode.OK, base64);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "error while sending address card");
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "error while sending address card");
            }
        }
    }
}