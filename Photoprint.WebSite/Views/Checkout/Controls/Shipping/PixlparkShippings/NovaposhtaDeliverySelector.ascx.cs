using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Photoprint.Core;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using Photoprint.WebSite.Shared;

namespace Photoprint.WebSite.Controls
{
    public partial class NovaposhtaDeliverySelector : BaseShippingSelectorControl
    {
        protected static readonly INovaposhtaV2Services _novaposhtaV2Services = Container.GetInstance<INovaposhtaV2Services>();

        protected Postal InitializedPostal { get; private set; }


        public override OrderAddress GetSelectedOrderAddress()
        {
            var parsedShippingAddress = GetSelectedShippingAddressParse();
            if (OrderAddress == null) OrderAddress = new OrderAddress();

            OrderAddress.DeliveryProperties = parsedShippingAddress?.DeliveryProperties ?? new DeliveryAddressProperties();
            OrderAddress.Country = GetAddressValue(AddressItem.Country).Trim();
            OrderAddress.Region = GetAddressValue(AddressItem.Region).Trim();
            OrderAddress.City = GetAddressValue(AddressItem.City).Trim();

            if (InitializedPostal.PostalType == PostalType.ToStorageDelivery)
            {
                OrderAddress.AddressLine1 = GetAddressValue(AddressItem.AddressLine1).Trim();
                OrderAddress.Description = txtFirstName.Text.Trim();
            }
            else
            {
                OrderAddress.District = txtRegion.Text.Trim();
                OrderAddress.PostalCode = txtPostalCode.Text.Trim();
                OrderAddress.Street = GetAddressValue(AddressItem.Street);
                OrderAddress.House = txtAddressHouse.Text.Trim();
                OrderAddress.Flat = txtAddressFlat.Text.Trim();
            }

            OrderAddress.FirstName = txtFirstName.Text.Trim();
            OrderAddress.LastName = txtLastName.Text.Trim();
            OrderAddress.MiddleName = !(chkAllowRegisterWithoutMiddleName?.Checked ?? false) ? txtMiddleName.Text.Trim() : string.Empty;
            OrderAddress.Phone = !string.IsNullOrWhiteSpace(SmsService.ValidatePhone(txtPhone.Text)) ? SmsService.ValidatePhone(txtPhone.Text) : txtPhone.Text.Trim();

            return OrderAddress;
        }

        private ShippingAddress GetSelectedShippingAddressParse()
        {
            if (!ValidateSelectableInput()) return null;

            if (InitializedPostal.PostalType == PostalType.ToClientDelivery)
            {
                var country = GetAddressValue(AddressItem.Country);
                var region = GetAddressValue(AddressItem.Region);
                var city = GetAddressValue(AddressItem.City);

                return FrontendShippingService.GetSuitableShipingAddresses(Shipping as Postal, country, region, city);
            }
            else
            {
                var shippingAddresses = FrontendShippingService.GetAvailableShippingAddressList((Postal) Shipping);

                var addressId = Request.Form[ddlAddresses.UniqueID];
                var ddlTarget = Request.Form["ddlTarget"];
                if (addressId == "0" || ddlTarget == "ddlCities")
                {
                    var sAddresses = shippingAddresses.Where(sa =>
                        sa.Id == int.Parse(!string.IsNullOrWhiteSpace(Request.Form[ddlCities.UniqueID])
                            ? Request.Form[ddlCities.UniqueID]
                            : ddlCities.SelectedValue));
                    if (sAddresses.Count() == 1)
                        addressId = sAddresses.First().Id.ToString(CultureInfo.InvariantCulture);
                }

                return shippingAddresses.FirstOrDefault(sa => sa.Id == int.Parse(!string.IsNullOrWhiteSpace(addressId)
                                                                  ? addressId
                                                                  : ddlAddresses.SelectedValue));
            }

        }

