using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using Elmah;
using Photoprint.Core;
using Photoprint.Core.Configuration;
using Photoprint.Core.Models;
using Photoprint.Core.Models.DDelivery;
using Photoprint.Core.Services;
using Photoprint.WebSite.Shared;
using RazorEngine;
using RazorEngine.Templating;

namespace Photoprint.WebSite.API.Controllers
{
	public class DDeliveryShippingController : BaseApiController
	{
		private readonly IShippingService _shippingService;
		private readonly IShoppingCartService _shoppingCartService;
		private readonly IPhotolabService _photolabService;
		private readonly IDDeliveryProviderService _dDeliveryService;

		private const string ViewTypeForm = "typeForm";
		private const string ViewCityHelper = "cityHelper";
		private const string ViewTypeHelper = "typeHelper";
		private const string ViewCouriers = "couriers";
		private const string ViewMap = "map";
		private const string ViewMapCompanyHelper = "mapCompanyHelper";

		public DDeliveryShippingController(IAuthenticationService authenticationService, IShippingService shippingService,
			IShoppingCartService shoppingCartService, IPhotolabService photolabService, IDDeliveryProviderService dDeliveryService)
			: base(authenticationService)
		{
			_shippingService = shippingService;
			_shoppingCartService = shoppingCartService;
			_photolabService = photolabService;
			_dDeliveryService = dDeliveryService;
			
			Engine.Razor.AddTemplate(ViewTypeForm, new LoadedTemplateSource(System.IO.File.ReadAllText(GetViewPath(ViewTypeForm)), GetViewPath(ViewTypeForm)));
			Engine.Razor.AddTemplate(ViewCityHelper, new LoadedTemplateSource(System.IO.File.ReadAllText(GetViewPath(ViewCityHelper)), GetViewPath(ViewCityHelper)));
			Engine.Razor.AddTemplate(ViewTypeHelper, new LoadedTemplateSource(System.IO.File.ReadAllText(GetViewPath(ViewTypeHelper)), GetViewPath(ViewTypeHelper)));
			Engine.Razor.AddTemplate(ViewCouriers, new LoadedTemplateSource(System.IO.File.ReadAllText(GetViewPath(ViewCouriers)), GetViewPath(ViewCouriers)));
			Engine.Razor.AddTemplate(ViewMap, new LoadedTemplateSource(System.IO.File.ReadAllText(GetViewPath(ViewMap)), GetViewPath(ViewMap)));
			Engine.Razor.AddTemplate(ViewMapCompanyHelper, new LoadedTemplateSource(System.IO.File.ReadAllText(GetViewPath(ViewMapCompanyHelper)), GetViewPath(ViewMapCompanyHelper)));
		}
		
		private string GetViewPath(string name)
		{
			return string.Format("{0}Views\\Checkout\\Controls\\Shipping\\PixlparkShippings\\DDelivery\\Views\\{1}._cshtml", Settings.SaasProjectRootPath, name);
		}

		[HttpPost]
		[Route("api/shippings/ddelivery/init")]
		public HttpResponseMessage Initialize(FormDataCollection form)
		{
			try
			{
				var request = new DDeliveryInitRequest(form);
				var shipping = _shippingService.GetById<Postal>(request.ShippingId);
				var settings = shipping?.ServiceProviderSettings as DDeliveryServiceProviderSettings;
				if (settings != null)
				{
					if ((settings.SupportedShippingTypes == null || settings.SupportedShippingTypes.Count > 1) && request.Type == null)
					{
						return TypeForm(form);
					}
					if (request.Type == DDeliveryShippingType.Courier)
					{
						return Courier(form);
					}
					if (request.Type == DDeliveryShippingType.Point)
					{
						return Map(form);
					}
				}
			}
			catch (Exception ex)
			{
				ErrorLog.GetDefault(HttpContext.Current).Log(new Error(ex, HttpContext.Current));
			}
			return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "params error");
		}

