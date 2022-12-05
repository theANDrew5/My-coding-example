using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Photoprint.Core;
using Photoprint.Core.Models;
using Photoprint.WebSite.Shared;

namespace Photoprint.WebSite.Controls
{
	public partial class ToClientMailShippingSelector : BaseShippingSelectorControl
	{
        protected Postal InitializedPostal { get; private set; }

		public override OrderAddress GetSelectedOrderAddress()
		{
			var shippingAddressParsed = GetSelectedShippingAddressParse();

			if (OrderAddress == null) OrderAddress = new OrderAddress();

			OrderAddress.DeliveryProperties = shippingAddressParsed?.DeliveryProperties ?? new DeliveryAddressProperties();
            OrderAddress.ShippingAddressId = shippingAddressParsed?.Id;
			OrderAddress.Country = GetAddressValue(AddressItem.Country).Trim();
			OrderAddress.District = txtRegion.Text.Trim();
			OrderAddress.Region = GetAddressValue(AddressItem.Region).Trim();
			OrderAddress.City = GetAddressValue(AddressItem.City).Trim();
			OrderAddress.PostalCode = txtPostalCode.Text.Trim();
			OrderAddress.AddressLine1 = txtAddressLine1.Text.Trim();
			OrderAddress.AddressLine2 = txtAddressLine2.Text.Trim();

			OrderAddress.Street = GetAddressValue(AddressItem.Street);
			OrderAddress.House = txtAddressHouse.Text.Trim();
			OrderAddress.Flat = txtAddressFlat.Text.Trim();

			OrderAddress.FirstName = txtFirstName.Text.Trim();
			OrderAddress.LastName = txtLastName.Text.Trim();

            if (UseMiddleName)
            {
				OrderAddress.MiddleName = !(chkAllowRegisterWithoutMiddleName?.Checked ?? false) ? txtMiddleName.Text.Trim() : string.Empty; 
            }

			OrderAddress.Phone = !string.IsNullOrWhiteSpace(SmsService.ValidatePhone(txtPhone.Text)) ? SmsService.ValidatePhone(txtPhone.Text) : txtPhone.Text.Trim();

            return OrderAddress;

            #region Boxberry time problem
		    // Boxberry еще не доработали систему, при которой будет возможность через сайт указывать желательное время доставки клиенту.
            // Но код ниже - рабочий. Как только понадобится, раскомментируйте его
            //if (_address.Properties.BoxberryAddressInfo != null)
            //{
            //    var form = Request.Form;
            //    _address.Properties.BoxberryAddressInfo.TimeFrom1 = TimeInfoController.GetTime(form, "choose-time-from");
            //    _address.Properties.BoxberryAddressInfo.TimeTo1 = TimeInfoController.GetTime(form, "choose-time-to");
            //    _address.Properties.BoxberryAddressInfo.Commentary = txtCourierCommentary.Text.Trim();
            //    _address.Properties.BoxberryAddressInfo.TimeFrom2 = TimeInfoController.GetTime(Request.Form, "choose-time-from");
            //    _address.Properties.BoxberryAddressInfo.TimeTo2 = TimeInfoController.GetTime(Request.Form, "choose-time-to");
            //}
            #endregion
        }

		private ShippingAddress GetSelectedShippingAddressParse()
		{
			if (!ValidateSelectableInput()) return null;

			var country = GetAddressValue(AddressItem.Country);
			var region = GetAddressValue(AddressItem.Region);
			var city = GetAddressValue(AddressItem.City);

			return FrontendShippingService.GetSuitableShipingAddresses(Shipping as Postal, country, region, city);
		}