        private string GetAddressValue(AddressItem item)
        {
            var shippingAddresses = FrontendShippingService.GetAvailableShippingAddressList((Postal)Shipping);

            switch (item)
            {
                case AddressItem.Country:
                    if (plhdCountrySelector.Visible && !string.IsNullOrWhiteSpace(Request.Form[ddlCountries.UniqueID]))
                    {
                        int.TryParse(Request.Form[ddlCountries.UniqueID], out var countryId);

                        if (shippingAddresses.Any(a => a.Id == countryId) && shippingAddresses.FirstOrDefault(a => a.Id == countryId) != null)
                            return shippingAddresses.FirstOrDefault(a => a.Id == countryId)?.Country;
                    }
                    if (plhdCountryInput.Visible)
                        return txtCountry.Text;
                    break;
                case AddressItem.Region:
                    if (plhdStateSelector.Visible && !string.IsNullOrWhiteSpace(Request.Form[ddlStates.UniqueID]))
                    {
                        int.TryParse(Request.Form[ddlStates.UniqueID], out var regionsId);

                        if (shippingAddresses.Any(a => a.Id == regionsId) && shippingAddresses.FirstOrDefault(a => a.Id == regionsId) != null)
                            return shippingAddresses.FirstOrDefault(a => a.Id == regionsId)?.Region;
                    }

                    if (plhdStateInput.Visible)
                        return txtState.Text;
                    break;
                case AddressItem.City:
                    if (plhdCitySelector.Visible && !string.IsNullOrWhiteSpace(Request.Form[ddlCities.UniqueID]))
                    {
                        int.TryParse(Request.Form[ddlCities.UniqueID], out var citiesId);

                        if (shippingAddresses.Any(a => a.Id == citiesId) && shippingAddresses.FirstOrDefault(a => a.Id == citiesId) != null)
                            return shippingAddresses.FirstOrDefault(a => a.Id == citiesId)?.City;
                    }

                    if (plhdCityInput.Visible)
                        return txtCity.Text;
                    break;
                case AddressItem.Street:
                    if (!string.IsNullOrWhiteSpace(txtAddressStreet.Text))
                        return txtAddressStreet.Text.Trim();

                    var streetItem = ddlStreets?.SelectedItem;
                    if (ddlStreets?.Items.Count > 0 && streetItem != null)
                    {
                        return streetItem.Text.Trim();
                    }
                    break;
                case AddressItem.AddressLine1:
                    if (plhdAddresses.Visible && !string.IsNullOrWhiteSpace(Request.Form[ddlAddresses.UniqueID]))
                    {
                        int.TryParse(Request.Form[ddlAddresses.UniqueID], out var addressesId);

                        if (shippingAddresses.FirstOrDefault(a => a.Id == addressesId) != null)
                            return shippingAddresses.FirstOrDefault(a => a.Id == addressesId).AddressLine1;
                    }
                    break;
            }
            return string.Empty;
        }

        protected NovaposhtaDeliverySelector()
        {
            Load += Page_Load;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            txtFirstName.Text = string.IsNullOrWhiteSpace(txtFirstName.Text) ? LoggedInUser.FirstName : txtFirstName.Text;
            txtLastName.Text = string.IsNullOrWhiteSpace(txtLastName.Text) ? LoggedInUser.LastName : txtLastName.Text;
            txtMiddleName.Text = string.IsNullOrWhiteSpace(txtMiddleName.Text) ? LoggedInUser.MiddleName : txtMiddleName.Text;
            txtPhone.Text = string.IsNullOrWhiteSpace(txtPhone.Text) ? LoggedInUser.GetCleanPhone() : txtPhone.Text;
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
                    const string regExpr = "^[а-яёєiїґА-ЯЁЄIЇҐ][а-яёєiїґА-ЯЁЄIЇҐ0-9]+$";

                    regFirstName.ValidationExpression = regExpr;
                    regFirstName.ErrorMessage = RM.GetString(RS.Common.FirstNameRequiredValidation);

                    regLastName.ValidationExpression = regExpr;
                    regLastName.ErrorMessage = RM.GetString(RS.Common.LastNameRequiredValidation);

                    regMiddleName.ValidationExpression = regExpr;
                    regMiddleName.ErrorMessage = RM.GetString(RS.Common.MiddleNameRequiredValidation);
                }
            }

