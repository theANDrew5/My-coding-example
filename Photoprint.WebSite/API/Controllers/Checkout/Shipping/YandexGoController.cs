using NLog;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Photoprint.WebSite.API
{
    public class YandexGoController : BaseApiController
    {
        private readonly IPhotolabService _photolabService;
        private readonly IFrontendShippingService _frontendShippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUserService _userService;
        private readonly IFrontendDiscountService _frontendDiscountService;
        private readonly IYandexGoService _yandexGoService;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public YandexGoController(IAuthenticationService authenticationService,
                                  IFrontendShippingService frontendShippingService,
                                  IPhotolabService photolabService,
                                  IShoppingCartService shoppingCartService,
                                  IUserService userService,
                                  IShippingProviderResolverService providerResolver,
                                  IFrontendDiscountService frontendDiscountService) : base(authenticationService)
        {
            _frontendShippingService = frontendShippingService;
            _photolabService = photolabService;
            _shoppingCartService = shoppingCartService;
            _userService = userService;
            _frontendDiscountService = frontendDiscountService;
            _yandexGoService = providerResolver.GetProvider(ShippingServiceProviderType.YandexGo) as IYandexGoService;
        }

        [HttpPost]
        [Route("api/shippings/yandexGo/checkPrice")]
        public HttpResponseMessage CheckPrice(YandexGoAddressDto dto)
        {
            var photolab = _photolabService.GetById(dto.PhotolabId);
            if (photolab == null) return Request.CreateResponse(HttpStatusCode.BadRequest);
            var user = _userService.GetById(dto.UserId);
            if (user == null) return Request.CreateResponse(HttpStatusCode.BadRequest);
            var cart = _shoppingCartService.GetCart(user, photolab);
            if (cart == null) return Request.CreateResponse(HttpStatusCode.BadRequest);

            var appliedDiscountIds = cart.Items.SelectMany(x => x.AppliedDiscounts.Select(y => y.DiscountId)).Distinct().ToArray();
            var appliedDiscount = _frontendDiscountService.GetListByIds(appliedDiscountIds);
            var address = new OrderAddress { Country = dto.Country, Region = dto.Region, City = dto.City, Latitude = dto.Latitude, Longitude = dto.Longitude };
            var postal = _frontendShippingService.GetById<Postal>(dto.ShippingId);
            var result = _yandexGoService.EstimateOrder(postal, address, cart.Items);

            if (result== null) return Request.CreateResponse(HttpStatusCode.BadRequest);

            if (appliedDiscount != null)
            {
                var filteredDiscounts = _frontendDiscountService.FilterDiscountsByShipping(photolab, postal, appliedDiscount);
                foreach (var discount in filteredDiscounts.Where(d => d.ValueForShipping.HasValue))
                {
                    result.Price = discount.ValueForShipping.Apply(result.Price, cart.Items.Sum(i => i.Quantity), cart.Items.Sum(x => x.ItemPartsQuantity));
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                result.Price,
                result.DistanceMeters
            });
            
        }

        public class YandexGoAddressDto
        {
            public int PhotolabId { get; set; }
            public int ShippingId { get; set; }
            public int UserId { get; set; }
            public string Fullname { get; set; }
            public string Country { get; set; }
            public string Region { get; set; }
            public string City { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
        }

    }
}