		[HttpPost]
		[Route("api/shippings/ddelivery/courier")]
		public HttpResponseMessage Courier(FormDataCollection form)
		{
			try
			{
				var request = new DDeliveryCourierRequest(form);
				var shipping = _shippingService.GetById<Postal>(request.ShippingId);
				var frontend = _photolabService.GetById(shipping.PhotolabId);
				var cart = _shoppingCartService.GetCart(frontend);
				var settings = shipping.ServiceProviderSettings as DDeliveryServiceProviderSettings;
				if (settings != null)
				{
					var city = _dDeliveryService.GetCurrentCity(settings, request.CityId, SiteUtils.GetIpAddress(Request));
					var info = _dDeliveryService.GetShippingTypeData(frontend, AuthenticationService.LoggedInUser, settings, cart.Items, city, shipping);
					var companies = _dDeliveryService.GetCourierPrices(frontend, AuthenticationService.LoggedInUser, settings, cart.Items, city, shipping);

					var model = new DDeliveryCourierFormModel(settings, info, companies);
					model.CityInfo.SelectedCity = city;
					model.CityInfo.Cities = _dDeliveryService.GetCityListForDisplay(city);
					
					var html = Engine.Razor.RunCompile(ViewCouriers, typeof(DDeliveryCourierFormModel), model);

				
					var result = new
					{
						html = html,
						js = "courier",
						type = "2", // TYPE_COURIER
						orderId = 0,
						typeData = new
						{
							self = model.ShippingInfo.Point,
							courier = model.ShippingInfo.Courier
						}
					};
					return Request.CreateResponse(HttpStatusCode.OK, result);
				}
			}
			catch (Exception ex)
			{
				ErrorLog.GetDefault(HttpContext.Current).Log(new Error(ex, HttpContext.Current));
			}
			return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "params error");
		}

		[HttpPost]
		[Route("api/shippings/ddelivery/map")]
		public HttpResponseMessage Map(FormDataCollection form)
		{
			try
			{
				var request = new DDeliveryCourierRequest(form);
				var shipping = _shippingService.GetById<Postal>(request.ShippingId);
				var frontend = _photolabService.GetById(shipping.PhotolabId);
				var cart = _shoppingCartService.GetCart(frontend);
				var settings = shipping.ServiceProviderSettings as DDeliveryServiceProviderSettings;
				if (settings != null)
				{
					var city = _dDeliveryService.GetCurrentCity(settings, request.CityId, SiteUtils.GetIpAddress(Request));
					var info = _dDeliveryService.GetShippingTypeData(frontend, AuthenticationService.LoggedInUser, settings, cart.Items, city, shipping);

					var companies = _dDeliveryService.GetPointPrices(frontend, AuthenticationService.LoggedInUser, settings, cart.Items, city, shipping).AsList();
					var points = _dDeliveryService.GetPoints(city, companies);

					var model = new DDeliveryMapFormModel(settings, info, companies);
					model.CityInfo.SelectedCity = city;
					model.CityInfo.Cities = _dDeliveryService.GetCityListForDisplay(city);

					var html = Engine.Razor.RunCompile(ViewMap, typeof(DDeliveryMapFormModel), model);

					//return json_encode(array('html'=>$content, 'js'=>'map', 'points' => $pointsJs, 'orderId' => $this->order->localId, 'type'=>DDeliverySDK::TYPE_SELF));
					var result = new
					{
						html = html,
						js = "map",
						type = "1", // TYPE_SELF
						orderId = 0,
						points = points,
						typeData = new
						{
							self = model.ShippingInfo.Point,
							courier = model.ShippingInfo.Courier
						}
					};
					return Request.CreateResponse(HttpStatusCode.OK, result);
				}
			}
			catch (Exception ex)
			{
				ErrorLog.GetDefault(HttpContext.Current).Log(new Error(ex, HttpContext.Current));
			}
			return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "params error");
		}

		[HttpPost]
		[Route("api/shippings/ddelivery/mapDataOnly")]
		public HttpResponseMessage MapDataOnly(FormDataCollection form)
		{
			try
			{
				var request = new DDeliveryCourierRequest(form);
				var shipping = _shippingService.GetById<Postal>(request.ShippingId);
				var frontend = _photolabService.GetById(shipping.PhotolabId);
				var cart = _shoppingCartService.GetCart(frontend);
				var settings = shipping.ServiceProviderSettings as DDeliveryServiceProviderSettings;
				if (settings != null)
				{
					var city = _dDeliveryService.GetCurrentCity(settings, request.CityId, SiteUtils.GetIpAddress(Request));
					var info = _dDeliveryService.GetShippingTypeData(frontend, AuthenticationService.LoggedInUser, settings, cart.Items, city, shipping);

					var companies = _dDeliveryService.GetPointPrices(frontend, AuthenticationService.LoggedInUser, settings, cart.Items, city, shipping).AsList();
					var points = _dDeliveryService.GetPoints(city, companies);

					var model = new DDeliveryMapFormModel(settings, info, companies);
					model.CityInfo.SelectedCity = city;
					model.CityInfo.Cities = _dDeliveryService.GetCityListForDisplay(city);

					var html = Engine.Razor.RunCompile(ViewMapCompanyHelper, typeof(IEnumerable<DDeliveryCalculatorResult>), model.Companies);

					//return json_encode(array('html'=>$content, 'points' => $pointsJs, 'orderId' => $this->order->localId, 'headerData' => $dataFromHeader));
					var result = new
					{
						html = html,
						type = "1", // TYPE_SELF
						orderId = 0,
						points = points,
						headerData = new
						{
							self = model.ShippingInfo.Point,
							courier = model.ShippingInfo.Courier
						}
					};
					return Request.CreateResponse(HttpStatusCode.OK, result);
				}
			}
			catch (Exception ex)
			{
				ErrorLog.GetDefault(HttpContext.Current).Log(new Error(ex, HttpContext.Current));
			}
			return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "params error");
		}

		[HttpPost]
		[Route("api/shippings/ddelivery/mapGetPoint")]
		public HttpResponseMessage GetPoint(FormDataCollection form)
		{
			try
			{
				var request = new DDeliveryGetPointRequest(form);
				var shipping = _shippingService.GetById<Postal>(request.ShippingId);
				var frontend = _photolabService.GetById(shipping.PhotolabId);
				var cart = _shoppingCartService.GetCart(frontend);
				var settings = shipping.ServiceProviderSettings as DDeliveryServiceProviderSettings;
				if (settings != null)
				{
					var city = _dDeliveryService.GetCurrentCity(settings, request.CityId, SiteUtils.GetIpAddress(Request));
					var companies = _dDeliveryService.GetPointPrices(frontend, AuthenticationService.LoggedInUser, settings, cart.Items, city, shipping).AsList();
					var points = _dDeliveryService.GetPoints(city, companies);

					var point = points.FirstOrDefault(p => p.Id == request.PointId);
					if (point != null)
					{
						var price = _dDeliveryService.GetPointPrice(frontend, AuthenticationService.LoggedInUser, settings, cart.Items, point, shipping);
						if (price != null)
						{
							var result = new
							{
								point,
								company = price
							};
							return Request.CreateResponse(HttpStatusCode.OK, result);
						}
					}
				}
			}
			catch (Exception ex)
			{
				ErrorLog.GetDefault(HttpContext.Current).Log(new Error(ex, HttpContext.Current));
			}
			return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "params error");
		}
        
		[HttpPost]
		[Route("api/shippings/ddelivery/searchCity")]
		public HttpResponseMessage SearchCity(FormDataCollection form)
		{
			try
			{
				var query = form["name"];
				if (!string.IsNullOrWhiteSpace(query) && query.Length >= 2)
				{
					var cities = _dDeliveryService.GetCityBySearch(form["name"]);
					var model = new DDeliveryCityInfo();
					model.Cities = cities;

					var html = Engine.Razor.RunCompile(ViewCityHelper, typeof(DDeliveryCityInfo), model);
					var result = new
					{
						html,
						displayData = new List<object>(),
						request = new
						{
							name = query,
							action = "searchCity"
						}
					};
					return Request.CreateResponse(HttpStatusCode.OK, result);
				}
			}
			catch (Exception ex)
			{
				ErrorLog.GetDefault(HttpContext.Current).Log(new Error(ex, HttpContext.Current));
				return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
			}
			return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "params error");
		}
        
		[HttpPost]
		[Route("api/shippings/ddelivery/typeForm")]
		public HttpResponseMessage TypeForm(FormDataCollection form)
		{
			try
			{
				var request = new DDeliveryTypeFormRequest(form);
				var shipping = _shippingService.GetById<Postal>(request.ShippingId);
				var frontend = _photolabService.GetById(shipping.PhotolabId);
				var cart = _shoppingCartService.GetCart(frontend);
				var settings = shipping.ServiceProviderSettings as DDeliveryServiceProviderSettings;
				if (settings != null)
				{
					var city = _dDeliveryService.GetCurrentCity(settings, request.CityId, SiteUtils.GetIpAddress(Request));
					var info = _dDeliveryService.GetShippingTypeData(frontend, AuthenticationService.LoggedInUser, settings, cart.Items, city, shipping);

					var model = new DDeliveryTypeFormModel(settings, info);
					model.CityInfo.SelectedCity = city;
					model.CityInfo.Cities = _dDeliveryService.GetCityListForDisplay(city);


					var html = Engine.Razor.RunCompile(ViewTypeForm, typeof(DDeliveryTypeFormModel), model);

					var result = new
					{
						html = html,
						js = ViewTypeForm,
						orderId = 0,
						typeData = new
						{
							self = model.ShippingInfo.Point,
							courier = model.ShippingInfo.Courier
						}
					};
					return Request.CreateResponse(HttpStatusCode.OK, result);
				}
			}
			catch (Exception ex)
			{
				return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
			}
			return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "params error");
		}
	}
    
	public class DDeliveryCityInfo
	{
		public IEnumerable<DDeliveryCity> Cities { get; set; }
		public DDeliveryCity SelectedCity { get; set; }
	}

	public class DDeliveryBaseModel
	{
		public DDeliveryServiceProviderSettings Settings { get; set; }
		public DDeliveryShippingInfo ShippingInfo { get; private set; }
		public DDeliveryCityInfo CityInfo { get; private set; }
		
		public DDeliveryBaseModel(DDeliveryServiceProviderSettings settings, DDeliveryShippingInfo shippingInfo)
		{
			Settings = settings;
			CityInfo = new DDeliveryCityInfo();
			ShippingInfo = shippingInfo;
		}
	}

	public class DDeliveryTypeFormModel : DDeliveryBaseModel
	{
		public DDeliveryTypeFormModel(DDeliveryServiceProviderSettings settings, DDeliveryShippingInfo shippingInfo)
			: base(settings, shippingInfo)
		{
			
		}
	}

	public class DDeliveryMapFormModel : DDeliveryBaseModel
	{
		public IList<DDeliveryCalculatorResult> Companies { get; private set; }
		public DDeliveryMapFormModel(DDeliveryServiceProviderSettings settings, DDeliveryShippingInfo shippingInfo, IEnumerable<DDeliveryCalculatorResult> companies)
			: base(settings, shippingInfo)
		{
			Companies = companies.ToList();
		}
	}

	public class DDeliveryCourierFormModel : DDeliveryBaseModel
	{
		public IList<DDeliveryCalculatorResult> Companies { get; private set; }
		
		public DDeliveryCourierFormModel(DDeliveryServiceProviderSettings settings, DDeliveryShippingInfo shippingInfo, IEnumerable<DDeliveryCalculatorResult> companies)
			: base(settings, shippingInfo)
		{
			Companies = companies.ToList();
		}
	}

	public abstract class DDeliveryBaseRequest
	{
		public abstract string Action { get; }
		public int ShippingId { get; private set; }
		public DDeliveryShippingType? Type { get; set; }
		public int? CityId { get; private set; }

		protected DDeliveryBaseRequest(FormDataCollection form)
		{
			ShippingId = int.Parse(form["shippingId"]);
			if (!string.IsNullOrWhiteSpace(form["city_id"]))
				CityId = int.Parse(form["city_id"]);

			if (!string.IsNullOrWhiteSpace(form["type"]))
			{
				Type = (DDeliveryShippingType) int.Parse(form["type"]);
			}
		}
	}
	
	public sealed class DDeliveryTypeFormRequest : DDeliveryBaseRequest
	{
		public DDeliveryTypeFormRequest(FormDataCollection form) : base(form)
		{
		}

		public override string Action
		{
			get { return "typeForm"; }
		}
	}

	public sealed class DDeliveryMapRequest : DDeliveryBaseRequest
	{
		public override string Action
		{
			get { return "map"; }
		}

		public DDeliveryMapRequest(FormDataCollection form)
			: base(form)
		{
		}
	}

	public sealed class DDeliveryCourierRequest : DDeliveryBaseRequest
	{
		public override string Action
		{
			get { return "courier"; }
		}

		public DDeliveryCourierRequest(FormDataCollection form)
			: base(form)
		{
		}
	}

	public sealed class DDeliveryInitRequest : DDeliveryBaseRequest
	{
		public override string Action
		{
			get { return "init"; }
		}

		public DDeliveryInitRequest(FormDataCollection form) : base(form)
		{
		}
	}
	
	public sealed class DDeliveryGetPointRequest : DDeliveryBaseRequest
	{
		public override string Action
		{
			get { return "mapGetPoint"; }
		}

		public int PointId { get; private set; }

		public DDeliveryGetPointRequest(FormDataCollection form)
			: base(form)
		{
			int id;
			if (!string.IsNullOrWhiteSpace(form["id"]) && int.TryParse(form["id"], out id))
			{
				PointId = id;
			}
		}
	}
}
