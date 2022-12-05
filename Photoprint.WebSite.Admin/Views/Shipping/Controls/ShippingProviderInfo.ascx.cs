using System;
using System.Collections.Generic;
using System.Linq;
using Photoprint.Core;
using Photoprint.Core.Models;
using Photoprint.Core.Services;

namespace Photoprint.WebSite.Admin.Controls
{
    public partial class ShippingProviderInfo : BaseControl
    {
        protected static readonly IShippingService ShippingService = WebSiteGlobal.Container.GetInstance<IShippingService>();
        protected static readonly IOrderService OrderService = WebSiteGlobal.Container.GetInstance<IOrderService>();
        protected static readonly IShippingProviderResolverService ShippingProviderResolver = WebSiteGlobal.Container.GetInstance<IShippingProviderResolverService>();
        protected static readonly ICompanyTeamService CompanyTeamService = WebSiteGlobal.Container.GetInstance<ICompanyTeamService>();
        protected static readonly IUserService UserService = WebSiteGlobal.Container.GetInstance<IUserService>();

        public Order CurrentOrder { get; set; }
        public Photolab OrderPhotolab { get; set; }

        private IReadOnlyCollection<DistributionPoint> _dpoints;
        protected IReadOnlyCollection<DistributionPoint> DPoints
        {
            get
            {
                if (_dpoints != null) return _dpoints;

                var dpointsBindings = ShippingService.GetShippingBindings(CurrentCompanyAccount, LoggedInUser)
                    .Bindings?.FirstOrDefault(frontendBindings => frontendBindings.Key == OrderPhotolab.Id)
                    .Value?.ShippingIds ?? Array.Empty<int>();

                _dpoints = ShippingService.GetList<DistributionPoint>(OrderPhotolab)
                    .OrderBy(x => !dpointsBindings.Contains(x.Id))
                    .AsList();

                return _dpoints;
            }
        }

        protected IReadOnlyCollection<User> Crew { get; private set; }
        protected bool IsEnabledCdekTariff(string code) { return code == "136" || code == "137" || code == "138" || code == "139"; }

        protected Postal GetOrderPostal()
		{
			if (CurrentOrder == null)
				return null;

			var postal = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);

            return postal;
		}

		protected ShippingProviderInfo()
		{
			Load += ShippingProviderInfoLoad;
		}

		private void ShippingProviderInfoLoad(object sender, EventArgs e)
		{
		    var postal = GetOrderPostal();
		    if (postal != null)
		    {
		        if (postal.ServiceProviderSettings is CdekServiceProviderSettings)
		        {
		            var team = CompanyTeamService.GetTeamByCompanyAccount(CurrentCompanyAccount);
		            var users = UserService.GetListByIds(team.Select(t => t.UserId).ToList());
		            var crew = new List<User>();
		            foreach (var user in users)
		            {
		                if (user == null ) continue;
		                if (LoggedInUser.IsCompanyAdministrator(CurrentCompanyAccount) || LoggedInUser.IsAdministrator || !user.IsCompanyAdministrator(CurrentCompanyAccount))
		                {
		                    crew.Add(user);
		                }
		            }
		            Crew = crew;
		        }                
		    }
		    

            prExgarantRegisterController.CurrentOrder = CurrentOrder;
		    prExgarantRegisterController.OrderFrontend = OrderPhotolab;
		}
        
