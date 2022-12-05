using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Photoprint.Core.Models;
using Photoprint.Core.Services;

namespace Photoprint.WebSite.API.Controllers.Checkout
{
    public class PaymentSelectorDto
    {
        public List<PaymentSelectorItemDto> Payments { get; set; }
        public int MaxPercentToPay { get; set; }
    }
    public class PaymentSelectorItemDto
    {
        public PaymentSelectorItemDto(string id, string title, string description, decimal commission, bool isDefault)
        {
            Id = id;
            Title = title;
            Commission = commission;
            Description = description;
            IsDefault = isDefault;
        }


        public string Id { get; }
        public bool IsDefault { get; }
        public string Title { get; }
        public string Description { get; }
        public decimal Commission { get; }
    }

    public class PaymentSelectorController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly IPhotolabService _photolabService;
        private readonly ILanguageService _languageService;
        private readonly IOrderService _orderService;
        private readonly IUserCompanyService _userCompanyService;
        private readonly IDiscountService _discountService;
        public PaymentSelectorController(IAuthenticationService authenticationService,
                                         IPaymentService paymentService,
                                         IPhotolabService photolabService,
                                         ILanguageService languageService,
                                         IOrderService orderService,
                                         IUserCompanyService userCompanyService,
                                         IDiscountService discountService) : base(authenticationService)
        {
            _paymentService = paymentService;
            _photolabService = photolabService;
            _languageService = languageService;
            _orderService = orderService;
            _userCompanyService = userCompanyService;
            _discountService = discountService;
        }

        [HttpGet]
        [Route("api/payments/getavailable")]
        public HttpResponseMessage GetPayments(string page, int photolabId, int orderId,int languageId)
        {
            var user = AuthenticationService.LoggedInUser;
            if (user == null) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "not logged in");

            var photolab = _photolabService.GetById(photolabId);
            if (photolab == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "photolab not found");

