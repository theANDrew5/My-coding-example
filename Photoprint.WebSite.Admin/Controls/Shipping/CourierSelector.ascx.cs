using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Photoprint.Core;
using Photoprint.Core.Models;
using Photoprint.Core.Services;

namespace Photoprint.WebSite.Admin.Controls
{
	public partial class CourierShippingSelector : BaseShippingSelectorControl
    {
		protected Courier SelectedCourier { get; set; }

        private bool IsSameShipping => CurrentOrder?.Shipping?.Id == SelectedCourier.Id;
        private bool AdressLinesVisible => DeliverySettings?.AddressSelectSettings?.UseAddressLines ??
                                           SelectedCourier.IsMultipleAddressLines;

        protected CourierShippingSelector()
		{
			Load += PageLoad;
		}

        protected bool HasAdditionalAddresses { get; set; } = false;

		private void PageLoad(object sender, EventArgs e)
        {
            if (Page.IsPostBack) return;

            reqAddressLine1.ErrorMessage = RM.GetString(RS.Order.Delivery.AddressLine1Req);

            plhdAddressLines.Visible = AdressLinesVisible;
            plhdAddress.Visible = !AdressLinesVisible;

            plhdPostalCode.Visible = SelectedCourier.IsIndexRequired;
            reqPostalCode.Enabled = SelectedCourier.IsIndexRequired;
            
            var prevAddress = CurrentOrder?.DeliveryAddress ?? new OrderAddress();

            var address = IsSameShipping
                ? SelectedCourier.ShippingAddresses.Select(sa => new ShippingAddress(sa)
                {
                    Id = sa.Id,
                    AddressLine1 = prevAddress.AddressLine1,
                    AddressLine2 = prevAddress.AddressLine2,
                    Street = prevAddress.Street,
                    Flat = prevAddress.Flat,
                    House = prevAddress.House,
                    PostalCode = prevAddress.PostalCode
                }).FirstOrDefault(sa => sa.Id == prevAddress.ShippingAddressId || sa.City == prevAddress.City)
                : SelectedCourier.ShippingAddresses.FirstOrDefault();

            txtCompanyName.Text = prevAddress.CompanyName;

            if (address is null) return;

			txtCountry.Text = address.Country;
            txtCountry.ReadOnly = !string.IsNullOrWhiteSpace(address.Country);
			txtRegion.Text = address.Region;
            txtRegion.ReadOnly = IsNewDelivery && !string.IsNullOrWhiteSpace(address.Region);

            if (HasAdditionalAddresses)
            {
                InitCities(SelectedCourier);
            }
            else
            {
                txtCity.Text = address.City;
                txtCity.ReadOnly = IsNewDelivery && !string.IsNullOrWhiteSpace(address.City);
                txtAddressId.Value = address.Id.ToString();
            }
            txtPostalCode.Text = address.PostalCode;
            txtAddressLine1.Text = address.AddressLine1;
			txtAddressLine2.Text = address.AddressLine2;
            txtStreet.Text = address.Street;
            txtHouse.Text = address.House;
            txtFlat.Text = address.Flat;
			txtDescription.Text = address.Description;            
        }


		public override void InitControl(Shipping shipping, Order order, IReadOnlyCollection<IPurchasedItem> items)
        {
            if (!(shipping is Courier courier)) throw new ArgumentException(nameof(courier));
			CurrentOrder = order ?? throw new ArgumentNullException(nameof(order));
            SelectedCourier = courier;

            HasAdditionalAddresses = SelectedCourier.ShippingAddresses.Count(sa => !string.IsNullOrWhiteSpace(sa.City)) > 1;
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
                .Distinct(new ListItemComparer())
                .ToArray();

            ddlCities.Items.Clear();
            ddlCities.Items.Add(new ListItem(RM.GetString(RS.Shipping.Address.SelectCity), "0"));
            ddlCities.Items.AddRange(cities);

            if (!string.IsNullOrWhiteSpace(cityId))
                ddlCities.SelectedValue = cities.First(c => c.Value == cityId).Value;

            ddlCities.DataBind();
            ddlCities.SelectedIndex = 0;
        }

        private class ListItemComparer : IEqualityComparer<ListItem>
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

        protected void cstDddlCity_ServerValidate(Object source, ServerValidateEventArgs args)
        {
            args.IsValid = ddlCities.SelectedValue != "0";
        }

        public override OrderAddress GeOrderAddress()
        {
            int addressId = 0;
            if (HasAdditionalAddresses)
                int.TryParse(ddlCities.SelectedValue, out addressId);
            else
                int.TryParse(txtAddressId.Value, out addressId);

            if (addressId == 0) return null;
                
            var originalAddress = SelectedCourier.ShippingAddresses.FirstOrDefault(sa => sa.Id == addressId);

            if (originalAddress == null) return null;

            var resultAddress = new OrderAddress(originalAddress)
            {
                PostalCode = txtPostalCode.Text,
                AddressLine1 = txtAddressLine1.Text,
                AddressLine2 = txtAddressLine2.Text,
                Street = txtStreet.Text,
                House = txtHouse.Text,
                Flat = txtFlat.Text,
                Description = txtDescription.Text,
                CompanyName = txtCompanyName.Text,
                Country = txtCountry.Text,
                Region = txtRegion.Text,
                City = txtCity.Text,
            };            

            return resultAddress;
        }

        public override bool ValidateInput()
		{
			return !string.IsNullOrWhiteSpace(txtCountry.Text) && !string.IsNullOrWhiteSpace(txtRegion.Text) && !string.IsNullOrWhiteSpace(txtCity.Text) &&
                   AdressLinesVisible
                        ? !string.IsNullOrWhiteSpace(txtAddressLine1.Text) && !string.IsNullOrWhiteSpace(txtAddressLine2.Text)
                        : !string.IsNullOrWhiteSpace(txtHouse.Text) &&
                          (!SelectedCourier.IsIndexRequired || !string.IsNullOrWhiteSpace(txtPostalCode.Text));
        }
    }
}