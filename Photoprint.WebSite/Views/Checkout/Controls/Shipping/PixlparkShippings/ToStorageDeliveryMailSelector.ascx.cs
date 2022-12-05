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
    public partial class ToStorageMailShippingSelector : BaseShippingSelectorControl
    {
        protected Postal InitializedPostal { get; private set; }

        public override OrderAddress GetSelectedOrderAddress()
        {
            var address = GetSelectedShippingAddressByParse();
            if (address is null) return null;
            if (OrderAddress == null)
            {
                OrderAddress = new OrderAddress(address)
                {
                    FirstName = txtFirstName.Text,
                    LastName = txtLastName.Text,
                    MiddleName = UseMiddleName && !(chkAllowRegisterWithoutMiddleName?.Checked ?? false)
                        ? txtMiddleName.Text
                        : string.Empty,
                    Phone = !string.IsNullOrWhiteSpace(SmsService.ValidatePhone(txtPhone.Text))
                        ? SmsService.ValidatePhone(txtPhone.Text)
                        : txtPhone.Text,
                    Description = address.AddressName,
                };
            }
            else
            {
                OrderAddress.ShippingAddressId = address.Id;
                OrderAddress.Country = address.Country;
                OrderAddress.Region = address.Region;
                OrderAddress.City = address.City;
                OrderAddress.PostalCode = address.PostalCode;
                OrderAddress.AddressLine1 = address.AddressLine1;
                OrderAddress.Street = address.Street;
                OrderAddress.FirstName = txtFirstName.Text;
                OrderAddress.LastName = txtLastName.Text;
                OrderAddress.MiddleName = UseMiddleName && !(chkAllowRegisterWithoutMiddleName?.Checked ?? false) ? txtMiddleName.Text : string.Empty;
                OrderAddress.Phone = !string.IsNullOrWhiteSpace(SmsService.ValidatePhone(txtPhone.Text)) ? SmsService.ValidatePhone(txtPhone.Text) : txtPhone.Text;
                OrderAddress.Description = address.AddressName;
                OrderAddress.DeliveryProperties = address.DeliveryProperties;
                OrderAddress.Latitude = address.Latitude;
                OrderAddress.Longitude = address.Longitude;
            }

            return OrderAddress;
        }

        private ShippingAddress GetSelectedShippingAddressByParse(bool parseRequest = true)
        {
            if (!ValidateSelectableInput()) return null;

            var shippingAddresses = FrontendShippingService.GetAvailableShippingAddressList((Postal)Shipping);
            if (plhdAddresses.Visible)
            {
                var addressId = Request.Form[ddlAddresses.UniqueID];
                var ddlTarget = Request.Form["ddlTarget"];
                if (parseRequest && (addressId == "0" || ddlTarget == "ddlCities") && plhdCities.Visible)
                {
                    var sAddresses = (from a in shippingAddresses
                                      where a.Id == int.Parse(parseRequest && !string.IsNullOrWhiteSpace(Request.Form[ddlCities.UniqueID])
                                                                              ? Request.Form[ddlCities.UniqueID]
                                                                              : ddlCities.SelectedValue)
                                      select a);

                    if (sAddresses.Count() == 1)
                        addressId = sAddresses.First().Id.ToString(CultureInfo.InvariantCulture);
                }

                if (int.TryParse(parseRequest && !string.IsNullOrWhiteSpace(addressId) ? addressId : ddlAddresses.SelectedValue,
                    out var idToSearch))
                {
                    return (from a in shippingAddresses
                            where a.Id == idToSearch
                            select a).FirstOrDefault();
                }

                return new ShippingAddress();
            }

            return new ShippingAddress();
        }
        protected ToStorageMailShippingSelector()
        {
            Load += PageLoad;
        }
        private void PageLoad(object sender, EventArgs e)
        {
            ddlAddresses.PluginConfig.PlaceholderTextSingle = RM.GetString(RS.Shop.Checkout.StorageAddress);

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

            var userHost = SiteUtils.GetIpAddress(new HttpContextWrapper(Context).Request);
            var geoInfo = GeoIpService.GetInfo(CurrentPhotolab, CurrentLanguage, userHost);

            var currentCountry = InitCountries(postal, geoInfo);
            var currentRegion = InitRegions(postal, currentCountry, geoInfo);
            var currentCity = InitCities(postal, currentCountry, currentRegion, geoInfo);
            var currentAddress = InitAddresses(postal, currentCountry, currentRegion, currentCity);
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
                plhdCountries.Visible = true;

                var countryId = string.Empty;
                if (!string.IsNullOrWhiteSpace(Request.Form[ddlCountries.UniqueID]))
                    countryId = Request.Form[ddlCountries.UniqueID];

                var selectedCountry = countries.FirstOrDefault(c => c.Value == countryId);

                if (countries.Length > 1)
                    ddlCountries.Items.Add(new ListItem(RM.GetString("Shop.Checkout.SelectCountry", false), "0"));
                if (countries.Length == 1)
                    selectedCountry = countries.First();

                ddlCountries.Items.AddRange(countries);
                var lastOrder = PreviousOrders?.FirstOrDefault(x => x.ShippingId == postal.Id);
                if (selectedCountry == null && lastOrder != null)
                {
                    var address = lastOrder.DeliveryAddress;
                    selectedCountry = countries.FirstOrDefault(x => x.Text.Equals(address.Country));
                }
                if (selectedCountry == null && !string.IsNullOrEmpty(geoInfo?.Country))
                {
                    selectedCountry = countries.FirstOrDefault(x => x.Text.Equals(geoInfo.Country));
                }
                if (selectedCountry != null)
                {
                    ddlCountries.SelectedValue = selectedCountry.Value;
                    currentCountry = selectedCountry.Text;
                }
                else
                    currentCountry = RM.GetString("Shop.Checkout.SelectCountry", false);

                ddlCountries.DataBind();
            }
            else
            {
                plhdCountries.Visible = false;
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


            plhdRegions.Visible = false;

            var regions = shippingAddresses
                .Where(address => address.Country == currentCountry)
                .OrderBy(address => address.Position)
                .ThenBy(address => address.Region)
                .Select(address => new ListItem(address.Region, address.Id.ToString(CultureInfo.InvariantCulture)))
                .Where(a => !string.IsNullOrWhiteSpace(a.Text))
                .Distinct(new DistinctListItem())
                .ToArray();

            if (regions.Any() || currentCountry == RM.GetString("Shop.Checkout.SelectCountry", false))
            {
                ddlRegions.Items.Clear();
                plhdRegions.Visible = true;

                var selectedState = (from c in regions where c.Value == regionId select c).FirstOrDefault();

                if (currentCountry == RM.GetString("Shop.Checkout.SelectCountry", false) || regions.Length > 1)
                    ddlRegions.Items.Add(new ListItem(RM.GetString("Shop.Checkout.SelectRegion", false), "0"));
                if (regions.Length == 1)
                    selectedState = regions.First();

                ddlRegions.Items.AddRange(regions);
                var lastOrder = PreviousOrders?.FirstOrDefault(x => x.ShippingId == postal.Id);
                if (selectedState == null && lastOrder != null)
                {
                    var address = lastOrder.DeliveryAddress;
                    selectedState = regions.FirstOrDefault(x => x.Text.Equals(address.Region));
                }
                if (selectedState == null && !string.IsNullOrEmpty(geoInfo?.Region))
                {
                    selectedState = regions.FirstOrDefault(x => x.Text.Equals(geoInfo.Region));
                }
                if (selectedState != null)
                {
                    ddlRegions.SelectedValue = selectedState.Value;
                    currentRegion = selectedState.Text;
                }
                else
                    currentRegion = RM.GetString("Shop.Checkout.SelectRegion", false);

                ddlRegions.DataBind();
            }

            return currentRegion;
        }

        private string InitCities(Postal postal, string currentCountry, string currentRegion, GeoIpInfoDTO geoInfo)
        {
            var currentCity = string.Empty;
            var shippingAddresses = FrontendShippingService.GetAvailableShippingAddressList(postal);

            var cityId = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request.Form[ddlCities.UniqueID]))
                cityId = Request.Form[ddlCities.UniqueID];

            plhdCities.Visible = false;

            var cities = shippingAddresses
                .Where(address => address.Country == currentCountry && address.Region == currentRegion)
                .OrderBy(address => address.City)
                .ThenBy(address => address.Position) // устойчево, проверено
                .Select(address => new ListItem(address.City, address.Id.ToString(CultureInfo.InvariantCulture)))
                .Where(a => !string.IsNullOrWhiteSpace(a.Text))
                .Distinct(new DistinctListItem())
                .ToArray();

            if (cities.Any() || currentRegion == RM.GetString("Shop.Checkout.SelectRegion", false))
            {
                ddlCities.Items.Clear();
                plhdCities.Visible = true;

                var selectedCity = (from c in cities where c.Value == cityId select c).FirstOrDefault();

                if (currentRegion == RM.GetString("Shop.Checkout.SelectRegion", false) || cities.Length > 1)
                    ddlCities.Items.Add(new ListItem(RM.GetString("Shop.Checkout.SelectCity", false), "0"));
                if (cities.Length == 1)
                    selectedCity = cities.First();

                ddlCities.Items.AddRange(cities);
                var lastOrder = PreviousOrders?.FirstOrDefault(x => x.ShippingId == postal.Id);
                if (selectedCity == null && lastOrder != null)
                {
                    var address = lastOrder.DeliveryAddress;
                    selectedCity = cities.FirstOrDefault(x => x.Text.Equals(address.City));
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
                else
                    currentCity = RM.GetString("Shop.Checkout.SelectCity", false);

                ddlCities.DataBind();
            }

            return currentCity;
        }

        private string InitAddresses(Postal postal, string currentCountry, string currentRegion, string currentCity)
        {
            var currentAddress = string.Empty;
            var shippingAddresses = FrontendShippingService.GetAvailableShippingAddressList(postal).ToList();

            var addressId = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request.Form[ddlAddresses.UniqueID]))
            {
                addressId = Request.Form[ddlAddresses.UniqueID];
            }

            plhdAddresses.Visible = false;

            var addresses = shippingAddresses
                .Where(address => address.Country == currentCountry && address.Region == currentRegion && address.City == currentCity)
                .OrderBy(address => address.Position)
                .ThenBy(address => address.AddressName)
                .Select(address => new ListItem($@"{(!string.IsNullOrWhiteSpace(address.TitleLocalized[CurrentLanguage])
                    ? address.TitleLocalized[CurrentLanguage]
                    : address.AddressName)} ({address.AddressLine1})", address.Id.ToString(CultureInfo.InvariantCulture)))
                .Where(a => !string.IsNullOrWhiteSpace(a.Text))
                .Distinct(new DistinctListItem())
                .ToArray();

            if (addresses.Any() || currentCity == RM.GetString("Shop.Checkout.SelectCity", false))
            {
                ddlAddresses.Items.Clear();
                plhdAddresses.Visible = true;
                plhdDescription.Visible = false;

                var selectedAddress = (from c in addresses where c.Value == addressId select c).FirstOrDefault();

                //if (currentCity == RM.GetString("Shop.Checkout.SelectCity", false) || addresses.Length > 1)
                //	ddlAddresses.Items.Add(new ListItem(RM.GetString("Shop.Checkout.SelectStorage", false), "0"));
                if (addresses.Length == 1)
                    selectedAddress = addresses.First();

                ddlAddresses.Items.AddRange(addresses);
                var lastOrder = PreviousOrders?.FirstOrDefault(x => x.ShippingId == postal.Id);
                if (selectedAddress == null && lastOrder != null)
                {
                    var orderAddress = lastOrder.DeliveryAddress;
                    selectedAddress = addresses.FirstOrDefault(x => x.Text.Equals(orderAddress.Description + string.Format(" ({0})", orderAddress.AddressLine1)));
                }

                if (selectedAddress != null)
                {
                    ddlAddresses.SelectedValue = selectedAddress.Value;
                    currentAddress = selectedAddress.Text;

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

            if (currentAddress.IsEmpty() && ddlAddresses.Items.Count > 0 && ddlAddresses.Items[0] != null)
            {
                ddlAddresses.Items[0].Selected = true;
                currentAddress = ddlAddresses.Items[0].Text;
            }

            return currentAddress;
        }

        public override bool ValidateInput()
        {
            return ValidateSelectableInput()
                && reqFirstName.IsValid
                && reqLastName.IsValid
                && reqPhone.IsValid
                && (reqMiddleName?.IsValid ?? true)
                && (!plhdCountries.Visible || cstCountry.IsValid)
                && (!plhdRegions.Visible || cstRegion.IsValid)
                && (!plhdCities.Visible || cstCity.IsValid)
                && (!plhdAddresses.Visible || cstAddress.IsValid);
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

        public bool ValidateSelectableInput()
        {
            if (ddlCountries.Items.Count > 1 && ddlCountries.SelectedValue == "0")
                return false;

            if (ddlRegions.Items.Count > 1 && ddlRegions.SelectedValue == "0")
                return false;

            if (ddlCities.Items.Count > 1 && (string.IsNullOrEmpty(ddlCities.SelectedValue) || ddlCities.SelectedValue == "0"))
                return false;

            if (ddlAddresses.Items.Count > 1 && ddlAddresses.SelectedValue == "0")
                return false;

            if (plhdMaxWeight.Visible) return false;

            return true;
        }

        protected void cstCountry_ServerValidate(Object source, ServerValidateEventArgs args)
        {
            args.IsValid = ddlCountries.SelectedValue != "0";
        }

        protected void cstRegion_ServerValidate(Object source, ServerValidateEventArgs args)
        {
            args.IsValid = ddlRegions.SelectedValue != "0";
        }

        protected void cstCity_ServerValidate(Object source, ServerValidateEventArgs args)
        {
            args.IsValid = ddlCities.SelectedValue != "0";
        }

        protected void cstAddress_ServerValidate(Object source, ServerValidateEventArgs args)
        {
            args.IsValid = ddlAddresses.SelectedValue != "0";
        }
    }
}