            var order = _orderService.GetById(orderId);
            if (order == null || order.UserId != user.Id) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "no access");
            var language = _languageService.GetById(languageId);
            if (language == null) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "language not found");

            var userCompany = _userCompanyService.GetByUser(user)?.UserCompany;
            var payments = _paymentService.GetAvailableList(photolab, order, userCompany, language).ToList();

            var frontMoney = _orderService.GetFrontMoney(order);
            var result = new PaymentSelectorDto
            {
                Payments = new List<PaymentSelectorItemDto>()
            };

            switch (page)
            {
                case "yandexkassa":
                    var yaPayment = payments.FirstOrDefault(x => x is YandexKassaPayment) as YandexKassaPayment;
                    if (yaPayment != null)
                    {
                        result.Payments.AddRange(yaPayment.AllowedTypes.Select(t =>
                            new PaymentSelectorItemDto(t.ToString(), RM.GetString("Payment.YandexKassa.Type" + t), string.Empty, GetCommission(yaPayment, order, frontMoney), false)));
                        if (yaPayment.AddCashPayButton)
                        {
                            result.Payments.AddRange(GetAdditional(order, frontMoney, userCompany, payments));
                        }
                    }
                    break;
                case "yandexkassaapi":
                    var yaApiPayment = payments.FirstOrDefault(x => x is YandexKassaApiPayment) as YandexKassaApiPayment;
                    if (yaApiPayment != null)
                    {
                        result.Payments.AddRange(yaApiPayment.AllowedTypes.Select(t =>
                            new PaymentSelectorItemDto(t.ToString(), RM.GetString("Payment.YandexKassaApi.Type" + t), string.Empty, GetCommission(yaApiPayment, order, frontMoney), false)));
                        if (yaApiPayment.AddCashPayButton)
                        {
                            result.Payments.AddRange(GetAdditional(order, frontMoney, userCompany, payments));
                        }
                    }
                    break;
                default:

                    //YM-94
                    var discountIds = _discountService.GetDiscountIdsByOrder(order);
                    if (discountIds?.Any() == true && payments != null)
                    {
                        var bindedPayments = _discountService.GetPaymentIdsByDiscounts(discountIds, photolab);
                        if (bindedPayments.Count > 0)
                        {
                            payments = payments.Where(x => bindedPayments.Contains(x.Id)).ToList();
                        }
                    }
                    result.Payments.AddRange(payments
                        .Where(x => x.PaymentSystem.Type == PaymentSystemType.Gateway)
                        .Select(p => new PaymentSelectorItemDto(p.PaymentSystem.UniqueId.ToLowerInvariant(), p.Title, p.Description, GetCommission(p, order, frontMoney), p.Properties.IsDefault)));
                    result.Payments.AddRange(GetAdditional(order, frontMoney, userCompany, payments));
                    break;
            }

            var userPayment = payments.FirstOrDefault(x => x.PaymentSystem.Type == PaymentSystemType.PaymentCard) as PaymentCardPayment;
            if (userPayment != null) result.MaxPercentToPay = userPayment.MaxPercentToPay;

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        private IEnumerable<PaymentSelectorItemDto> GetAdditional(Order order, decimal frontMoney, UserCompany userCompany, List<Core.Models.Payment> payments)
        {
            var cash1 = payments.FirstOrDefault(x => x.PaymentSystem.Type == PaymentSystemType.Cash && x is CashPayment) as CashPayment;
            var cash2 = payments.FirstOrDefault(x => x.PaymentSystem.Type == PaymentSystemType.CashV2 && x is CashPayment) as CashPayment;
            var cash3 = payments.FirstOrDefault(x => x.PaymentSystem.Type == PaymentSystemType.CashV3 && x is CashPayment) as CashPayment;
            var transfer = payments.FirstOrDefault(x => x is InvoicePayment) as InvoicePayment;
            if (cash1 != null && cash1.IsActive)
            {
                var cashTitle = !string.IsNullOrWhiteSpace(cash1.Title)
                    ? cash1.Title
                    : RM.GetString(RS.Payment.YandexKassa.CashPay);

                yield return new PaymentSelectorItemDto(cash1.PaymentSystem.UniqueId.ToLowerInvariant(), cashTitle,
                    cash1.Description, GetCommission(cash1, order, frontMoney), cash1.Properties.IsDefault);
            }
            if (cash2 != null && cash2.IsActive)
            {
                var cashTitle = !string.IsNullOrWhiteSpace(cash2.Title)
                    ? cash2.Title
                    : RM.GetString(RS.Payment.YandexKassa.CashPay);

                yield return new PaymentSelectorItemDto(cash2.PaymentSystem.UniqueId.ToLowerInvariant(), cashTitle,
                    cash2.Description, GetCommission(cash2, order, frontMoney), cash2.Properties.IsDefault);
            }
            if (cash3 != null && cash3.IsActive)
            {
                var cashTitle = !string.IsNullOrWhiteSpace(cash3.Title)
                    ? cash3.Title
                    : RM.GetString(RS.Payment.YandexKassa.CashPay);

                yield return new PaymentSelectorItemDto(cash3.PaymentSystem.UniqueId.ToLowerInvariant(), cashTitle,
                    cash3.Description, GetCommission(cash3, order, frontMoney), cash3.Properties.IsDefault);
            }
            if (transfer != null && transfer.IsActive)
            {
                var transferTitle = !string.IsNullOrWhiteSpace(transfer.Title)
                    ? transfer.Title
                    : RM.GetString(RS.Payment.YandexKassa.BankTransfer);
                var views = _paymentService.GetAvailableBankTransferPaymentViews(transfer, order, null, userCompany);
                if ((!order.IsDefault && transfer.ShowWhenUserAccountRefill) || views.Any(v => v.ShippingIds == null || v.ShippingIds.Length == 0 || v.ShippingIds.Contains(order.ShippingId)))
                {
                    yield return new PaymentSelectorItemDto(transfer.PaymentSystem.UniqueId.ToLowerInvariant(),
                        transferTitle, transfer.Description, GetCommission(transfer, order, frontMoney), transfer.Properties.IsDefault);
                }
            }

        }

        protected decimal GetCommission(Core.Models.Payment payment, Order order, decimal frontMoney)
        {
            if (payment == null) return 0;
            if (!payment.IsCommissionSupported) return 0;

            return payment.Commission.Calculate(order, frontMoney);
        }
    }
}