		private string GetAddressValue(AddressItem item)
		{
			var shippingAddresses = FrontendShippingService.GetAvailableShippingAddressList((Postal)Shipping);

			switch (item)
			{
				case AddressItem.Country:
					if (plhdCountrySelector.Visible && !string.IsNullOrWhiteSpace(Request.Form[ddlCountry.UniqueID]))
					{
					    int.TryParse(Request.Form[ddlCountry.UniqueID], out var countryId);

						if (shippingAddresses.Any(a => a.Id == countryId) && shippingAddresses.FirstOrDefault(a => a.Id == countryId) != null)
							return shippingAddresses.FirstOrDefault(a => a.Id == countryId).Country;
					}
					if (plhdCountryInput.Visible)
						return txtCountry.Text;

					break;
				case AddressItem.Region:
					if (plhdStateSelector.Visible && !string.IsNullOrWhiteSpace(Request.Form[ddlRegions.UniqueID]))
					{
					    int.TryParse(Request.Form[ddlRegions.UniqueID], out var regionsId);

						if (shippingAddresses.Any(a => a.Id == regionsId) && shippingAddresses.FirstOrDefault(a => a.Id == regionsId) != null)
							return shippingAddresses.FirstOrDefault(a => a.Id == regionsId).Region;
					}

					if (plhdStateInput.Visible)
						return txtState.Text;
					break;
				case AddressItem.City:
					if (plhdCitySelector.Visible && !string.IsNullOrWhiteSpace(Request.Form[ddlCities.UniqueID]))
					{
					    int.TryParse(Request.Form[ddlCities.UniqueID], out var citiesId);

						if (shippingAddresses.Any(a => a.Id == citiesId) && shippingAddresses.FirstOrDefault(a => a.Id == citiesId) != null)
							return shippingAddresses.FirstOrDefault(a => a.Id == citiesId).City;
					}

					if (plhdCityInput.Visible)
						return txtCity.Text;

					break;
                case AddressItem.Street:
                    if (!string.IsNullOrWhiteSpace(txtAddressStreet.Text))
                        return txtAddressStreet.Text.Trim();
                    break;
			}
			return string.Empty;
		}
        protected ToClientMailShippingSelector()
		{
			Load += PageLoad;
		}
        private void PageLoad(object sender, EventArgs e)
        {
			cstCountry.ErrorMessage = cstRegion.ErrorMessage = cstCity.ErrorMessage = RM.GetString("Common.NoSelectedItem");
			txtFirstName.Text = string.IsNullOrWhiteSpace(txtFirstName.Text) ? LoggedInUser.FirstName : txtFirstName.Text;
			txtLastName.Text = string.IsNullOrWhiteSpace(txtLastName.Text) ? LoggedInUser.LastName : txtLastName.Text;
			txtPhone.Text = string.IsNullOrWhiteSpace(txtPhone.Text) ? LoggedInUser.GetCleanPhone() : txtPhone.Text;

            if (UseMiddleName)
                txtMiddleName.Text = string.IsNullOrWhiteSpace(txtMiddleName.Text) ? LoggedInUser.MiddleName : txtMiddleName.Text;
            else
            {
                regMiddleName.Enabled = false;
                reqMiddleName.Enabled = false;
            }

            if (!CurrentPhotolab.Properties.IsCyrillicNamesEnabled || IsCompanyTeamMember)
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

		protected override void InitSelectedShipping(Shipping shipping)
		{
			InitSelectedShipping(shipping as Postal);
		}

		private void InitSelectedShipping(Postal postal)
		{
			if (postal == null) return;

			InitializedPostal = postal;

			if (postal.ShippingServiceProviderType == ShippingServiceProviderType.Cdek 
                || postal.ShippingServiceProviderType == ShippingServiceProviderType.Photomax
                || postal.ShippingServiceProviderType == ShippingServiceProviderType.Evropochta)
			{
				plhdAddressLines.Visible = false;
				plhdAddressSeparate.Visible = true;
			}
			else
			{
				plhdAddressLines.Visible = true;
				plhdAddressSeparate.Visible = false;

                plhdAddressLine2.Visible = postal.IsMultipleAddressLines;
				txtAddressLine1.TextMode = postal.IsMultipleAddressLines ? TextBoxMode.SingleLine : TextBoxMode.MultiLine;
			}

            // Пока Boxberry не доделает выбор доставки по времени - не используем
		    //if (postal.ShippingServiceProviderType == ShippingServiceProviderType.Boxberry && postal.PostalType == PostalType.ToClientDelivery)
		    //{
		    //    plhdCourierChooseTime.Visible = true;
		    //}

		    var userHost = SiteUtils.GetIpAddress(new HttpContextWrapper(Context).Request);
		    var geoInfo = GeoIpService.GetInfo(CurrentPhotolab, CurrentLanguage, userHost);

            var currentCountry = InitCountries(postal, geoInfo);
			var currentRegion = InitRegions(postal, currentCountry, geoInfo);
			var currentCity = InitCities(postal, currentCountry, currentRegion, geoInfo);

			if ((!string.IsNullOrWhiteSpace(currentCity) || !string.IsNullOrWhiteSpace(currentRegion)) && string.IsNullOrWhiteSpace(currentCountry))
				plhdCountryInput.Visible = false;			
		}
        private string InitCountries(Postal postal, GeoIpInfoDTO geoInfo)
		{
			var shippingAddresses = FrontendShippingService.GetAvailableShippingAddressList(postal);
			var emptyCountries = shippingAddresses.Where(a => string.IsNullOrWhiteSpace(a.Country)).ToList();

			var countries = (from address in shippingAddresses
							 orderby address.Country
							 select new ListItem(address.Country, address.Id.ToString(CultureInfo.InvariantCulture))).Where(a => !string.IsNullOrWhiteSpace(a.Text)).Distinct(new DistinctListItem()).ToArray();

			var currentCountry = string.Empty;
			if (!emptyCountries.Any() && countries.Length > 1)
			{
				ddlCountry.Items.Clear();
				plhdCountrySelector.Visible = true;
				plhdCountryInput.Visible = false;

				var countryId = string.Empty;
				if (!string.IsNullOrWhiteSpace(Request.Form[ddlCountry.UniqueID]))
					countryId = Request.Form[ddlCountry.UniqueID];

				var selectedCountry = (from c in countries where c.Value == countryId select c).FirstOrDefault();

				ddlCountry.Items.Add(new ListItem(RM.GetString("Shop.Checkout.SelectCountry", false), "0"));
				ddlCountry.Items.AddRange(countries);
			    if (selectedCountry == null && !string.IsNullOrEmpty(geoInfo?.Country))
			    {
			        selectedCountry = countries.FirstOrDefault(x => x.Text.Equals(geoInfo.Country));
			    }
                if (selectedCountry != null)
				{
					ddlCountry.SelectedValue = selectedCountry.Value;
					currentCountry = selectedCountry.Text;
				}

				ddlCountry.DataBind();
			}
			else
			{
				plhdCountrySelector.Visible = false;
				plhdCountryInput.Visible = true;

				if (countries.Length == 1 && !emptyCountries.Any())
				{
					txtCountry.Text = countries.First().Text;
					currentCountry = txtCountry.Text;
					//txtCountry.ReadOnly = true;
				}
			}

			return currentCountry;
		}
        private string InitRegions(Postal postal, string currentCountry, GeoIpInfoDTO geoInfo)
		{
			var currentRegion = string.Empty;
			var shippingAddresses = FrontendShippingService.GetAvailableShippingAddressList(postal);

			var regionId = string.Empty;
			if (!string.IsNullOrWhiteSpace(Request.Form[ddlRegions.UniqueID]))
				regionId = Request.Form[ddlRegions.UniqueID];
            
			plhdStateSelector.Visible = false;
			plhdStateInput.Visible = true;

			if (!string.IsNullOrWhiteSpace(currentCountry))
			{
				var emptyRegions = shippingAddresses.Where(a => string.IsNullOrWhiteSpace(a.Region) && a.Country == currentCountry);

				var regions = shippingAddresses
				     .Where(address => address.Country == currentCountry)
				     .OrderBy(address => address.Position)
				     .ThenBy(address => address.Region)
				     .Select(address => new ListItem(address.Region, address.Id.ToString(CultureInfo.InvariantCulture)))
				     .Where(a => !string.IsNullOrWhiteSpace(a.Text))
				     .Distinct(new DistinctListItem())
				     .ToArray();

				if (!emptyRegions.Any() && regions.Length > 1)
				{
					ddlRegions.Items.Clear();
					plhdStateSelector.Visible = true;
					plhdStateInput.Visible = false;

					var selectedState = (from c in regions where c.Value == regionId select c).FirstOrDefault();

					ddlRegions.Items.Add(new ListItem(RM.GetString("Shop.Checkout.SelectRegion", false), "0"));
					ddlRegions.Items.AddRange(regions);
				    if (selectedState == null && !string.IsNullOrEmpty(geoInfo?.Region))
				    {
				        selectedState = regions.FirstOrDefault(x => x.Text.Equals(geoInfo.Region));
				    }
                    if (selectedState != null)
					{
						ddlRegions.SelectedValue = selectedState.Value;
						currentRegion = selectedState.Text;
					}
					ddlRegions.DataBind();
				}
				else
				{
					if (regions.Count() == 1 && !emptyRegions.Any())
					{
						txtState.Text = regions.First().Text;
						currentRegion = txtState.Text;
						txtState.ReadOnly = true;
					}
				}
			}

			return currentRegion;
		}

		private Func<ShippingAddress, bool> GetCityFilter(string country, string region)
        {
			bool isEmptyCountry = string.IsNullOrWhiteSpace(country);
			bool isEmptyRegion = string.IsNullOrWhiteSpace(region);

			if (!isEmptyCountry && !isEmptyRegion) return address => address.Country == country && address.Region == region;
			if (!isEmptyCountry) return address => address.Country == country;
			if (!isEmptyRegion) return address => address.Region == region;
			return default;
		}

		private string InitCities(Postal postal, string currentCountry, string currentRegion, GeoIpInfoDTO geoInfo)
		{
			var currentCity = string.Empty;
			var shippingAddresses = FrontendShippingService.GetAvailableShippingAddressList(postal);

			var cityId = string.Empty;
			if (!string.IsNullOrWhiteSpace(Request.Form[ddlCities.UniqueID]))
				cityId = Request.Form[ddlCities.UniqueID];
            
			plhdCitySelector.Visible = false;
			plhdCityInput.Visible = true;

			if (!string.IsNullOrWhiteSpace(currentCountry) || !string.IsNullOrWhiteSpace(currentRegion))
			{
				var emptyCities = shippingAddresses.Where(a => string.IsNullOrWhiteSpace(a.City) && a.Country == currentCountry && a.Region == currentRegion);
                var cities = shippingAddresses
                    .Where(GetCityFilter(currentCountry, currentRegion))
                    .OrderBy(x => x.Position)
                    .ThenBy(x => x.City)
                    .Select(x => new ListItem(x.City, x.Id.ToString(CultureInfo.InvariantCulture)))
                    .Where(a => !string.IsNullOrWhiteSpace(a.Text))
                    .Distinct(new DistinctListItem())
					.ToArray();

				if (!emptyCities.Any() && cities.Length > 1)
				{
					ddlCities.Items.Clear();
					plhdCitySelector.Visible = true;
					plhdCityInput.Visible = false;

					var selectedCity = (from c in cities where c.Value == cityId select c).FirstOrDefault();

					ddlCities.Items.Add(new ListItem(RM.GetString("Shop.Checkout.SelectCity", false), "0"));
					ddlCities.Items.AddRange(cities);

                    //for YarkiyMir
				    if (selectedCity == null && !string.IsNullOrEmpty(Request.Cookies["place_name"]?.Value))
				    {
				        var encodedCityName = HttpUtility.UrlDecode(Request.Cookies["place_name"].Value);
                        selectedCity = cities.FirstOrDefault(x => x.Text.Equals(encodedCityName));
				    }

                    if (selectedCity == null && !string.IsNullOrEmpty(geoInfo?.City))
                    {
                        selectedCity = cities.FirstOrDefault(x => x.Text.Equals(geoInfo.City));
                    }

                    if (selectedCity != null)
					{
						ddlCities.SelectedValue = selectedCity.Value;
						currentCity = selectedCity.Text;
					}
					ddlCities.DataBind();
				}
				else
				{
					if (cities.Length == 1 && !emptyCities.Any())
					{
						txtCity.Text = cities.First().Text;
						currentCity = txtCity.Text;
						txtCity.ReadOnly = true;
					}
				}
			}

			return currentCity;
		}

        private AddressValidationResult _addressValidationResult;
        protected AddressValidationResult ValidateAddress()
        {
            var address = GetSelectedOrderAddress();
            if (_addressValidationResult == null)
            {
                var provider = ShippingProviderResolverService.GetProvider((Postal)Shipping);
                _addressValidationResult = provider?.TestDelivery(Shipping.ServiceProviderSettings, address, InitializedPostal) ?? AddressValidationResult.GetValidState();
            }
	        return _addressValidationResult;
	    }

		public override bool ValidateInput()
		{
            if (!((Postal)Shipping).IsIndexRequired) reqIndex.Enabled = false;
            if (!((Postal)Shipping).IsRegionRequired) reqState.Enabled = false;

            var isValid = reqFirstName.IsValid && reqLastName.IsValid && reqPhone.IsValid && (reqMiddleName?.IsValid ?? true) &&
                          (!plhdCountrySelector.Visible || cstCountry.IsValid) &&
                          (!plhdCountryInput.Visible || reqCountry.IsValid) &&
                          (!plhdStateSelector.Visible || cstRegion.IsValid) && (!plhdStateInput.Visible || reqState.IsValid) &&
                          (!plhdCitySelector.Visible || cstCity.IsValid) && (!plhdCityInput.Visible || reqCity.IsValid) &&
                                   (!plhdRegionInput.Visible || cstRegionCorrect.IsValid) &&
                          (!reqIndex.Enabled || reqIndex.IsValid) && reqAddressLine1.IsValid;
            var shippingValidationResult = ValidateAddress();
            SetLocalizedCustomErrorString(shippingValidationResult.Message);
            return isValid && shippingValidationResult.IsValid;
        }

	    protected override void SetLocalizedCustomErrorString(string input)
	    {
	        if (string.IsNullOrWhiteSpace(input)) return;

	        var provider = ShippingProviderResolverService.GetProvider((Postal)Shipping);
	        if (provider != null)
	        {
	            BaseLocalizedShippingError = provider.GetLocalizedString(input, CurrentPhotolab, CurrentLanguage);
	        }
	    }

        private bool ValidateSelectableInput()
		{
			if (plhdCountrySelector.Visible && ddlCountry.SelectedValue == "0")
				return false;

			if (plhdStateSelector.Visible && ddlRegions.SelectedValue == "0")
				return false;

			return !plhdCitySelector.Visible || ddlCities.SelectedValue != "0";
		}

		private class DistinctListItem : IEqualityComparer<ListItem>
		{
			public bool Equals(ListItem x, ListItem y)
			{
				if (x == null || y == null) return false;
				return x.Text == y.Text;
			}

			public int GetHashCode(ListItem address)
			{
				return address.Text.GetHashCode();
			}
		}

		protected void CstCountryServerValidate(Object source, ServerValidateEventArgs args)
		{			
			args.IsValid = ddlCountry.SelectedValue != "0";
		}
		protected void CstRegionServerValidate(Object source, ServerValidateEventArgs args)
		{
			args.IsValid = ddlRegions.SelectedValue != "0";
		}
		protected void CstCityServerValidate(Object source, ServerValidateEventArgs args)
		{
			args.IsValid = ddlCities.SelectedValue != "0";
		}
	    protected void CstNameCorrect(Object source, ServerValidateEventArgs args)
	    {
	        args.IsValid = ValidateAddress().IsNameValid.GetValueOrDefault(true);
	    }
        protected void CstCityCorrect(Object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidateAddress().IsCityValid.GetValueOrDefault(true);
        }
        protected void CstPhoneCorrect(Object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidateAddress().IsPhoneValid.GetValueOrDefault(true);
        }
        protected void CstHouseCorrect(Object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidateAddress().IsHouseValid.GetValueOrDefault(true);
        }
        protected void CstFlatCorrect(Object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidateAddress().IsFlatValid.GetValueOrDefault(true);
        }
        protected void CstStreetCorrect(Object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidateAddress().IsStreetValid.GetValueOrDefault(true);
        }
	    protected void CstAreaRegionServerValidate(object source, ServerValidateEventArgs args)
	    {
	        args.IsValid = ValidateAddress().IsRegionValid.GetValueOrDefault(true);
        }
	}
}