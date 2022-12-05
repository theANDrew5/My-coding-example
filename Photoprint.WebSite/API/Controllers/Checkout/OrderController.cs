using Photoprint.Core;
using Photoprint.Core.Configuration;
using Photoprint.Core.InputModels;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using Photoprint.Services;
using Photoprint.WebSite.API.Models;
using Photoprint.WebSite.API.Models.Account;
using Photoprint.WebSite.Modules;
using Photoprint.WebSite.Shared;
using Photoprint.WebSite.Views.OAuth.Code;
using Photoprint.WebSite.Views.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using File = Photoprint.Core.Models.File;

namespace Photoprint.WebSite.API.Controllers.Discounts
{
    public partial class OrderController : BaseApiController
    {
        private readonly IPhotolabService _photolabService;
        private readonly IFrontendShippingService _frontendShippingService;
        private readonly IFrontendOrderService _frontendOrderService;
        private readonly IOrderTextService _orderTextService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IOrderInvoiceService _orderInvoiceService;
        private readonly IUserService _userService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IFileService _fileService;
        private readonly IFrontendMaterialTypeService _frontendMaterialTypeService;
        private readonly IFrontendMaterialService _frontendMaterialService;
        private readonly IPhotolabDomainService _domainService;
        private readonly IOrderSearchService _orderSearchService;
        private readonly IFrontendPaymentService _frontendPaymentService;
        private readonly ISmsService _smsService;
        private readonly IFrontendCustomWorkService _frontendCustomWorkService;
        private readonly IUserCompanyService _userCompanyService;
        private readonly ILanguageService _languageService;
        private readonly IPhotolabSettingsService _photolabSettingsService;
        private readonly IProcessPaymentService _processPaymentService;
        private readonly IOrderCloneService _orderCloneService;
        private readonly IOrderService _orderService;

        public OrderController(
            IAuthenticationService authenticationService,
            IPhotolabService photolabService,
            IOrderDetailService orderDetailService,
            IFrontendShippingService frontendShippingService,
            IFrontendOrderService frontendOrderService,
            IOrderInvoiceService orderInvoiceService,
            IUserService userService,
            IFileService fileService,
            IFrontendMaterialTypeService frontendMaterialTypeService,
            IFrontendPaymentService frontendPaymentService,
            IFrontendMaterialService frontendMaterialService,
            ISmsService smsService,
            IShoppingCartService shoppingCartService,
            IPhotolabDomainService domainService,
            IOrderSearchService orderSearchService,
            IOrderTextService orderTextService,
            IFrontendCustomWorkService frontendCustomWorkService,
            IUserCompanyService userCompanyService,
            ILanguageService languageService,
            IPhotolabSettingsService photolabSettingsService,
            IProcessPaymentService processPaymentService,
            IOrderCloneService orderCloneService,
            IOrderService orderService) : base(authenticationService)
        {
            _photolabService = photolabService;
            _frontendShippingService = frontendShippingService;
            _frontendOrderService = frontendOrderService;
            _orderDetailService = orderDetailService;
            _orderInvoiceService = orderInvoiceService;
            _userService = userService;
            _fileService = fileService;
            _frontendPaymentService = frontendPaymentService;
            _frontendMaterialTypeService = frontendMaterialTypeService;
            _frontendMaterialService = frontendMaterialService;
            _smsService = smsService;
            _shoppingCartService = shoppingCartService;
            _domainService = domainService;
            _orderSearchService = orderSearchService;
            _orderTextService = orderTextService;
            _frontendCustomWorkService = frontendCustomWorkService;
            _userCompanyService = userCompanyService;
            _languageService = languageService;
            _photolabSettingsService = photolabSettingsService;
            _processPaymentService = processPaymentService;
            _orderCloneService = orderCloneService;
            _orderService = orderService;
        }