        protected void CdekRegisterOrderClick(object sender, EventArgs e)
        {
            var cdekPoperties = CurrentOrder.DeliveryAddress.DeliveryProperties.CdekAddressInfo;
            if (int.TryParse(Request.Form["txtCdekTariff"], out var singleTariff))
            {
                cdekPoperties.Tariff = singleTariff;
            }
			else
            {
                if (!int.TryParse(Request.Form["selCdekTariff"], out var selectedTariff))
					return;
                if (selectedTariff != cdekPoperties.Tariff)
                    cdekPoperties.Tariff = selectedTariff;
            }

            Int32.TryParse(Request.Form["cdekRegisterType"], out var type);
			var cdekService = ShippingProviderResolver.GetProvider(GetOrderPostal());
			if (cdekService != null)
			{
				var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
				try
				{
					cdekService.GetCreateOrderRegistration(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings,null, (ShippingRegisterType)type);
                    litErrorMessage.Visible = false;
					Response.Redirect(Request.Url.AbsolutePath + "#shipping");
				}
				catch (Exception ex)
				{
				    ShowError(ex.Message);
				}
			}
		}
        protected void OmnivaRegisterClick(object sender, EventArgs e)
        {
            var omnivaService = ShippingProviderResolver.GetProvider(GetOrderPostal());
            if(omnivaService != null)
            {
                omnivaService.GetCreateOrderRegistration(OrderPhotolab, CurrentOrder, CurrentOrder.Shipping.ServiceProviderSettings);
            }
        }
		protected void CdekDeleteOrderClick(object sender, EventArgs e)
		{
			var cdekService = ShippingProviderResolver.GetProvider(GetOrderPostal());
			if (cdekService != null)
			{
				var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
				cdekService.GetDeleteOrderRegistration(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);
			}
			Response.Redirect(Request.Url.AbsolutePath + "#shipping");
		}
        