            ddlAddresses.Style["Float"] = "none";
            ddlCities.Style["Float"] = "none";
            ddlCountries.Style["Float"] = "none";
            ddlStates.Style["Float"] = "none";
            ddlStreets.Style["Float"] = "none";
        }

        protected override void InitSelectedShipping(Shipping shipping)
        {
            InitSelectedShipping(shipping as Postal);
        }

        private void InitSelectedShipping(Postal postal)
        {
            if (postal == null) return;
            InitializedPostal = postal;

            var userHost = SiteUtils.GetIpAddress(new HttpContextWrapper(Context).Request);
            var geoInfo = GeoIpService.GetInfo(CurrentPhotolab, CurrentLanguage, userHost);

            var currentCountry = InitCountries(postal, geoInfo);
            var currentState = InitStates(postal, currentCountry, geoInfo);
            var currentCity = InitCities(postal, currentCountry, currentState, geoInfo);

            if (postal.PostalType == PostalType.ToClientDelivery)
            {
                plhdRegionInput.Visible = true;
                plhdClientShipping.Visible = true;

                InitStreets(postal, currentCity);

                if (!string.IsNullOrWhiteSpace(currentCity) && string.IsNullOrWhiteSpace(currentState) && !postal.IsRegionRequired)
                    plhdStateInput.Visible = false;

                if ((!string.IsNullOrWhiteSpace(currentCity) || !string.IsNullOrWhiteSpace(currentState)) && string.IsNullOrWhiteSpace(currentCountry))
                    plhdCountryInput.Visible = false;
            }
            else
            {
                plhdStorageShipping.Visible = true;

                InitAddresses(postal, currentCountry, currentState, currentCity);
            }
        }

        private string InitCountries(Postal postal, GeoIpInfoDTO geoInfo)
        {
            var shippingAddresses = FrontendShippingService.GetAvailableShippingAddressList(postal);
            var countries = shippingAddresses
                .OrderBy(address => address.Position)
                .ThenBy(address => address.Country)
                .Select(address => new ListItem(address.Country, address.Id.ToString(CultureInfo.InvariantCulture)))
                .Where(a => !string.IsNullOrWhiteSpace(a.Text))
                .Distinct(new DistinctListItem())
                .ToArray();

            var currentCountry = string.Empty;

            if (countries.Any())
            {
                ddlCountries.Items.Clear();

                var countryId = string.Empty;
                if (!string.IsNullOrWhiteSpace(Request.Form[ddlCountries.UniqueID]))
                    countryId = Request.Form[ddlCountries.UniqueID];

                var selectedCountry = countries.FirstOrDefault(c => c.Value == countryId);

                if (countries.Length > 1)
                {
                    plhdCountrySelector.Visible = true;

                    ddlCountries.Items.Add(new ListItem(RM.GetString(RS.Shop.Checkout.SelectCountry, false), "0"));
                    ddlCountries.Items.AddRange(countries);
                }
                else
                {
                    plhdCountrySelector.Visible = false;
                    plhdCountryInput.Visible = true;
                    txtCountry.Enabled = false;
                    selectedCountry = countries.FirstOrDefault();
                }

                if (selectedCountry == null && !string.IsNullOrEmpty(geoInfo?.Country))
                    selectedCountry = countries.FirstOrDefault(c => c.Text.Equals(geoInfo.Country));

                if (selectedCountry != null)
                {
                    if (plhdCountrySelector.Visible)
                        ddlCountries.SelectedValue = selectedCountry.Value;
                    else
                        txtCountry.Text = selectedCountry.Text;

                    currentCountry = selectedCountry.Text;
                }
                else
                    currentCountry = RM.GetString(RS.Shop.Checkout.SelectCountry, false);

                ddlCountries.DataBind();
            }
            else
            {
                plhdCountrySelector.Visible = false;
                plhdCountryInput.Visible = postal.PostalType == PostalType.ToClientDelivery;
            }

            return currentCountry;
        }

        private string InitStates(Postal postal, string currentCountry, GeoIpInfoDTO geoInfo)
        {
            var shippingAddresses = FrontendShippingService.GetAvailableShippingAddressList(postal);
            var states = shippingAddresses
                .Where(address => address.Country == currentCountry)
                .OrderBy(address => address.Position)
                .ThenBy(address => address.Region)
                .Select(address => new ListItem(address.Region, address.Id.ToString(CultureInfo.InvariantCulture)))
                .Where(a => !string.IsNullOrWhiteSpace(a.Text))
                .Distinct(new DistinctListItem())
                .ToArray();

            var currentState = string.Empty;

            if (states.Any())
            {
                ddlStates.Items.Clear();

                var stateId = string.Empty;
                if (!string.IsNullOrWhiteSpace(Request.Form[ddlStates.UniqueID]))
                    stateId = Request.Form[ddlStates.UniqueID];

                var selectedState = states.FirstOrDefault(s => s.Value == stateId);

                if (states.Length > 1)
                {
                    plhdStateSelector.Visible = true;

                    ddlStates.Items.Add(new ListItem(RM.GetString(RS.Shop.Checkout.SelectRegion, false), "0"));
                    ddlStates.Items.AddRange(states);
                }
                else
                {
                    plhdStateSelector.Visible = false;
                    plhdStateInput.Visible = true;
                    selectedState = states.FirstOrDefault();
                }

                if (selectedState == null && !string.IsNullOrEmpty(geoInfo?.Region))
                    selectedState = states.FirstOrDefault(s => s.Text.Equals(geoInfo.Region));

                if (selectedState != null)
                {
                    if (plhdStateSelector.Visible)
                        ddlStates.SelectedValue = selectedState.Value;
                    else
                        txtState.Text = selectedState.Text;

                    currentState = selectedState.Text;
                }
                else
                    currentState = RM.GetString(RS.Shop.Checkout.SelectRegion, false);

                ddlStates.DataBind();
            }
            else
            {
                plhdStateSelector.Visible = false;
                plhdStateInput.Visible = postal.PostalType == PostalType.ToClientDelivery;
            }

            return currentState;
        }

        private string InitCities(Postal postal, string currentCountry, string currentState, GeoIpInfoDTO geoInfo)
        {
            var shippingAddresses = FrontendShippingService.GetAvailableShippingAddressList(postal);
            var cities = shippingAddresses
                .Where(address => address.Country == currentCountry && address.Region == currentState)
                .OrderBy(address => address.Position)
                .ThenBy(address => address.City)
                .Select(address => new ListItem(address.City, address.Id.ToString(CultureInfo.InvariantCulture)))
                .Where(a => !string.IsNullOrWhiteSpace(a.Text))
                .Distinct(new DistinctListItem())
                .ToArray();

            var currentCity = string.Empty;

            if (cities.Any())
            {
                ddlCities.Items.Clear();

                var cityId = string.Empty;
                if (!string.IsNullOrWhiteSpace(Request.Form[ddlCities.UniqueID]))
                    cityId = Request.Form[ddlCities.UniqueID];

                var selectedCity = cities.FirstOrDefault(c => c.Value == cityId);

                if (cities.Length > 1)
                {
                    plhdCitySelector.Visible = true;

                    ddlCities.Items.Add(new ListItem(RM.GetString(RS.Shop.Checkout.SelectCity, false), "0"));
                    ddlCities.Items.AddRange(cities);
                }
                else
                {
                    plhdCityInput.Visible = true;
                    selectedCity = cities.FirstOrDefault();
                }

                if (selectedCity == null && !string.IsNullOrEmpty(geoInfo?.City))
                    selectedCity = cities.FirstOrDefault(c => c.Text.Equals(geoInfo.City));

                if (selectedCity != null)
                {
                    if (plhdCitySelector.Visible)
                        ddlCities.SelectedValue = selectedCity.Value;
                    else
                        txtCity.Text = selectedCity.Text;

                    currentCity = selectedCity.Text;
                }
                else
                    currentCity = RM.GetString(RS.Shop.Checkout.SelectCity, false);

                ddlCities.DataBind();
            }
            else
            {
                plhdCitySelector.Visible = false;
                plhdCityInput.Visible = postal.PostalType == PostalType.ToClientDelivery;
            }

            return currentCity;
        }

        private void InitStreets(Postal postal, string currentCity)
        {
            ddlStreets.Items.Clear();

            var addresses = FrontendShippingService.GetAvailableShippingAddressList(postal);

            var address = addresses.FirstOrDefault(a => a.City == currentCity);
            if (address == null)
            {
                ShowStreetInputInsteadDDL();
                return;
            }

            if (!(postal.ServiceProviderSettings is NovaposhtaV2ServiceProviderSettings properties))
            {
                ShowStreetInputInsteadDDL();
                return;
            }

            var cityRef = address.DeliveryProperties?.NovaposhtaV2AddressInfo?.CityRef;
            if (cityRef == null)
            {
                ShowStreetInputInsteadDDL();
                return;
            }

            var streets = _novaposhtaV2Services.GetStreets(properties.ApiKey, cityRef);
            if (streets.Count == 0)
            {
                ShowStreetInputInsteadDDL();
                return;
            }

            plhdStreetSelector.Visible = true;
            ddlStreets.Items.AddRange(streets.Select(s => new ListItem($"{s.Type} {s.Description}", s.Ref)).ToArray());
            ddlStreets.SelectedIndex = 0;
            ddlStreets.DataBind();
        }

        private void ShowStreetInputInsteadDDL()
        {
            plhdStreetSelector.Visible = false;
            plhdStreetInput.Visible = true;
        }

        private void InitAddresses(Postal postal, string currentCountry, string currentState, string currentCity)
        {
            var shippingAddresses = FrontendShippingService.GetAvailableShippingAddressList(postal).ToList();
            var addresses = shippingAddresses
                .Where(address => address.Country == currentCountry && address.Region == currentState && address.City == currentCity)
                .OrderBy(address => address.Position)
                .ThenBy(address => address.AddressName)
                .Select(address => new ListItem($"{address.AddressName} ({address.AddressLine1})", address.Id.ToString(CultureInfo.InvariantCulture)))
                .Where(a => !string.IsNullOrWhiteSpace(a.Text))
                .Distinct(new DistinctListItem())
                .ToArray();

            ddlAddresses.Items.Clear();
            if (addresses.Any())
            {
                var addressId = string.Empty;
                if (!string.IsNullOrWhiteSpace(Request.Form[ddlAddresses.UniqueID]))
                    addressId = Request.Form[ddlAddresses.UniqueID];

                var selectedAddress = addresses.FirstOrDefault(a => a.Value == addressId);

                if (addresses.Length > 1)
                    ddlAddresses.Items.Add(new ListItem(RM.GetString(RS.Shop.Checkout.SelectStorage, false), "0"));
                else
                    selectedAddress = addresses.FirstOrDefault();

                ddlAddresses.Items.AddRange(addresses);

                if (selectedAddress != null)
                {
                    ddlAddresses.SelectedValue = selectedAddress.Value;

                    var selectedAddressId = int.Parse(selectedAddress.Value);
                    var realAddress = shippingAddresses.FirstOrDefault(a => a.Id == selectedAddressId);
                    if (!string.IsNullOrWhiteSpace(realAddress?.DisplayDescription))
                    {
                        plhdDescription.Visible = true;
                        litDescription.Text = realAddress.DisplayDescription;
                    }

                    if (realAddress != null)
                    {
                        var priceList = FrontendShippingPriceService.GetById(CurrentPhotolab, realAddress.PriceId).PriceList;
                        if (priceList.IsAvailableWeightConstrain)
                        {
                            var shoppingCartWeight = ShippableItems.Sum(x => x.ItemWeight);

                            if (priceList.MaximumWeight < shoppingCartWeight)
                            {
                                plhdMaxWeight.Visible = true;
                                var weightMeasurement = CurrentPhotolab.GetMeasurementString(MeasurementType.Weight);
                                litMaxWeight.Text = string.Format(RM.GetString(RS.Shop.Checkout.MaximumWeight),
                                    priceList.MaximumWeight, weightMeasurement, shoppingCartWeight,
                                    weightMeasurement);
                            }
                        }
                    }
                }

                ddlAddresses.DataBind();
            }
            else if (currentCity == RM.GetString(RS.Shop.Checkout.SelectCity))
                ddlAddresses.Items.Add(new ListItem(RM.GetString(RS.Shop.Checkout.SelectStorage, false), "0"));
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

            var isValid = ValidateSelectableInput() && reqFirstName.IsValid && reqLastName.IsValid && reqMiddleName.IsValid && reqPhone.IsValid &&
                          (!plhdCountrySelector.Visible || cstCountry.IsValid) && (!plhdCountryInput.Visible || reqCountry.IsValid) &&
                          (!plhdStateSelector.Visible || cstState.IsValid) && (!plhdStateInput.Visible || reqState.IsValid) &&
                          (!plhdRegionInput.Visible || cstRegion.IsValid) &&
                          (!plhdCitySelector.Visible || cstCity.IsValid) && (!plhdCityInput.Visible || reqCity.IsValid) &&
                          (!reqIndex.Enabled || reqIndex.IsValid) &&
                          (!plhdClientShipping.Visible || (!plhdStreetSelector.Visible || cstStreet.IsValid) && (!plhdStreetInput.Visible || reqAddressStreet.IsValid) &&
                           reqAddressHouse.IsValid && reqAddressFlat.IsValid) &&
                          (!plhdStorageShipping.Visible || !plhdAddresses.Visible || cstAddress.IsValid);
            var shippingValidationResult = ValidateAddress();
            SetLocalizedCustomErrorString(shippingValidationResult.Message);
            return isValid && shippingValidationResult.IsValid;
        }

        private bool ValidateSelectableInput()
        {
            if (ddlCountries.Items.Count > 1 && ddlCountries.SelectedValue == "0")
                return false;

            if (ddlStates.Items.Count > 1 && ddlStates.SelectedValue == "0")
                return false;

            if (ddlCities.Items.Count > 1 && ddlCities.SelectedValue == "0")
                return false;

            if (ddlAddresses.Items.Count > 1 && ddlAddresses.SelectedValue == "0")
                return false;

            if (ddlStreets.Items.Count > 1 && ddlStreets.SelectedValue == "0")
                return false;

            if (plhdMaxWeight.Visible) return false;

            return true;
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

        protected bool CstCityReqFunc => ddlStreets.SelectedValue != "0";

        protected void CstCountryServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ddlCountries.SelectedValue != "0";
        }
        protected void CstStateServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ddlStates.SelectedValue != "0";
        }
        protected void CstCityServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ddlCities.SelectedValue != "0";
        }
        protected void CstStreetServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ddlStreets.SelectedValue != "0";
        }
        protected void CstNameCorrect(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidateAddress().IsNameValid.GetValueOrDefault(true);
        }
        protected void CstCityCorrect(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidateAddress().IsCityValid.GetValueOrDefault(true);
        }
        protected void CstPhoneCorrect(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidateAddress().IsPhoneValid.GetValueOrDefault(true);
        }
        protected void CstHouseCorrect(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidateAddress().IsHouseValid.GetValueOrDefault(true);
        }
        protected void CstFlatCorrect(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidateAddress().IsFlatValid.GetValueOrDefault(true);
        }
        protected void CstStreetCorrect(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidateAddress().IsStreetValid.GetValueOrDefault(true);
        }
        protected void CstAreaRegionServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidateAddress().IsRegionValid.GetValueOrDefault(true);
        }

        protected void CstAddressServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ddlAddresses.SelectedValue != "0";
        }
    }
}