        [HttpGet]
        [Route("api/orders/{orderId}/ecommerceStatus")]
        public HttpResponseMessage ChangeEcommerceStatus(ECommerceType type, int orderId)
        {
            var order = _frontendOrderService.GetById(orderId);
            if (order != null && order.UserId == AuthenticationService.LoggedInUser?.Id)
            {
                _frontendOrderService.UpdateECommerceStatus(order, ECommerceStatus.Sent, type);
                return Request.CreateResponse(HttpStatusCode.OK, "marked as sent");
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "no access or order null");
        }


        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "POST")]
        [Route("api/order/create-with-ids")]
        public HttpResponseMessage CreateOrder(ExtendedOrderCreateRequest request)
        {
            if (request == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad request");

            var response = CheckAuthData(request.FrontendId, request.User, out var photolab, out var user);
            if (response != null) return response;

            if (request.Items == null || request.Items.Count == 0) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format("Request does not contains Items"));

            var materials = new List<ShoppingCartItem>();

            foreach (var materialData in request.Items)
            {
                var material = _frontendMaterialService.GetById(materialData.MaterialId);
                if (material == null) continue;

                var materialType = _frontendMaterialTypeService.GetById(material.MaterialTypeId);
                if (materialType == null) continue;

                var mQuantity = materialData.Quantity;
                material.CirculationSettings.GetMatchedRule(materialData.Quantity).TryGetPrice(mQuantity, out var circulation);
                if (materialData.ItemPartsQuantity > 0) circulation.ItemPartsCount = materialData.ItemPartsQuantity;

                var item = new MaterialShoppingCartItem(materialType, material, circulation, null, EditorType.None)
                {
                    ItemWeight = 0,
                    SourceType = ShoppingCartItemSourceType.QuickCustomForm,
                    AdminComment = request.Description
                };

                if (Settings.CompanyAccountIdsWithPosibilityToCreateOrderFromExternalSite.Contains(photolab.CompanyAccountId))
                {
                    request.Description = request.User.FullInfo;
                }

                if (materialData.Options != null)
                {
                    var selectedItems = materialData.Options.SelectMany(x => x.Items).AsList();
                    var additionalPriceItems = _frontendCustomWorkService.GetAdditionalPriceItemByState(selectedItems, new ProductInfoData(material), photolab, item, out _, out _);

                    foreach (var additionalPriceItem in additionalPriceItems)
                    {
                        additionalPriceItem.IsQuantityChangable = true;
                    }

                    if (additionalPriceItems.Count > 0)
                    {
                        item.Properties.AdditionalPriceItems.AddRange(additionalPriceItems);
                        _frontendCustomWorkService.RecalculateAdditionalPriceItems(item);
                    }

                    item.Properties.AdditionalPriceItemsIsProcessed = true;
                }

                materials.Add(item);
            }

            OrderCommentInput orderCommentInputList = null;
            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                var ip = SiteUtils.GetIpAddress(new HttpRequestWrapper(HttpContext.Current.Request));
                orderCommentInputList = new OrderCommentInput(null, null, ip, request.Description, true);
            }

            var attachments = GetFiles(request.Files);

            return CreateCustomOrder(photolab, user, materials, attachments, orderCommentInputList, request.Number, Array.Empty<OrderDetailInput>(), Array.Empty<string>());
        }

        [HttpPost]
        [Route("api/order/create")]
        public HttpResponseMessage CreateOrderByQuickCustomForm(CommonOrderCreateRequest request)
        {
            if (request is null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad request");

            var response = CheckAuthData(request.FrontendId, request.User, out var photolab, out var user);
            if (response != null) return response;

            if (request.Items is null || request.Items.Count == 0)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format("Request does not contains Items"));

            var language = photolab.DefaultLanguage;
            var items = new List<ShoppingCartItem>(request.Items.Count);
            var orderDetailInputs = new List<OrderDetailInput>(request.Items.Count);
            var orderDetailTitles = new List<string>(request.Items.Count);

            foreach (var item in request.Items)
            {
                var cartItem = new ShoppingCartItem(item.Quantity)
                {
                    ItemPrice = item.ItemPrice,
                    Name = item.Title,
                    ItemWeight = 0,
                    SourceType = ShoppingCartItemSourceType.QuickCustomForm
                };

                if (!string.IsNullOrEmpty(item.Category) && !string.IsNullOrEmpty(item.Material))
                {
                    var materialType = _frontendMaterialTypeService.GetByUrl(item.Category, language);
                    if (materialType != null)
                    {
                        var material = _frontendMaterialService.GetByType(materialType, item.Material, language);
                        if (material != null)
                        {
                            orderDetailInputs.Add(new OrderDetailInput
                            {
                                //order added after create
                                Language = language,
                                Price = item.ItemPrice,
                                Quantity = item.Quantity,
                                Material = material,
                                CustomEditorType = EditorType.None // это для произвольного заказа
                            });
                            orderDetailTitles.Add($"{materialType.Title} ({material.Title})");
                            continue;
                        }
                    }

                    cartItem.AdminComment = request.Description;
                }

                if (item.Options != null && item.Options.Count > 0)
                {
                    foreach (var option in item.Options)
                    {
                        var additionalPriceItem = new AdditionalPriceItem(0, option.Title, option.Description, option.ItemPrice, 0, null, PriceFormatType.Multiple, option.Quantity);
                        additionalPriceItem.IsQuantityChangable = true;
                        cartItem.Properties.AdditionalPriceItems.Add(additionalPriceItem);
                    }
                }
                items.Add(cartItem);
            }

            OrderCommentInput orderCommentInputList = null;
            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                var ip = SiteUtils.GetIpAddress(new HttpRequestWrapper(HttpContext.Current.Request));
                orderCommentInputList = new OrderCommentInput(null, null, ip, request.Description, true);
            }

            var attachments = GetFiles(request.Files);

            return CreateCustomOrder(photolab, user, items, attachments, orderCommentInputList, request.Number, orderDetailInputs, orderDetailTitles);
        }

        private HttpResponseMessage CheckAuthData(int? frontendId, UserCreateRequest userData, out Photolab photolab, out User user)
        {
            user = null;

            if (frontendId != null)
            {
                photolab = _photolabService.GetById(frontendId.Value);
            }
            else
            {
                photolab = WebSiteGlobal.CurrentPhotolab;
            }
            if (photolab == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format("Frontend {0} not found", frontendId));
            }

            var response = TryGetUser(userData, photolab, out user);
            if (user == null)
            {
                return response;
            }

            // только новый пользователь может авторизоваться
            AuthenticationService.SignIn(user, true, photolab);

            return null;
        }
        private HttpResponseMessage TryGetUser(UserCreateRequest request, Photolab frontend, out User user)
        {
            user = null;

            if (AuthenticationService.LoggedInUser != null && !AuthenticationService.LoggedInUser.IsAnonymous)
            {
                user = AuthenticationService.LoggedInUser;
                return null;
            }

            if (request != null)
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Phone))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format("User email or phone is empty"));
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    var userFromDB = _userService.GetByEmail(request.Email, frontend.CompanyAccount);
                    if (userFromDB == null)
                    {
                        userFromDB = _userService.GetByPhone(request.Phone, frontend.CompanyAccount);
                        if (userFromDB == null)
                        {
                            user = _userService.CreateOrGetClient(frontend.CompanyAccount, frontend, request.FirstName, request.LastName, request.Email, request.Phone);
                            return null;
                        }
                    }

                    // если аккаунт добавлен в список с возможным созданием извне - чекаем дефолтного пользователя
                    if (Settings.CompanyAccountIdsWithPosibilityToCreateOrderFromExternalSite.Contains(frontend.CompanyAccountId))
                    {
                        var defaultSettings = _photolabSettingsService.GetSettings<OrderPredefinedSettings>(frontend);
                        if (defaultSettings?.DefaultUserId != null)
                        {
                            var defaultUser = _userService.GetById(defaultSettings.DefaultUserId.Value);
                            if (defaultUser != null)
                            {
                                user = defaultUser;
                                return null;
                            }
                        }
                    }

                    return Request.CreateErrorResponse(HttpStatusCode.Forbidden, string.Format("User is already exist"));
                }
            }

            if (user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, string.Format("User search/create error"));
            }

            return null;
        }

        private IReadOnlyCollection<File> GetFiles(IReadOnlyCollection<int> filesIds)
        {
            if (filesIds == null || filesIds.Count == 0) return Array.Empty<File>();

            var attachments = new List<File>(filesIds.Count);
            foreach (var fileId in filesIds)
            {
                var file = _fileService.GetById(fileId);
                if (file == null) continue;

                attachments.Add(file);
            }

            return attachments;
        }

        private HttpResponseMessage CreateCustomOrder(Photolab photolab, User user, IReadOnlyCollection<ShoppingCartItem> items, 
            IReadOnlyCollection<File> attachments, OrderCommentInput description, string number,
            IReadOnlyCollection<OrderDetailInput> orderDetailInputs, IReadOnlyCollection<string> orderDetailTitles)
        {

            var createdOrder = _frontendOrderService.CreateByCustomFrontendForm(photolab, user, items, attachments, orderDetailTitles, description, null, number, true);
            if (createdOrder == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad request");
            }

            SetWebAnalyticsClientId(createdOrder, GetHttpContext(Request));

            if (orderDetailInputs.Count > 0)
            {
                foreach (var orderDetailInput in orderDetailInputs)
                {
                    // add order to input
                    orderDetailInput.Order = createdOrder;
                    _orderDetailService.AddToOrder(orderDetailInput);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { OrderId = createdOrder.Id });
        }

        [HttpGet]
        [ApiAuthorize]
        [Route("api/order/orders/{orderId}/info")]
        public HttpResponseMessage GetOrderInfo(int orderId, int languageId = 0)
        {
            var language = WebSiteGlobal.CurrentPhotolab.Languages.FirstOrDefault(x => x.Id == languageId) ?? WebSiteGlobal.CurrentLanguage;

            var order = _frontendOrderService.GetById(orderId);
            if (order is null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Order not found");

            var availableStatuses = _frontendOrderService.GetAvailableFrontendStatuses(order, AuthenticationService.LoggedInUser);

            var result = new List<KeyValuePair<int, string>>(availableStatuses.Count);
            foreach (var status in availableStatuses)
            {
                result.Add(new KeyValuePair<int, string>((int)status, _orderTextService.GetOrderFrontendStatusTitle(WebSiteGlobal.CurrentPhotolab, status, language)));
            }
            var statuses = result.Select(x => new { x.Key, x.Value }).ToArray();

            var settings = _photolabSettingsService.GetSettings<PaymentSystemsSettings>(WebSiteGlobal.CurrentPhotolab);
            if (settings?.AllowRefundsInWebsite ?? false)
            {
                var invoices = _orderInvoiceService
                    .GetList(order)
                    .Where(i => i.PaymentSystem.Type == PaymentSystemType.Gateway
                    && i.Status == OrderInvoiceStatus.Paid
                    && settings.RefundableCheck(i.PaymentSystem.UniqueId))
                    .Select(i => new { i.Id, i.PaymentSystem.Title, Price = Utility.GetPriceStringFormated(i.Price, WebSiteGlobal.CurrentPhotolab) });
                return Request.CreateResponse(HttpStatusCode.OK, new { invoices, statuses });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { statuses });
        }

        [HttpPost]
        [ApiAuthorize]
        [Route("api/order/orders/{orderId}/status")]
        public HttpResponseMessage SetOrderStatus(int orderId, OrderChangeStatusRequest request)
        {
            var order = _frontendOrderService.GetById(orderId);
            if (order is null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Order not found");
            OrderStatus newStatus;
            try
            {
                newStatus = (OrderStatus)request.NewStatusId;
            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad Request. Status is not correct");
            }

            if (!_frontendOrderService.GetAvailableFrontendStatuses(order, AuthenticationService.LoggedInUser).Contains(newStatus))
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Access Denied");

            _processPaymentService.Refund(order, AuthenticationService.LoggedInUser.FullName, isAdmin: false);

            _frontendOrderService.SetStatus(order, newStatus, AuthenticationService.LoggedInUser, "Changed via frontend", true, false);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }


        [HttpGet]
        [Route("api/order/status")]
        public HttpResponseMessage UpdateNetprintProductionStatus([FromUri] NetprintStatusRequest request)
        {
            if (request?.Order_id == null || request.State_id == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad request");

            //var photolabs = _photolabService.GetList(WebSiteGlobal.CurrentCompanyAccount); TODO нету никакой проверки на секьюрность! (в бо настроен на photomax.ru)
            var order = _orderSearchService.GetListWithSerchByPhotoexpertId(request.Order_id).FirstOrDefault();

            if (order == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad request");

            if (int.TryParse(request.State_id, out var statusCode))
            {
                var productionStatus = NetprintProductionStatus.Unknown;
                if (Enum.IsDefined(typeof(NetprintProductionStatus), statusCode))
                {
                    productionStatus = (NetprintProductionStatus)statusCode;
                }
                else if (statusCode > 400 && statusCode <= 599)
                {
                    productionStatus = NetprintProductionStatus.Errors;
                }
                if (productionStatus != NetprintProductionStatus.Unknown)
                {
                    try
                    {
                        _frontendOrderService.SetStatus(order, productionStatus);
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                }
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad request");
        }

        [HttpPost]
        [ApiAuthorize]
        [Route("api/order/quickOrderCreate")]
        public HttpResponseMessage QuickOrderCreate(QuickCreateOrderRequest request)
        {
            if (WebSiteGlobal.LoggedInUser?.IsAnonymous != false)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Not Authorisated");

            if (request.PhotolabId is null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "PhotolabId required");

            var lab = _photolabService.GetById(request.PhotolabId.Value);
            if (lab is null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Photolab not found");

            var quickOrderSettings = _photolabSettingsService.GetSettings<PhotolabQuickOrderSettings>(lab);
            if (quickOrderSettings is null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Quick order settings not found");

            if (quickOrderSettings.DistributionPointIsRequired && request.ShippingId is null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Shipping required");

            var shipping = request.ShippingId != null ? _frontendShippingService.GetById<Shipping>(request.ShippingId.Value) : null;
            if (quickOrderSettings.DistributionPointIsRequired && shipping is null)
            {
                var distributionPoints = _frontendShippingService.GetAvailableList<DistributionPoint>(lab);
                if (distributionPoints.Count == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Shipping not found");
            }

            var user = WebSiteGlobal.LoggedInUser;
            var cart = _shoppingCartService.GetCart(user, lab);
            if (cart.Items.Count == 0)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "EmptyCart");
            }
            if (request.ItemIds != null && request.ItemIds.Count() != cart.Items.Count)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "CheckCart");
            }

            var isChanged = false;
            if (!string.IsNullOrEmpty(request.FirstName) && !user.FirstName.Equals(request.FirstName))
            {
                user.FirstName = request.FirstName;
                isChanged = true;
            }
            if (!string.IsNullOrEmpty(request.LastName) && string.IsNullOrEmpty(user.LastName) || !user.LastName.Equals(request.LastName))
            {
                user.LastName = request.LastName;
                isChanged = true;
            }
            if (!string.IsNullOrEmpty(request.AdmitadId))
            {
                user.Properties.AdmitadUserId = request.AdmitadId;
                isChanged = true;
            }
            if (isChanged)
            {
                _userService.Update(user);
            }

            var materialItems = cart.Items.Select(i => i as MaterialShoppingCartItem);

            var isForbiddenItemsExist = quickOrderSettings.BindedMaterials.Count > 0 &&
                materialItems.Any(x => quickOrderSettings.BindedMaterials.All(t => x?.MaterialId != t));

            var isInsidePriceBoundaries = !(
                (quickOrderSettings.MinShoppingCartPrice != null && quickOrderSettings.MinShoppingCartPrice > cart.TotalPrice)
                || (quickOrderSettings.MaxShoppingCartPrice != null && quickOrderSettings.MaxShoppingCartPrice < cart.TotalPrice));

            if (!isInsidePriceBoundaries || isForbiddenItemsExist) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "");

            var userCompany = _userCompanyService.GetByUser(user)?.UserCompany;

            var quickOrderItems = cart.Items.Select(item => { item.SourceType = ShoppingCartItemSourceType.QuickOrder; return item; });

            var order = _frontendOrderService.CreateByQuickForm(lab, user, quickOrderItems, shipping, null, userCompany);

            cart.Items.Clear();
            _shoppingCartService.SaveCart(cart);

            SetWebAnalyticsClientId(order, GetHttpContext(Request));

            var payment = _frontendPaymentService.GetList(lab).ToList().FirstOrDefault(x => x.PaymentSystem.Type == PaymentSystemType.Cash);
            var domain = _domainService.GetDefaultDomain(lab);

            var url = new UrlManager(domain) { CurrentOrderId = order.Id }.GetHRefUrl(SiteLinkType.UserOrder);
            return Request.CreateResponse(HttpStatusCode.OK, new { link = url });
        }


        private void SetWebAnalyticsClientId(Order order, HttpContextWrapper context)
        {
            if (context == null) return;

            var gacid = context.Request.Cookies["_ga"]?.Value ?? string.Empty;
            if (!string.IsNullOrEmpty(gacid))
            {
                var gaClientId = Regex.Match(gacid, "([0-9]+.[0-9]+)$").Value;
                if (!string.IsNullOrEmpty(gaClientId))
                {
                    order.Properties.GoogleAnalyticsClientId = gaClientId;
                }
            }
            var yaClientId = context.Request.Cookies["_ym_uid"]?.Value ?? string.Empty;
            if (!string.IsNullOrEmpty(yaClientId))
            {
                order.Properties.YandexMetrikaClientId = yaClientId;
            }
            _frontendOrderService.UpdateProperties(order);
        }

        [HttpPost]
        [ApiAuthorize]
        [Route("api/order/{orderId}/updatePrice")]
        public HttpResponseMessage UpdateUserDiscount(int orderId, OrderUpdateRequest request)
        {
            var user = AuthenticationService.LoggedInUser;
            var isPrivilegesElevated = user.IsAdministrator
                                       || user.IsInFrontendTeam(WebSiteGlobal.CurrentPhotolab, TeamMemberRole.Administrator)
                                       || user.IsOperator(WebSiteGlobal.CurrentPhotolab)
                                       || user.IsInFrontendTeam(WebSiteGlobal.CurrentPhotolab,
                                           TeamMemberRole.Manager);
            var wasUpdated = false;
            if (isPrivilegesElevated)
            {
                var order = _frontendOrderService.GetById(orderId);
                if (order != null)
                {
                    if (Math.Abs(order.DiscountPrice - request.NewDiscount) > 0.01m)
                    {
                        order.DiscountPrice = request.NewDiscount;
                        wasUpdated = true;
                    }
                    var withdrawAmount = request.NewPrice - order.PaidPrice;
                    if (withdrawAmount > 0.01m) // Доплата
                    {
                        var invoice = _orderInvoiceService.GetList(order).FirstOrDefault();
                        var payment = _frontendPaymentService.GetAvailableList(WebSiteGlobal.CurrentPhotolab, order, null, WebSiteGlobal.CurrentPhotolab.DefaultLanguage).FirstOrDefault();
                        if (payment == null)
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.Forbidden,
                                "Seems you have no appropriate payment system to proceed this order. Please contact the local administrator.");
                        }
                        if (invoice == null || invoice.Price < withdrawAmount + invoice.PaidPrice)
                        {
                            invoice = _orderInvoiceService.CreateNew(order, payment, withdrawAmount);
                        }
                        var amountToPay = invoice.PaidPrice > 0 // инвойс может быть уже частично оплачен
                            ? withdrawAmount + invoice.PaidPrice
                            : withdrawAmount;
                        invoice.IsAdminModifyPrice = request.IsAdminModifyPrice;
                        _orderInvoiceService.UpdatePaidPrice(invoice, amountToPay);
                    }
                    else if (withdrawAmount < 0.01m) // списать средства с клиента
                    {
                        withdrawAmount = Math.Abs(withdrawAmount); // Нам проще работать с положительными значениями
                        var invoices = _orderInvoiceService.GetList(order);
                        foreach (var orderInvoice in invoices)
                        {
                            if (orderInvoice.PaidPrice > withdrawAmount)
                            {
                                orderInvoice.IsAdminModifyPrice = request.IsAdminModifyPrice;
                                _orderInvoiceService.UpdatePaidPrice(orderInvoice, orderInvoice.PaidPrice - withdrawAmount);
                                withdrawAmount = 0;
                                break;
                            }
                            else
                            {
                                withdrawAmount -= orderInvoice.PaidPrice;
                                _orderInvoiceService.UpdatePaidPrice(orderInvoice, 0);
                            }
                        }
                    }
                    if (wasUpdated)
                    {
                        _frontendOrderService.UpdatePrices(order);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, $"Order: {orderId} was successfully updated");
                }
            }
            return Request.CreateErrorResponse(HttpStatusCode.Forbidden,
                "Seems you have no enough privileges. Please contact the local administrator.");
        }
        [HttpGet]
        [ApiAuthorize]
        [Route("api/order/canCloneOrder")]
        public HttpResponseMessage CanCloneOrder(int orderId)
        {
            var user = AuthenticationService.LoggedInUser;
            if (user == null || user.IsAnonymous) return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Not Authorised");

            var order = _frontendOrderService.GetById(orderId);
            if (order == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");

            if (order.UserId != user.Id) return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Not Authorised");

            var photolab = _photolabService.GetById(order.PhotolabId);
            if (!photolab.Properties.IsOrderDuplicationAllowed || order.OldPhotolabId.HasValue)
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "order duplication disabled");

            return Request.CreateResponse(HttpStatusCode.OK, _frontendOrderService.CanBeClonned(photolab, order));
        }
        [HttpPost]
        [ApiAuthorize]
        [Route("api/order/clone")]
        public HttpResponseMessage CloneOrder(int orderId)
        {
            var user = AuthenticationService.LoggedInUser;
            if (user == null || user.IsAnonymous) return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Not Authorised");

            var order = _frontendOrderService.GetById(orderId);
            if (order == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");

            if (order.UserId != user.Id) return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Not Authorised");

            var photolab = _photolabService.GetById(order.PhotolabId);
            if (!photolab.Properties.IsOrderDuplicationAllowed || order.OldPhotolabId.HasValue)
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "order duplication disabled");

            if (!_frontendOrderService.CanBeClonned(photolab, order))
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "some materials not enabled");

            var clonedOrder = _frontendOrderService.CloneOrder(photolab, order);
            return Request.CreateResponse(HttpStatusCode.OK, clonedOrder.Id);
        }
        [HttpPost]
        [ApiAuthorize]
        [Route("api/order/retry")]
        public HttpResponseMessage RetryOrder(int orderId)
        {
            var user = AuthenticationService.LoggedInUser;
            if (user == null || user.IsAnonymous) return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Not Authorisated");

            var order = _frontendOrderService.GetById(orderId);
            if (order == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");

            if (order.UserId != user.Id && order.ManagerId != user.Id) return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Not Authorisated");

            var photolab = _photolabService.GetById(order.PhotolabId);
            if (order.OldPhotolabId.HasValue)
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "order duplication disabled");

            _frontendOrderService.RetrySupplierOrder(photolab, order, user);
            return Request.CreateResponse(HttpStatusCode.OK, orderId);
        }

        [HttpPost]
        [Route("api/order/changeUserCompany")]
        public HttpResponseMessage ChangeUserCompany(int orderId)
        {
            var user = AuthenticationService.LoggedInUser;
            if (user == null || user.IsAnonymous) return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "not logged in");

            var order = _frontendOrderService.GetById(orderId);
            if (order == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");

            if (_frontendOrderService.HasAccessEdit(order, user))
            {
                if (order.UserCompanyAccountId.HasValue)
                {
                    _frontendOrderService.UpdateUserCompany(order);
                    return Request.CreateResponse(HttpStatusCode.OK, orderId);
                }

                var userOwner = _userService.GetById(order.UserId);
                if (userOwner == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "user not found");

                var userOwnerCompany = _userCompanyService.GetByUser(userOwner)?.UserCompany;
                if (userOwnerCompany == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "user company not found");

                _frontendOrderService.UpdateUserCompany(order, userOwnerCompany);
                return Request.CreateResponse(HttpStatusCode.OK, orderId);
            }
            return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "user has no access to edit");
        }

        [HttpGet]
        [ApiAuthorize]
        [Route("api/order/{orderId}/details")]
        public HttpResponseMessage GetOrderDetaislToClone(int orderId)
        {
            var user = AuthenticationService.LoggedInUser;
            if (user == null || user.IsAnonymous) return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "not logged in");

            var order = _frontendOrderService.GetById(orderId);
            if (order == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");
            var photolab = _photolabService.GetById(order.PhotolabId);
            if (photolab == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "photolab not found");

            var userCompany = order.UserCompanyAccountId.HasValue ? _userCompanyService.GetById(order.UserCompanyAccountId.Value) : null;
            var details = _orderCloneService.GetDetailListToClone(order, userCompany, user);

            var data = new { details };
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
        [ApiAuthorize]
        [Route("api/order/{orderId}/clone")]
        public HttpResponseMessage CloneOrderDetails(int orderId, OrderCloneRequest request)
        {
            var user = AuthenticationService.LoggedInUser;
            if (user == null || user.IsAnonymous) return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "not logged in");

            if (request == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "request is null");
            if (request.SelectedOrderDetailIds == null || request.SelectedOrderDetailIds.Count == 0) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "no details chosen");

            var order = _frontendOrderService.GetById(orderId);
            if (order == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "order not found");

            var photolab = _photolabService.GetById(order.PhotolabId);
            if (order == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "photolab not found");

            var userOwner = _userService.GetById(order.UserId);
            if (order == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "user woner owner not found");
            var userCompany = order.UserCompanyAccountId.HasValue ? _userCompanyService.GetById(order.UserCompanyAccountId.Value) : null;

            var isBypassShoppingCart = _orderCloneService.Clone(photolab, order, userCompany, userOwner, request.SelectedOrderDetailIds);

            return Request.CreateResponse(HttpStatusCode.OK, isBypassShoppingCart);
        }
    }
}