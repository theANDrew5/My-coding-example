using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Photoprint.Core;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using Photoprint.WebSite.Shared;

namespace Photoprint.WebSite.Controls
{
    public partial class UkrposhtaDeliverySelector : BaseShippingSelectorControl
    {
        private static readonly IUkrposhtaServices _ukrposhtaServices = Container.GetInstance<IUkrposhtaServices>();

        protected Postal InitializedPostal { get; private set; }

        public override OrderAddress GetSelectedOrderAddress()
        {
            if (OrderAddress != null) return OrderAddress;

            OrderAddress = new OrderAddress
            {
                Country = "Україна",
                Region = ddlStates.SelectedItem.Text,
                District = ddlRegions.SelectedItem.Text,
                City = ddlCities.SelectedItem.Text,
                FirstName = txtFirstName.Text,
                LastName = txtLastName.Text,
                MiddleName = !(chkAllowRegisterWithoutMiddleName?.Checked ?? false) ? txtMiddleName.Text : string.Empty,
                Phone = !string.IsNullOrWhiteSpace(SmsService.ValidatePhone(txtPhone.Text)) ? SmsService.ValidatePhone(txtPhone.Text) : txtPhone.Text,
                DeliveryProperties = new DeliveryAddressProperties
                {
                    UkrposhtaAddressInfo = new UkrposhtaAddressInfo
                    {
                        StateId = ddlStates.SelectedValue,
                        DistrictId = ddlRegions.SelectedValue,
                        CityId = ddlCities.SelectedValue
                    }
                }
            };

            if (InitializedPostal.PostalType == PostalType.ToStorageDelivery)
            {
                if (ddlAddresses.SelectedValue != "0")
                {
                    var storageInfo = ddlAddresses.SelectedItem.Text.Split('(');
                    OrderAddress.Description = storageInfo[0].Trim();
                    OrderAddress.AddressLine1 = storageInfo[1].Trim();
                }
                OrderAddress.PostalCode = ddlAddresses.SelectedValue;
            }
            else
            {
                OrderAddress.PostalCode = ddlHouses.SelectedValue;  //txtPostalCode.Text;
                OrderAddress.Street = ddlStreets.SelectedItem.Text;
                OrderAddress.House = ddlHouses.SelectedItem.Text;
                OrderAddress.Flat = txtAddressFlat.Text;
                OrderAddress.DeliveryProperties.UkrposhtaAddressInfo.StreetId = ddlStreets.SelectedValue;
            }

            return OrderAddress;
        }

        protected UkrposhtaDeliverySelector()
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

            ddlStates.Style["Float"] = "none";
            ddlRegions.Style["Float"] = "none";
            ddlCities.Style["Float"] = "none";
            ddlStreets.Style["Float"] = "none";
            ddlAddresses.Style["Float"] = "none";
            ddlHouses.Style["Float"] = "none";
        }

        protected override void InitSelectedShipping(Shipping shipping)
        {
            InitializedPostal = shipping as Postal;

            var userHost = SiteUtils.GetIpAddress(new HttpContextWrapper(Context).Request);
            var geoInfo = GeoIpService.GetInfo(CurrentPhotolab, CurrentLanguage, userHost);

            InitStates(geoInfo);
            InitRegions(ddlStates.SelectedValue);
            InitCities(ddlStates.SelectedValue, ddlRegions.SelectedValue, geoInfo);

            if (InitializedPostal.PostalType == PostalType.ToClientDelivery)
            {
                plhdClientShipping.Visible = true;

                InitStreets(ddlStates.SelectedValue, ddlRegions.SelectedValue, ddlCities.SelectedValue);
                InitHouses(ddlStreets.SelectedValue);

                //txtPostalCode.Text = ddlHouses.SelectedValue;
            }
            else
            {
                plhdStorageShipping.Visible = true;
                InitAddresses(ddlStates.SelectedValue, ddlRegions.SelectedValue, ddlCities.SelectedValue);
                if (ddlAddresses.Items.Count == 0)
                    ddlAddresses.Items.Add(new ListItem(RM.GetString(RS.Shop.Checkout.EmptyStorages, false), "0"));
            }
        }

        private void InitStates(GeoIpInfoDTO geoInfo)
        {
            ddlStates.Items.Clear();

            var states = _ukrposhtaServices.GetStates();
            ddlStates.Items.AddRange(states.Select(item => new ListItem(item.Name, item.Id)).ToArray());

            var stateId = !string.IsNullOrWhiteSpace(Request.Form[ddlStates.UniqueID]) ? Request.Form[ddlStates.UniqueID] : string.Empty;

            var selectedState = states.FirstOrDefault(state => state.Id == stateId) ?? states.FirstOrDefault(state => state.Name == geoInfo?.Region);

            ddlStates.SelectedValue = selectedState != null ? selectedState.Id : "0";
        }

        private void InitRegions(string currentStateId)
        {
            ddlRegions.Items.Clear();

            var regions = _ukrposhtaServices.GetRegions(currentStateId);
            ddlRegions.Items.AddRange(regions.Select(item => new ListItem(item.Name, item.Id)).ToArray());

            var regionId = !string.IsNullOrWhiteSpace(Request.Form[ddlRegions.UniqueID]) ? Request.Form[ddlRegions.UniqueID] : string.Empty;
            var selectedRegion = regions.FirstOrDefault(region => region.Id == regionId);
            ddlRegions.SelectedValue = selectedRegion != null ? selectedRegion.Id : "0";
        }

