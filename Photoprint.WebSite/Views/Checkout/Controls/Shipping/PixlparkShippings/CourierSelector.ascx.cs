using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Photoprint.Core.Models;

namespace Photoprint.WebSite.Controls
{
    public partial class CourierShippingSelector : BaseShippingSelectorControl
    {
        public override OrderAddress GetSelectedOrderAddress()
		{
			OrderAddress = new OrderAddress
			{
				Country = txtCountry.Text,
				Region = txtState.Text,
                Street = txtStreet.Text,
				House = txtHouse.Text,
				Flat = txtFlat.Text,
				AddressLine1 = txtAddressLine1.Visible ? txtAddressLine1.Text : $"{txtStreet.Text} {txtHouse.Text}{(!string.IsNullOrWhiteSpace(txtFlat.Text) ? $", {txtFlat.Text}" : string.Empty)}",
				AddressLine2 = txtAddressLine2.Text,
				Description = txtDescription.Text,
				FirstName = txtFirstName.Text,
				MiddleName = UseMiddleName && (!chkAllowRegisterWithoutMiddleName?.Checked ?? false) ? txtMiddleName.Text : string.Empty,
				LastName = txtLastName.Text,
				Phone = !string.IsNullOrWhiteSpace(SmsService.ValidatePhone(txtPhone.Text)) ? SmsService.ValidatePhone(txtPhone.Text) : txtPhone.Text,
				PostalCode = txtPostalCode.Text,
				CompanyName = txtCompanyName.Text
			};

            if (HasAdditionalAddresses)
            {
                OrderAddress.City = ddlCities.SelectedItem.Text;
                OrderAddress.ShippingAddressId = int.Parse(ddlCities.SelectedValue);
            }
            else
            {
                OrderAddress.City = txtCity.Text;
                OrderAddress.ShippingAddressId = int.Parse(addressId.Value);
            }
            return OrderAddress;
        }
        protected CourierShippingSelector()
		{
			Load += PageLoad;

		}
		private void PageLoad(object sender, EventArgs e)
        {
			txtFirstName.Text = string.IsNullOrWhiteSpace(txtFirstName.Text) ? LoggedInUser.FirstName : txtFirstName.Text;
			txtLastName.Text = string.IsNullOrWhiteSpace(txtLastName.Text) ? LoggedInUser.LastName : txtLastName.Text;

            if (UseMiddleName)
                txtMiddleName.Text = string.IsNullOrWhiteSpace(txtMiddleName.Text) ? LoggedInUser.MiddleName : txtMiddleName.Text;
            else
            {
                regMiddleName.Enabled = false;
                reqMiddleName.Enabled = false;
            }

            txtPhone.Text = string.IsNullOrWhiteSpace(txtPhone.Text) ? LoggedInUser.GetCleanPhone() : txtPhone.Text;
            txtCompanyName.Text = string.IsNullOrWhiteSpace(txtCompanyName.Text) ? string.Empty : txtCompanyName.Text;
            if (IsValidationDisabled)		    
            {
                if (Shipping.ShippingServiceProviderType != ShippingServiceProviderType.Photomax)
                {
                    regFirstName.Enabled = false;
                    regLastName.Enabled = false;
                    regMiddleName.Enabled = false;
                }
                else
                {
                    var regExpr = "^[а-яА-ЯёЁa-zA-Z0-9]+$";

                    regFirstName.ValidationExpression = regExpr;
                    regFirstName.ErrorMessage = RM.GetString(RS.Common.FirstNameRequiredValidation);

                    regLastName.ValidationExpression = regExpr;
                    regLastName.ErrorMessage = RM.GetString(RS.Common.LastNameRequiredValidation);

                    regMiddleName.ValidationExpression = regExpr;
                    regMiddleName.ErrorMessage = RM.GetString(RS.Common.MiddleNameRequiredValidation);
                }
            }
        }

		protected Courier InitializedCourier { get; private set; }

        protected bool HasAdditionalAddresses { get; set; } = false;
		
