using System;
using System.Linq;
using Photoprint.Core.Models;
using Photoprint.Core.Models.DDelivery;
using Photoprint.Core.Services;

namespace Photoprint.WebSite.Controls
{
	public partial class DDeliverySelector : BaseShippingSelectorControl
    {
		protected static readonly IDDeliveryProviderService DDeliveryService = Container.GetInstance<IDDeliveryProviderService>();
		protected static readonly IShoppingCartService ShoppingCartService = Container.GetInstance<IShoppingCartService>();

		protected Postal InitializedPostal { get; private set; }

        protected DDeliveryServiceProviderSettings Settings => InitializedPostal?.ServiceProviderSettings as DDeliveryServiceProviderSettings;

		public override OrderAddress GetSelectedOrderAddress()
		{
			if (OrderAddress != null) return OrderAddress;

			DDeliveryCity city = null;
			var cityId = Newtonsoft.Json.JsonConvert.DeserializeObject<int>(Request.Form["selectedCityId"]);
			if (cityId > 0)
			{
				city = DDeliveryService.GetCityById(cityId);
			}

			var point = Newtonsoft.Json.JsonConvert.DeserializeObject<DDeliveryPickupPoint>(Request.Form["selectedPoint"]);
			var clientCalculatorResult = Newtonsoft.Json.JsonConvert.DeserializeObject<DDeliveryCalculatorResult>(Request.Form["selectedCompany"]);

			DDeliveryCalculatorResult serverResult = null;
			var cart = ShoppingCartService.GetCart(CurrentPhotolab);
			if (point == null)
			{
				serverResult = DDeliveryService
					.GetCourierPrices(CurrentPhotolab, LoggedInUser, Settings, cart.Items, city, InitializedPostal)
					.FirstOrDefault(r => r.DeliveryCompany == clientCalculatorResult.DeliveryCompany);
			}
			else
			{
				serverResult = DDeliveryService.GetPointPrice(CurrentPhotolab, LoggedInUser, Settings, cart.Items, point, InitializedPostal);
			}

			OrderAddress = new OrderAddress
			{
				Country = "Россия",
				Region = city.Region,
				City = city.Name,
				PostalCode = txtPostalCode.Text,
				Street = point != null ? null : txtStreet.Text,
				House = point != null ? null : txtHouse.Text,
				Flat = point != null ? null : txtFlat.Text,
				AddressLine1 = point != null ? point.Address : string.Format("ул.: {0}, дом: {1}, кв.: {2}", txtStreet.Text, txtHouse.Text, txtFlat.Text),
				FirstName = txtFirstName.Text,
				LastName = txtLastName.Text,
				Phone = !string.IsNullOrWhiteSpace(SmsService.ValidatePhone(txtPhone.Text)) ? SmsService.ValidatePhone(txtPhone.Text) : txtPhone.Text,
				DeliveryProperties = new DeliveryAddressProperties
				{
					DDeliveryAddressInfo = new DDeliveryAddressInfo
					{
						City = city,
						Point = point,
						Result = serverResult
					}
				}
			};

			return OrderAddress;
		}
        protected DDeliverySelector()
		{
			Load += PageLoad;
		}

		private void PageLoad(object sender, EventArgs e)
        {
			txtFirstName.Text = string.IsNullOrWhiteSpace(txtFirstName.Text) ? LoggedInUser.FirstName : txtFirstName.Text;
			txtLastName.Text = string.IsNullOrWhiteSpace(txtLastName.Text) ? LoggedInUser.LastName : txtLastName.Text;
			txtPhone.Text = string.IsNullOrWhiteSpace(txtPhone.Text) ? LoggedInUser.GetCleanPhone() : txtPhone.Text;
            if (IsValidationDisabled)
            {
                if (Shipping.ShippingServiceProviderType != ShippingServiceProviderType.Photomax)
                {
                    regFirstName.Enabled = false;
                    regLastName.Enabled = false;
                }
                else
                {
                    var regExpr = "^[а-яА-ЯёЁa-zA-Z0-9]+$";

                    regFirstName.ValidationExpression = regExpr;
                    regFirstName.ErrorMessage = RM.GetString(RS.Common.FirstNameRequiredValidation);

                    regLastName.ValidationExpression = regExpr;
                    regLastName.ErrorMessage = RM.GetString(RS.Common.LastNameRequiredValidation);
                }
            }
        }

		protected override void InitSelectedShipping(Shipping shipping)
		{
			InitSelectedShipping(shipping as Postal);
		}

		private void InitSelectedShipping(Postal postal)
		{
			if (postal == null) return;
			InitializedPostal = postal;
		}


		public override bool ValidateInput()
		{
			if (!((Postal) Shipping).IsIndexRequired) reqIndex.Enabled = false;
			var point = Newtonsoft.Json.JsonConvert.DeserializeObject<DDeliveryPickupPoint>(Request.Form["selectedPoint"]);
			if (point != null)
			{
				reqStreet.Enabled = false;
				reqFlat.Enabled = false;
				reqHouse.Enabled = false;
			}
			return reqFirstName.IsValid && reqLastName.IsValid && reqPhone.IsValid &&
						  (!reqIndex.Enabled || reqIndex.IsValid) && reqStreet.IsValid && reqHouse.IsValid && reqFlat.IsValid;
		}
	}
}