        private void InitCities(string currentStateId, string currentRegionId, GeoIpInfoDTO geoInfo)
        {
            ddlCities.Items.Clear();

            var cities = _ukrposhtaServices.GetCities(currentStateId, currentRegionId);
            ddlCities.Items.AddRange(cities.Select(item => new ListItem(item.Name, item.Id)).ToArray());

            var cityId = !string.IsNullOrWhiteSpace(Request.Form[ddlCities.UniqueID]) ? Request.Form[ddlCities.UniqueID] : string.Empty;
            var selectedCity = cities.FirstOrDefault(state => state.Id == cityId) ?? cities.FirstOrDefault(state => state.Name == geoInfo?.City);
            ddlCities.SelectedValue = selectedCity != null ? selectedCity.Id : "0";
        }

        private void InitStreets(string currentStateId, string currentRegionId, string currentCityId)
        {
            ddlStreets.Items.Clear();

            var streets = _ukrposhtaServices.GetStreets(currentStateId, currentRegionId, currentCityId);
            ddlStreets.Items.AddRange(streets.Select(item => new ListItem(item.Name, item.Id)).ToArray());

            var streetId = !string.IsNullOrWhiteSpace(Request.Form[ddlStreets.UniqueID]) ? Request.Form[ddlStreets.UniqueID] : string.Empty;
            var selectedStreet = streets.FirstOrDefault(street => street.Id == streetId);
            ddlStreets.SelectedValue = selectedStreet != null ? selectedStreet.Id : "0";
        }

        private void InitAddresses(string currentStateId, string currentRegionId, string currentCityId)
        {
            ddlAddresses.Items.Clear();

            var addresses = _ukrposhtaServices.GetPostoffices(currentStateId, currentRegionId, currentCityId);
            ddlAddresses.Items.AddRange(addresses.Select(item => new ListItem(item.Name, item.Id)).ToArray());

            var addressId = !string.IsNullOrWhiteSpace(Request.Form[ddlAddresses.UniqueID]) ? Request.Form[ddlAddresses.UniqueID] : string.Empty;
            var selectedAddress = addresses.FirstOrDefault(address => address.Id == addressId);
            ddlAddresses.SelectedValue = selectedAddress != null ? selectedAddress.Id : "0";
        }

        private void InitHouses(string currentStreetId)
        {
            ddlHouses.Items.Clear();

            var houses = _ukrposhtaServices.GetHouses(currentStreetId);
            ddlHouses.Items.AddRange(houses.Select(item => new ListItem(item.Name, item.Id)).ToArray());

            var houseId = !string.IsNullOrWhiteSpace(Request.Form[ddlHouses.UniqueID]) ? Request.Form[ddlHouses.UniqueID] : string.Empty;
            var selectedHouse = houses.FirstOrDefault(house => house.Id == houseId);
            ddlHouses.SelectedValue = selectedHouse != null ? selectedHouse.Id : "0";
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
            //if (!((Postal)Shipping).IsIndexRequired) reqIndex.Enabled = false;

            var isValid = ValidateSelectableInput() && reqFirstName.IsValid && reqLastName.IsValid && reqMiddleName.IsValid && reqPhone.IsValid &&
                          cstState.IsValid && cstRegion.IsValid && cstCity.IsValid /*&& (!reqIndex.Enabled || reqIndex.IsValid)*/ &&
                          (!plhdClientShipping.Visible || cstStreet.IsValid && reqAddressFlat.IsValid) && (!plhdStorageShipping.Visible || cstAddress.IsValid);
            var shippingValidationResult = ValidateAddress();
            SetLocalizedCustomErrorString(shippingValidationResult.Message);
            return isValid && shippingValidationResult.IsValid;
        }

        private bool ValidateSelectableInput()
        {
            if (ddlStates.Items.Count > 1 && ddlStates.SelectedValue == "0")
                return false;

            if (ddlRegions.Items.Count > 1 && ddlRegions.SelectedValue == "0")
                return false;

            if (ddlCities.Items.Count > 1 && ddlCities.SelectedValue == "0")
                return false;

            if (ddlStreets.Items.Count > 1 && ddlStreets.SelectedValue == "0")
                return false;

            if (ddlAddresses.Items.Count > 1 && ddlAddresses.SelectedValue == "0")
                return false;

            if (ddlHouses.Items.Count > 1 && ddlHouses.SelectedValue == "0")
                return false;

            return !plhdMaxWeight.Visible;
        }

        protected void CstStateServerValidate(object source, ServerValidateEventArgs args) => args.IsValid = ddlStates.SelectedValue != "0";
        protected void CstAreaRegionServerValidate(object source, ServerValidateEventArgs args) => args.IsValid = ValidateAddress().IsRegionValid.GetValueOrDefault(true);
        protected void CstCityServerValidate(object source, ServerValidateEventArgs args) => args.IsValid = ddlCities.SelectedValue != "0";
        protected void CstStreetServerValidate(object source, ServerValidateEventArgs args) => args.IsValid = ddlStreets.SelectedValue != "0";
        protected void CstHouseServerCorrect(object source, ServerValidateEventArgs args) => args.IsValid = ddlHouses.SelectedValue != "0";
        protected void CstAddressServerValidate(object source, ServerValidateEventArgs args) => args.IsValid = ddlAddresses.SelectedValue != "0";
        protected void CstNameCorrect(object source, ServerValidateEventArgs args) => args.IsValid = ValidateAddress().IsNameValid.GetValueOrDefault(true);
        protected void CstPhoneCorrect(object source, ServerValidateEventArgs args) => args.IsValid = ValidateAddress().IsPhoneValid.GetValueOrDefault(true);
        protected void CstFlatCorrect(object source, ServerValidateEventArgs args) => args.IsValid = ValidateAddress().IsFlatValid.GetValueOrDefault(true);
    }
}