		protected override void InitSelectedShipping(Shipping shipping)
		{
			var courier = shipping as Courier;
			if (courier == null) return;

			InitializedCourier = courier;

            plhdFullAddress.Visible = !courier.IsMultipleAddressLines;
            plhdAddressLine1.Visible = courier.IsMultipleAddressLines;
            plhdAddressLine2.Visible = courier.IsMultipleAddressLines;
			txtAddressLine1.TextMode = courier.IsMultipleAddressLines ? TextBoxMode.SingleLine : TextBoxMode.MultiLine;

            var baseAddress = courier.ShippingAddresses.FirstOrDefault();

            if (baseAddress == null) return;

			txtCountry.Text = baseAddress.Country;
			txtCountry.ReadOnly = !string.IsNullOrWhiteSpace(baseAddress.Country);

			txtState.Text = baseAddress.Region;
			txtState.ReadOnly = !string.IsNullOrWhiteSpace(baseAddress.Region);

            HasAdditionalAddresses = courier.ShippingAddresses.Count(sa => !string.IsNullOrWhiteSpace(sa.City)) > 1;

            if (HasAdditionalAddresses)
            {
                InitCities(courier);
            }
            else
            {
                txtCity.Text = baseAddress.City;
                txtCity.ReadOnly = !string.IsNullOrWhiteSpace(baseAddress.City);
                addressId.Value = baseAddress.Id.ToString();
            }

            plhdState.Visible = !(string.IsNullOrWhiteSpace(baseAddress.Region) && !string.IsNullOrWhiteSpace(baseAddress.City));
			plhdCountry.Visible = !(string.IsNullOrWhiteSpace(baseAddress.Country) && (!string.IsNullOrWhiteSpace(baseAddress.City) || !string.IsNullOrWhiteSpace(baseAddress.Region)));
            var lastOrder = PreviousOrders.FirstOrDefault(x=>x.ShippingId == courier.Id);
            if (lastOrder != null)
            {
                var address = lastOrder.DeliveryAddress;
                if (address != null)
                {
                    txtAddressLine1.Text = address.AddressLine1;
                    txtAddressLine2.Text = address.AddressLine2;
                    txtStreet.Text = address.Street;
                    txtHouse.Text = address.House;
                    txtFlat.Text = address.Flat;
                    txtPostalCode.Text = address.PostalCode;
                    txtDescription.Text = address.Description;
                    if (string.IsNullOrWhiteSpace(baseAddress.Country))
                        txtCountry.Text = address.Country;
                    if (string.IsNullOrWhiteSpace(baseAddress.Region))
                        txtState.Text = address.Region;
                    if (HasAdditionalAddresses)
                    {
                        if (ddlCities.SelectedIndex != 0) return;
                        foreach (ListItem item in ddlCities.Items)
                        {
                            if (address.City.Equals(item.Text))
                                item.Selected = true;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(baseAddress.City))
                            txtCity.Text = address.City;
                    }
                }                
            }
		}

        private void InitCities(Courier courier)
		{
			var currentCity = string.Empty;
            var cityId = string.Empty;
			if (!string.IsNullOrWhiteSpace(Request.Form[ddlCities.UniqueID]))
				cityId = Request.Form[ddlCities.UniqueID];

            var cities = courier.ShippingAddresses
                .OrderBy(address => address.City)
                .ThenBy(address => address.Position) // устойчево, проверено
			    .Select(address => new ListItem(address.City, address.Id.ToString(CultureInfo.InvariantCulture)))
                .Where(a => !string.IsNullOrWhiteSpace(a.Text))
                .Distinct(new DistinctListItem())
                .ToArray();

            ddlCities.Items.Clear();
            ddlCities.Items.AddRange(cities);
            ddlCities.DataBind();
            ddlCities.SelectedIndex = 0;
        }

		public override bool ValidateInput()
		{
			if (!((Courier)Shipping).IsIndexRequired) reqIndex.Enabled = false;
			if (!((Courier)Shipping).IsRegionRequired) reqState.Enabled = false;
            
		    return (cstDdlCity.IsValid || cstTxtCity.IsValid) &&
                    reqFirstName.IsValid && reqLastName.IsValid && reqPhone.IsValid &&
		           (!plhdCountry.Visible || reqCountry.IsValid) && (reqMiddleName?.IsValid ?? true) &&
		           (!plhdState.Visible || !((Courier) Shipping).IsRegionRequired || reqState.IsValid) &&
		           (!reqIndex.Enabled || reqIndex.IsValid) && (reqAddressLine1.IsValid || (reqStreet.IsValid && reqHouse.IsValid));
		}

        protected void cstDddlCity_ServerValidate(Object source, ServerValidateEventArgs args)
        {
            args.IsValid = ddlCities.SelectedValue != "0";
        }
        protected void cstTxtCity_ServerValidate(Object source, ServerValidateEventArgs args)
        {
            args.IsValid = !string.IsNullOrWhiteSpace(txtCity.Text);
        }

        private class DistinctListItem : IEqualityComparer<ListItem>
        {
            public bool Equals(ListItem x, ListItem y)
            {
                if (x == null || y == null)
                    return false;
                return x.Text == y.Text;
            }

            public int GetHashCode(ListItem address)
            {
                return address.Text.GetHashCode();
            }
        }

    }
}