        protected void NovaposhtaDeleteOrderClick(object sender, EventArgs e)
		{
			try
			{
				var novaService = ShippingProviderResolver.GetProvider(GetOrderPostal());
				if (novaService != null)
				{
					var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
					novaService.GetDeleteOrderRegistration(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);
				}
				Response.Redirect(Request.Url.AbsolutePath + "#shipping");
			}
			catch (Exception ex)
			{
			    ShowError(ex.Message);
			}
			
		}
        protected void NovaposhtaDeleteOrderClick2(object sender, EventArgs e)
		{
			try
			{
				var novaService = ShippingProviderResolver.GetProvider(GetOrderPostal());
				if (novaService != null)
				{
					var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
					var settings = new NovaposhtaServiceProviderSettings((NovaposhtaServiceProviderSettings)shipping.ServiceProviderSettings, true);
					novaService.GetDeleteOrderRegistration(OrderPhotolab, CurrentOrder, settings);
				}
				Response.Redirect(Request.Url.AbsolutePath + "#shipping");
			}
			catch (Exception ex)
			{
			    ShowError(ex.Message);
            }
		}
        protected void GetNovaposhtaDocs(object sender, EventArgs e)
        {
            try
            {
                var novaService = ShippingProviderResolver.GetProvider(GetOrderPostal());
                var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
                var settings = new NovaposhtaServiceProviderSettings((NovaposhtaServiceProviderSettings)shipping.ServiceProviderSettings);

                novaService.GetCreateOrderRegistration(OrderPhotolab, CurrentOrder, settings);
                plhdSuccessMessage.Visible = true;
                litSuccessMessage.Text = RS.Order.Info.NovaposhtaSuccess;
                plhdErrorMessage.Visible = false;

                if (settings.ChangeOrderStatusToShippedAfterAutomaticRegistration)
                {
                    var availableStatus = OrderService.GetAvailableStatuses(CurrentOrder, LoggedInUser);
                    if (availableStatus.Contains(OrderStatus.Shipped))
                    {
                        OrderService.SetStatus(CurrentOrder, OrderStatus.Shipped,
                            "Автоматически, т.к. заказ зарегистрирован в Новапочте вручную.");
                    }
                }

                Response.Redirect(Request.Url.AbsolutePath + "#shipping");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        protected void GetNovaposhtaDocs2(object sender, EventArgs e)
        {
            try
            {
                var novaService = ShippingProviderResolver.GetProvider(GetOrderPostal());
                var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
                var settings = new NovaposhtaServiceProviderSettings((NovaposhtaServiceProviderSettings)shipping.ServiceProviderSettings, true);
                novaService.GetCreateOrderRegistration(OrderPhotolab, CurrentOrder, settings);
                plhdSuccessMessage.Visible = true;
                litSuccessMessage.Text = RS.Order.Info.NovaposhtaSuccess;
                plhdErrorMessage.Visible = false;

                if (settings.ChangeOrderStatusToShippedAfterAutomaticRegistration)
                {
                    var availableStatus = OrderService.GetAvailableStatuses(CurrentOrder, LoggedInUser);
                    if (availableStatus.Contains(OrderStatus.Shipped))
                    {
                        OrderService.SetStatus(CurrentOrder, OrderStatus.Shipped,
                            "Автоматически, т.к. заказ зарегистрирован в Новапочте вручную.");
                    }
                }

                Response.Redirect(Request.Url.AbsolutePath + "#shipping");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        protected void GetImLogisticsDocs(object sender, EventArgs e)
		{
			var imService = ShippingProviderResolver.GetProvider(GetOrderPostal());
			if (imService != null)
			{
				var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
				try
				{
					imService.GetCreateOrderRegistration(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);
                    plhdErrorMessage.Visible = false;
					Response.Redirect(Request.Url.AbsolutePath + "#shipping");
				}
				catch (Exception ex)
				{
				    ShowError(ex.Message);
                }
			}
		}
        protected void ImLogisticsDeleteOrderClick(object sender, EventArgs e)
		{
			try
			{
				var imService = ShippingProviderResolver.GetProvider(GetOrderPostal());
				if (imService != null)
				{
					var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
					imService.GetDeleteOrderRegistration(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);
				}
				Response.Redirect(Request.Url.AbsolutePath + "#shipping");
			}
			catch (Exception ex)
			{
			    ShowError(ex.Message);
            }
		}
        protected void ImLogisticsUpdateClick(object sender, EventArgs e)
		{
			try
			{
				var imService = ShippingProviderResolver.GetProvider(GetOrderPostal());
				if (imService != null)
				{
					var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
					imService.GetOrderStatus(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);
				}
				Response.Redirect(Request.Url.AbsolutePath + "#shipping");
			}
			catch (Exception ex)
			{
			    ShowError(ex.Message);
            }
		}

		protected void DDeliveryRegisterOrderClick(object sender, EventArgs e)
		{
			var ddeliveryService = ShippingProviderResolver.GetProvider(GetOrderPostal());
			var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
			var settings = (DDeliveryServiceProviderSettings)shipping.ServiceProviderSettings;

			try
			{
				ddeliveryService.GetCreateOrderRegistration(OrderPhotolab, CurrentOrder, settings);
			}
			catch (Exception ex)
			{
			    ShowError(ex.Message);
            }
			
		}

	    protected void GetNovaposhtaV2Docs(object sender, EventArgs e)
	    {
	        try
            {
                var novaService = ShippingProviderResolver.GetProvider(GetOrderPostal());
                var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
                var settings = shipping.ServiceProviderSettings as NovaposhtaV2ServiceProviderSettings;
                novaService.GetCreateOrderRegistration(OrderPhotolab, CurrentOrder, settings);
                plhdSuccessMessage.Visible = true;
                litSuccessMessage.Text = RS.Order.Info.NovaposhtaSuccess;
                plhdErrorMessage.Visible = false;

                if (settings.ChangeOrderStatusToShippedAfterAutomaticRegistration)
                {
                    var availableStatus = OrderService.GetAvailableStatuses(CurrentOrder, LoggedInUser);
                    if (availableStatus.Contains(OrderStatus.Shipped))
                    {
                        OrderService.SetStatus(CurrentOrder, OrderStatus.Shipped,
                            "Автоматически, т.к. заказ зарегистрирован в Новапочте вручную.");
                    }
                }

                Response.Redirect(Request.Url.AbsolutePath + "#shipping");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        protected void NovaposhtaV2DeleteOrderClick2(object sender, EventArgs e)
	    {
	        try
	        {
	            var novaService = ShippingProviderResolver.GetProvider(GetOrderPostal());
	            if (novaService != null)
	            {
	                var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
	                novaService.GetDeleteOrderRegistration(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);
	            }
	            Response.Redirect(Request.Url.AbsolutePath + "#shipping");
	        }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

	    protected void RegisterYandexOrder(object sender, EventArgs e)
	    {
            try
            {
                var yandexService = ShippingProviderResolver.GetProvider(GetOrderPostal());
                if (yandexService != null)
                {
                    var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
                    yandexService.GetCreateOrderRegistration(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);
                }
                Response.Redirect(Request.Url.AbsolutePath + "#shipping");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        protected void DeleteYandexOrder(object sender, EventArgs e)
	    {
	        try
	        {
	            var yandexService = ShippingProviderResolver.GetProvider(GetOrderPostal());
	            if (yandexService != null)
	            {
	                var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
	                yandexService.GetDeleteOrderRegistration(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);
	            }
	            Response.Redirect(Request.Url.AbsolutePath + "#shipping");
	        }
	        catch (Exception ex)
	        {
	            ShowError(ex.Message);
            }
	    }
        protected void UpdateYandexOrder(object sender, EventArgs e)
	    {
	        try
	        {
                var yandexService = ShippingProviderResolver.GetProvider(GetOrderPostal());
                if (yandexService != null)
                {
                    var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
                    var status = yandexService.GetOrderStatus(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);
                    plhdSuccessMessage.Visible = true;
                    litSuccessMessage.Text = status.ToString();
                }
                Response.Redirect(Request.Url.AbsolutePath + "#shipping");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

	    protected void RegisterImlV2Order(object sender, EventArgs e)
	    {
            var imService = ShippingProviderResolver.GetProvider(GetOrderPostal());
            if (imService != null)
            {
                var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
                try
                {
                    imService.GetCreateOrderRegistration(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);
                    plhdErrorMessage.Visible = false;
                    Response.Redirect(Request.Url.AbsolutePath + "#shipping");
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }
        protected void ImlV2UpdateStatusClick(object sender, EventArgs e)
	    {
            try
            {
                var imService = ShippingProviderResolver.GetProvider(GetOrderPostal());
                if (imService != null)
                {
                    var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
                    imService.GetOrderStatus(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);
                }
                Response.Redirect(Request.Url.AbsolutePath + "#shipping");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

	    protected void PhotomaxUpdateStatusClick(object sender, EventArgs e)
	    {
            try
            {
                var photomaxService = ShippingProviderResolver.GetProvider(GetOrderPostal());
                if (photomaxService != null)
                {
                    var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
                    var status = photomaxService.GetOrderStatus(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);
                    OrderService.SetStatus(CurrentOrder, status);
                }
                Response.Redirect(Request.Url.AbsolutePath + "#shipping");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        
	    protected void BoxberryRegisterOrderClick(object sender, EventArgs e)
	    {
	        try
	        {
	            var boxberryService = ShippingProviderResolver.GetProvider(GetOrderPostal());
	            if (boxberryService != null)
	            {
	                var postal = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
	                if (postal != null)
	                    boxberryService.GetCreateOrderRegistration(OrderPhotolab, CurrentOrder, postal.ServiceProviderSettings);
	                Response.Redirect(Request.Url.AbsolutePath + "#shipping");
	            }
            }
	        catch (Exception ex)
	        {
	            ShowError(ex.Message);
            }
        }

        protected void EvropochtaRegisterOrderClick(object sender, EventArgs e)
        {
            try
            {
                var evropochtaService = ShippingProviderResolver.GetProvider(GetOrderPostal());
                if (evropochtaService != null)
                {
                    var postal = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
                    if (postal != null)
                        evropochtaService.GetCreateOrderRegistration(OrderPhotolab, CurrentOrder, postal.ServiceProviderSettings);
                    Response.Redirect(Request.Url.AbsolutePath + "#shipping");
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

	    protected void BoxberryDeleteOrderClick(object sender, EventArgs e)
	    {
	        try
	        {
	            var boxberryService = ShippingProviderResolver.GetProvider(GetOrderPostal());
	            if (boxberryService != null)
	            {
	                var postal = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
	                if (postal != null)
	                    boxberryService.GetDeleteOrderRegistration(OrderPhotolab, CurrentOrder, postal.ServiceProviderSettings);
	                Response.Redirect(Request.Url.AbsolutePath + "#shipping");
	            }
	        }
	        catch (Exception ex)
	        {
	            ShowError(ex.Message);
            }
	    }

        protected void EvropochtaDeleteOrderClick(object sender, EventArgs e)
        {
            try
            {
                var evropochtaService = ShippingProviderResolver.GetProvider(GetOrderPostal());
                if (evropochtaService != null)
                {
                    var postal = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
                    if (postal != null)
                        evropochtaService.GetDeleteOrderRegistration(OrderPhotolab, CurrentOrder, postal.ServiceProviderSettings);
                    Response.Redirect(Request.Url.AbsolutePath + "#shipping");
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        protected void BoxberryUpdateStatus(object sender, EventArgs e)
	    {
	        try
	        {
	            var boxberryService = ShippingProviderResolver.GetProvider(GetOrderPostal());
	            if (boxberryService != null)
	            {
	                var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
	                if (shipping != null)
	                {
	                    var newStatus = boxberryService.GetOrderStatus(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);

	                    if (newStatus != CurrentOrder.Status)
	                        OrderService.SetStatus(CurrentOrder, newStatus);
	                }
	            }

	            Response.Redirect(Request.Url.AbsolutePath);
	        }
	        catch (Exception ex)
	        {
	            ShowError(ex.Message);
	        }
	    }

        protected void EvropochtaUpdateStatus(object sender, EventArgs e)
        {
            try
            {
                var evropochtaService = ShippingProviderResolver.GetProvider(GetOrderPostal());
                if (evropochtaService != null)
                {
                    var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
                    if (shipping != null)
                    {
                        var newStatus = evropochtaService.GetOrderStatus(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);

                        if (newStatus != CurrentOrder.Status)
                            OrderService.SetStatus(CurrentOrder, newStatus);
                    }
                }

                Response.Redirect(Request.Url.AbsolutePath);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        protected void DDeliveryV2RegisterOrder(object sender, EventArgs e)
	    {
	        try
	        {
	            var ddeliveryV2Service = ShippingProviderResolver.GetProvider(GetOrderPostal());
	            if (ddeliveryV2Service == null) return;

	            var postal = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
	            if (postal == null) return;

	            ddeliveryV2Service.GetCreateOrderRegistration(OrderPhotolab, CurrentOrder, postal.ServiceProviderSettings);
	        }
	        catch (Exception ex)
	        {
	            ShowError(ex.Message);
	        }
	    }
        protected void DDeliveryV2CancelOrder(object sender, EventArgs e)
	    {
	        try
	        {
	            var ddeliveryV2Service = ShippingProviderResolver.GetProvider(GetOrderPostal());
	            if (ddeliveryV2Service == null) return;

	            var postal = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
	            if (postal == null) return;

	            ddeliveryV2Service.GetDeleteOrderRegistration(OrderPhotolab, CurrentOrder, postal.ServiceProviderSettings);
	        }
	        catch (Exception ex)
	        {
	            ShowError(ex.Message);
	        }
	    }
        protected void DDeliveryV2GetOrderStatus(object sender, EventArgs e)
	    {
	        try
	        {
	            var ddeliveryV2Service = ShippingProviderResolver.GetProvider(GetOrderPostal());
	            if (ddeliveryV2Service == null) return;

	            var postal = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
	            if (postal == null) return;

	            ddeliveryV2Service.GetOrderStatus(OrderPhotolab, CurrentOrder, postal.ServiceProviderSettings);
	        }
	        catch (Exception ex)
	        {
	            ShowError(ex.Message);
	        }
	    }

	    protected void UkrposhtaRegisterOrderClick(object sender, EventArgs e)
	    {
	        try
	        {
	            var ukrposhtaService = ShippingProviderResolver.GetProvider(GetOrderPostal());
	            if (ukrposhtaService == null) return;

	            var postal = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
	            if (postal == null) return;

	            ukrposhtaService.GetCreateOrderRegistration(OrderPhotolab, CurrentOrder, postal.ServiceProviderSettings);
	        }
	        catch (Exception ex)
	        {
	            ShowError(ex.Message);
            }
	    }

	    protected void UkrposhtaGetOrderStatusClick(object sender, EventArgs e)
	    {
	        try
	        {
	            var ukrposhtaService = ShippingProviderResolver.GetProvider(GetOrderPostal());
	            if (ukrposhtaService == null) return;

	            var postal = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
	            if (postal == null) return;

	            ukrposhtaService.GetOrderStatus(OrderPhotolab, CurrentOrder, postal.ServiceProviderSettings);
	        }
	        catch (Exception ex)
	        {
                ShowError(ex.Message);
	        }
	    }

		protected void EcontRegisterOrderClick(object sender, EventArgs e)
        {
			var econtService = ShippingProviderResolver.GetProvider(GetOrderPostal());
			if(econtService != null)
            {
				var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
                try
                {
					econtService.GetCreateOrderRegistration(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);
					litErrorMessage.Visible = false;
					Response.Redirect(Request.Url.AbsolutePath + "#shipping");
				}
				catch(Exception ex)
                {
					ShowError(ex.Message);
                }
            }
        }
		protected void EcontDeleteOrderClick(object sender, EventArgs e)
		{
			var econtService = ShippingProviderResolver.GetProvider(GetOrderPostal());
			if (econtService != null)
			{
				var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
				try
				{
					econtService.GetDeleteOrderRegistration(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);
					litErrorMessage.Visible = false;
					Response.Redirect(Request.Url.AbsolutePath + "#shipping");
				}
				catch (Exception ex)
				{
					ShowError(ex.Message);
				}
			}
		}
		protected void PickpointRegisterOrderClick(object sender, EventArgs e)
		{
			var pickpointService = ShippingProviderResolver.GetProvider(GetOrderPostal());
			if (pickpointService != null)
			{
				var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
				try
				{
					pickpointService.GetCreateOrderRegistration(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);
					litErrorMessage.Visible = false;
					Response.Redirect(Request.Url.AbsolutePath + "#shipping");
				}
				catch (Exception ex)
				{
					ShowError(ex.Message);
				}
			}
		}
		protected void PickpointDeleteOrderClick(object sender, EventArgs e)
		{
			var pickpointService = ShippingProviderResolver.GetProvider(GetOrderPostal());
			if (pickpointService != null)
			{
				var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
				try
				{
					pickpointService.GetDeleteOrderRegistration(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);
					litErrorMessage.Visible = false;
					Response.Redirect(Request.Url.AbsolutePath + "#shipping");
				}
				catch (Exception ex)
				{
					ShowError(ex.Message);
				}
			}
		}
		protected void JustinDeleteOrderClick(object sender, EventArgs e)
        {
            var justinService = ShippingProviderResolver.GetProvider(GetOrderPostal());
            if (justinService != null)
            {
                var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
                try
                {
                    justinService.GetDeleteOrderRegistration(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);
                    litErrorMessage.Visible = false;
                    Response.Redirect(Request.Url.AbsolutePath + "#shipping");
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        protected void JustinRegisterOrderClick(object sender, EventArgs e)
        {
            var justinService = ShippingProviderResolver.GetProvider(GetOrderPostal());
            if (justinService != null)
            {
                var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
                try
                {
                    justinService.GetCreateOrderRegistration(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);
                    litErrorMessage.Visible = false;
                    Response.Redirect(Request.Url.AbsolutePath + "#shipping");
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        protected void PostnlRegisterOrderClick(object sender, EventArgs e)
        {
            var postnlService = ShippingProviderResolver.GetProvider(GetOrderPostal());
            if (postnlService != null)
            {
                var shipping = ShippingService.GetById<Postal>(CurrentOrder.ShippingId);
                try
                {
                    postnlService.GetCreateOrderRegistration(OrderPhotolab, CurrentOrder, shipping.ServiceProviderSettings);
                    litErrorMessage.Visible = false;
                    Response.Redirect(Request.Url.AbsolutePath + "#shipping");
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        private void ShowError(string text)
	    {
	        litErrorMessage.Text = text;
	        plhdSuccessMessage.Visible = false;
	        plhdErrorMessage.Visible = true;
	    }

    }
}