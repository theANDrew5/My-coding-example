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
using Photoprint.WebSite.API.Models;

namespace Photoprint.WebSite.API.Controllers
{
    public class PriceForDiscountDto
    {
        public int PhotolabId { get; set; }
        public int ShippingId { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public int UserId { get; set; }
    }

	public class ShippingsController : BaseApiController
	{
		private readonly IFrontendShippingService _frontendShippingService;
	    private readonly IFrontendShippingPriceService _frontendShippingPriceService;
        private readonly IShippingCalculatorService _calculatorService;
		private readonly IMaterialService _materialService;
		private readonly ICustomWorkService _customWorkService;
		private readonly IPhotolabService _photolabService;
		private readonly IMaterialTypeService _materialTypeService;
        private readonly IFrontendDiscountService _frontendDiscountService;
	    private readonly IShoppingCartService _shoppingCartService;
	    private readonly IUserService _userService;

        public ShippingsController(IAuthenticationService authenticationService,
								   IFrontendShippingService frontendShippingService,
								   IShippingCalculatorService calculatorService,
								   IMaterialService materialService,
								   ICustomWorkService customWorkService,
								   IPhotolabService photolabService,
								   IMaterialTypeService materialTypeService,
								   IFrontendDiscountService frontendDiscountService,
								   IFrontendShippingPriceService frontendShippingPriceService,
								   IShoppingCartService shoppingCartService,
								   IUserService userService)
			: base(authenticationService)
		{
			_frontendShippingService = frontendShippingService;
			_materialService = materialService;
			_customWorkService = customWorkService;
			_photolabService = photolabService;
			_materialTypeService = materialTypeService;
            _frontendDiscountService = frontendDiscountService;
            _frontendShippingPriceService = frontendShippingPriceService;
            _calculatorService = calculatorService;
		    _shoppingCartService = shoppingCartService;
		    _userService = userService;
        }

        [HttpGet]
		[Route("api/shippings/byMaterial")]
		public HttpResponseMessage GetByMaterial(int materialId, int customWorkItemId)
		{
			try
			{
				var material = _materialService.GetById(materialId);
				var lab = material != null ? _photolabService.GetById(material.PhotolabId) : null;

				if (lab != null && customWorkItemId > 0)
				{
					var customWorkItem = _customWorkService.GetItemById(customWorkItemId);

					var items = new List<ShoppingCartItem>();
					var item = GetItemByMaterialRequest(material, customWorkItem);
					items.Add(item);
					var availableShippings = _frontendShippingService.GetAvailableList<Shipping>(lab, items);

					var results = new List<ShippingInfoDto>();
					foreach (var shipping in availableShippings)
					{
						var dto = FilterShipping(shipping, null, null, null, 1);
						results.Add(dto);
					}

					return Request.CreateResponse(HttpStatusCode.OK, results);
				}
			}
			catch (Exception ex)
			{
				Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(ex, HttpContext.Current));
			}
			return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "params error");
		}

		private ShoppingCartItem GetItemByMaterialRequest(Material material, CustomWorkItem workItem)
		{
			var circulation = material.CirculationSettings.GetCirculationByQuantity(1);
			if (circulation == null) circulation = new Circulation(1, material.Price);
			circulation.ItemPartsCount = 1;

			var type = _materialTypeService.GetById(material.MaterialTypeId);
			var item = new MaterialShoppingCartItem(type, material, circulation, null);
			item.Properties.AdditionalPriceItems.AddRange( new List<AdditionalPriceItem>(1) { new AdditionalPriceItem(workItem.Id, null, null, 0, 0, null, PriceFormatType.Once, 1) });

			return item;
		}

	    [HttpPost]
	    [Route("api/shippings/getcalculatedprice")]
	    public HttpResponseMessage GetCalculatedPrice(PriceForDiscountDto priceInfo)
	    {
	        var lab = _photolabService.GetById(priceInfo.PhotolabId);
	        var shipping = _frontendShippingService.GetById<Postal>(priceInfo.ShippingId);
	        var shippingAddress = _frontendShippingService.GetSuitableShipingAddresses(shipping, priceInfo.Country, priceInfo.Region, priceInfo.City, priceInfo.Address);
	        var user = _userService.GetById(priceInfo.UserId);
	        var items = _shoppingCartService.GetCart(user, lab).Items;

            var filteredDiscounts = _frontendDiscountService.FindDiscountsForOrder(lab, user, items, shipping).ToArray();

            var result = _calculatorService.GetShippingPrice(shipping, new OrderAddress(shippingAddress), items,
                filteredDiscounts);

            return Request.CreateResponse(HttpStatusCode.OK, new {success = result.Success, price = result.Price, properties = result.Properties.ToJSON()});
	    }

        [HttpGet]
		[Route("api/shippings")]
		public IEnumerable<ShippingInfoDto> Get(int? shippingId = null, int? addressId = null, string city = null, string type = null, double? weight = null)
		{
			var results = new List<ShippingInfoDto>();
			try
			{
				var currentWeight = weight ?? 0;
				if (shippingId.HasValue)
				{
					var shipping = _frontendShippingService.GetById<Shipping>(shippingId.Value);
					var res = FilterShipping(shipping, addressId, type, city, currentWeight);
					if (res != null)
						results.Add(res);
				}
				else
				{
					var shippings = _frontendShippingService.GetList<Shipping>(WebSiteGlobal.CurrentPhotolab);
					results.AddRange(shippings.Select(it => FilterShipping(it, addressId, type, city, currentWeight)).Where(result => result != null));
				}
			}
			catch (Exception ex)
			{
				Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(ex, HttpContext.Current));
			}
			return results;
		}

		private ShippingInfoDto FilterShipping(Shipping shipping, int? addressId, string type, string city, double weight)
		{
			if (shipping == null || !shipping.IsEnabled)
				return null;
			if (!string.IsNullOrEmpty(type) && !shipping.Type.ToString().Equals(type, StringComparison.InvariantCultureIgnoreCase))
				return null;
			var addresses = new List<ShippingAddressDto>();
		    var photolab = _photolabService.GetById(shipping.PhotolabId);

		    switch (shipping)
		    {
                case Courier courier:
                {
                    foreach (var address in courier.ShippingAddresses)
                    {
                        if (!addressId.HasValue || address.Id == addressId)
                        {
                            var adr = GetShippingAddressDto(city, address.Id, address);
                            if (adr != null)
                            {
                                if (!_frontendShippingPriceService.TryCalculatePriceForShippingAddress(photolab, address,
                                        new List<IShippable> { new OrderDetail { ItemWeight = weight, Quantity = 1 } },
                                        out var price))
                                    throw new PhotoprintSystemException(DeliveryExceptionMessages.PriceCalculationError,
                                        string.Empty);
                                adr.Price = price;
                                addresses.Add(adr);
                            }
                        }
                    }
                    break;
                }
                case DistributionPoint point:
                {
                    if (!addressId.HasValue || addressId.Value == point.Address.Id)
                    {
                        var adr = GetShippingAddressDto(city, point.Address.Id, point.Address);
                        if (adr != null)
                        {
                            if (!_frontendShippingPriceService.TryCalculatePriceForShipping(point,
                                    new List<IShippable> { new OrderDetail { ItemWeight = weight, Quantity = 1 } },
                                    out var price))
                                throw new PhotoprintSystemException(DeliveryExceptionMessages.PriceCalculationError,
                                    string.Empty);
                            adr.Price = price;
                            addresses.Add(adr);
                        }
                    }

                    break;
                }
                case Postal postal:
                {
                    foreach (var address in postal.ShippingAddresses)
                    {
                        if (!addressId.HasValue || address.Id == addressId)
                        {
                            var adr = GetShippingAddressDto(city, address.Id, address);
                            if (adr != null)
                            {
                                if (!_frontendShippingPriceService.TryCalculatePriceForShippingAddress(photolab, address,
                                        new List<IShippable> { new OrderDetail { ItemWeight = weight, Quantity = 1 } },
                                        out var price))
                                    throw new PhotoprintSystemException(DeliveryExceptionMessages.PriceCalculationError,
                                        string.Empty);
                                adr.Price = price;
                                addresses.Add(adr);
                            }
                        }
                    }

                    break;
                }
		    }
			return addresses.Any() ? new ShippingInfoDto(shipping, photolab.DefaultLanguage, addresses) : null;
		}

		private static ShippingAddressDto GetShippingAddressDto(string city, int id, ShippingAddress address)
		{
			ShippingAddressDto adr = null;
			if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(address.City) || address.City.Equals(city, StringComparison.InvariantCultureIgnoreCase)) 
			{ 
				adr = new ShippingAddressDto(address);
			}

			if (adr != null)
			{
				adr.Id = id;
			}

			return adr;
		}
	}
}
