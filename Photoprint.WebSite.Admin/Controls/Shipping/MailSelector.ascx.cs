using Photoprint.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Photoprint.Core.Services;

namespace Photoprint.WebSite.Admin.Controls
{
    public partial class MailShippingSelector : BaseShippingSelectorControl
    {
        protected Postal SelectedPostal { get; set; }
        protected ShippingServiceProviderType PostalProviderType => SelectedPostal.ShippingServiceProviderType;

        protected bool UseAddressLines => SelectedPostal.PostalType != PostalType.ToStorageDelivery 
                                          && (DeliverySettings?.AddressSelectSettings?.UseAddressLines ?? SelectedPostal.IsMultipleAddressLines);


        private bool IsSameShipping => CurrentOrder?.Shipping?.Id == SelectedPostal.Id;

        protected MailShippingSelector()
        {
            Load += PageLoad;
        }

        private void PageLoad(object sender, EventArgs e)
        {
            if(CurrentOrder is null || SelectedPostal is null) return;

            var shippingAddresses = FrontendShippingService.GetAvailableShippingAddressList(SelectedPostal);

            var initAddress = IsSameShipping ? CurrentOrder.DeliveryAddress : null;

            var selectedCountry = InitCountries(initAddress, shippingAddresses);
            var selectedRegion = InitRegions(initAddress, selectedCountry, shippingAddresses);
            var selectedCity = InitCities(initAddress, selectedCountry, selectedRegion, shippingAddresses);
            var selectedAddress = InitAddresses(initAddress, selectedCountry, selectedRegion, selectedCity, shippingAddresses);

            txtPostalCode.Text = !string.IsNullOrWhiteSpace(selectedAddress?.PostalCode) ? selectedAddress.PostalCode : txtPostalCode.Text;
            
            if (!IsPostBack)
            {
                initAddress ??= new OrderAddress();

                txtPostalCode.Text = initAddress.PostalCode;
                txtAddressLine1.Text = initAddress.AddressLine1;
                txtAddressLine2.Text = initAddress.AddressLine2;

                txtAddressStreet.Text = initAddress.Street;
                txtAddressHouse.Text = initAddress.House;
                txtAddressFlat.Text = initAddress.Flat;

                txtPostalCode.Text = initAddress?.PostalCode ?? string.Empty;
            }

            var usePostCode = DeliverySettings?.AddressSelectSettings?.UsePostcode ?? SelectedPostal.IsIndexRequired;
            plhdPostalCode.Visible = usePostCode;
            reqPostalCode.Enabled = usePostCode;
        }

        public override void InitControl(Shipping shipping, Order order, IReadOnlyCollection<IPurchasedItem> items)
        {
            if (!(shipping is Postal postal)) throw new ArgumentException(nameof(postal));
            CurrentOrder = order ?? throw new ArgumentNullException(nameof(order));
            SelectedPostal = postal;
        }

        public override OrderAddress GeOrderAddress()
        {
            OrderAddress newAddress = null;

            if (SelectedPostal.PostalType == PostalType.ToClientDelivery)
            {
                newAddress = CurrentOrder.DeliveryAddress;

                if (SelectedPostal.IsMultipleAddressLines)
                {
                    newAddress.AddressLine1 = txtAddressLine1.Text;
                    newAddress.AddressLine2 = txtAddressLine2.Text;
                }
                else
                {
                    newAddress.Street = txtAddressStreet.Text;
                    newAddress.House = txtAddressHouse.Text;
                    newAddress.Flat = txtAddressFlat.Text;
                    newAddress.AddressLine1 = string.Empty;
                    newAddress.AddressLine2 = string.Empty;
                }

                newAddress.City = !string.IsNullOrWhiteSpace(txtCity.Text) ? txtCity.Text : ddlCities?.SelectedItem?.Text;
                newAddress.Region = !string.IsNullOrWhiteSpace(txtRegion.Text) ?  txtRegion.Text : ddlRegions?.SelectedItem?.Text;
                newAddress.Country = !string.IsNullOrWhiteSpace(txtCountry.Text) ? txtCountry.Text : ddlCountries?.SelectedItem?.Text;
                newAddress.PostalCode = SelectedPostal.IsIndexRequired ? txtPostalCode.Text : string.Empty;
            }
            else
            {
                var selectedAddressIdStr = ddlTarget.Value switch
                {
                    "ddlAddresses" => ddlAddresses.SelectedValue,
                    "ddlCities" => ddlCities.SelectedValue,
                    "ddlRegions" => ddlRegions.SelectedValue,
                    "ddlCountries" => ddlCountries.SelectedValue,
                    _ => string.Empty
                };

                if (!int.TryParse(selectedAddressIdStr, out var selectedAddressId)) return IsSameShipping ? CurrentOrder.DeliveryAddress : null;

                var selectedAddress = FrontendShippingService.GetShippingAddressList(SelectedPostal).FirstOrDefault(sa => sa.Id == selectedAddressId);

                if (selectedAddress == null) return null;
                newAddress = new OrderAddress(selectedAddress)
                {
                    PostalCode = SelectedPostal.IsIndexRequired ? txtPostalCode.Text : string.Empty,
                };

                if (ddlTarget.Value != "ddlAddresses")
                {
                    if (SelectedPostal.IsMultipleAddressLines)
                    {
                        newAddress.AddressLine1 = txtAddressLine1.Text;
                        newAddress.AddressLine2 = txtAddressLine2.Text;
                    }
                    else
                    {
                        newAddress.Street = txtAddressStreet.Text;
                        newAddress.House = txtAddressHouse.Text;
                        newAddress.Flat = txtAddressFlat.Text;
                    }

                    switch (ddlTarget.Value)
                    {
                        case "ddlAddresses":
                            break;
                        case "ddlRegions":
                            newAddress.City = txtCity.Text;
                            break;
                        case "ddlCountries":
                            newAddress.City = txtCity.Text;
                            newAddress.Region = txtRegion.Text;
                            break;
                    }
                }
            }
            return newAddress;
        }

        private string InitCountries(OrderAddress oldAddress, IReadOnlyCollection<ShippingAddress> shippingAddresses)
        {
            var countries = shippingAddresses
                .OrderBy(address => address.Position)
                .ThenBy(address => address.Country)
                .Select(address => new ListItem(address.Country, address.Id.ToString(CultureInfo.InvariantCulture)))
                .Where(a => !string.IsNullOrWhiteSpace(a.Text))
                .Distinct(new DistinctListItem())
                .ToArray();

            var selectedCountry = string.Empty;

            if (countries.Length > 0)
            {
                var selectedCountryItem = countries.FirstOrDefault(c => c.Value == Request.Form[ddlCountries.UniqueID]);

                ddlCountries.Items.Clear();

                ddlCountries.Items.Add(new ListItem(RM.GetString(RS.Shipping.Address.SelectCountry), "0"));
                ddlCountries.Items.AddRange(countries);

                var selectedItem = selectedCountryItem ?? (oldAddress != null
                    ? countries.FirstOrDefault(li => li.Text.Equals(oldAddress.Country))
                    : null);

                if (selectedItem != null)
                {
                    ddlCountries.SelectedValue = selectedItem.Value;
                    selectedCountry = selectedItem.Text;
                }

                ddlCountries.DataBind();
            }
            else
            {
                txtCountry.Text = oldAddress?.Country;
            }

            plhdCountrySelector.Visible = countries.Length > 0;
            plhdCountryInput.Visible = countries.Length == 0;

            return selectedCountry;
        }

        private string InitRegions(OrderAddress oldAddress, string selectedCountry, IReadOnlyCollection<ShippingAddress> shippingAddresses)
        {
            var regions = shippingAddresses
                .Where(address => address.Country == selectedCountry)
                .OrderBy(address => address.Position)
                .ThenBy(address => address.Region)
                .Select(address => new ListItem(address.Region, address.Id.ToString(CultureInfo.InvariantCulture)))
                .Where(a => !string.IsNullOrWhiteSpace(a.Text))
                .Distinct(new DistinctListItem())
                .ToArray();

            var selectedRegion = string.Empty;

            if (regions.Length > 0)
            {
                var selectedRegionItem = regions.FirstOrDefault(c => c.Value == Request.Form[ddlRegions.UniqueID]);

                ddlRegions.Items.Clear();

                plhdRegionSelector.Visible = true;
                plhdRegionInput.Visible = false;

                ddlRegions.Items.Add(new ListItem(RM.GetString(RS.Shipping.Address.SelectRegion), "0"));
                ddlRegions.Items.AddRange(regions);

                var selectedItem = selectedRegionItem ?? (oldAddress != null
                    ? regions.FirstOrDefault(li => li.Text.Equals(oldAddress.Region))
                    : null);

                if (selectedItem != null)
                {
                    ddlRegions.SelectedValue = selectedItem.Value;
                    selectedRegion = selectedItem.Text;
                }

                ddlRegions.DataBind();
            }
            else if(!Page.IsPostBack)
            {
                txtRegion.Text = oldAddress?.Region;
            }

            plhdRegionSelector.Visible = regions.Length > 0;
            plhdRegionInput.Visible = regions.Length == 0;

            return selectedRegion;
        }

        private string InitCities(OrderAddress oldAddress, string selectedCountry, string selectedRegion, IReadOnlyCollection<ShippingAddress> shippingAddresses)
        {
            var cities = shippingAddresses
                .Where(address => address.Country == selectedCountry && address.Region == selectedRegion)
                .OrderBy(address => address.City)
                .ThenBy(address => address.Position) // устойчево, проверено
                .Select(address => new ListItem(address.City, address.Id.ToString(CultureInfo.InvariantCulture)))
                .Where(a => !string.IsNullOrWhiteSpace(a.Text))
                .Distinct(new DistinctListItem())
                .ToArray();

            var selectedCity = string.Empty;

            if (cities.Length > 0)
            {
                var selectedCityItem = cities.FirstOrDefault(c => c.Value == Request.Form[ddlCities.UniqueID]);
                ddlCities.Items.Clear();

                plhdCitySelector.Visible = true;
                plhdCityInput.Visible = false;

                ddlCities.Items.Add(new ListItem(RM.GetString(RS.Shipping.Address.SelectCity), "0"));
                ddlCities.Items.AddRange(cities);

                var selectedItem = selectedCityItem ?? (oldAddress != null
                    ? cities.FirstOrDefault(li => li.Text.Equals(oldAddress.City)) : null);

                if (selectedItem != null)
                {
                    ddlCities.SelectedValue = selectedItem.Value;
                    selectedCity = selectedItem?.Text;
                }
                ddlCities.DataBind();
            }
            else if(!Page.IsPostBack)
            {
                txtCity.Text = oldAddress?.City;
            }

            plhdCitySelector.Visible = cities.Length > 0;
            plhdCityInput.Visible = cities.Length == 0;

            return selectedCity;
        }

        private ShippingAddress InitAddresses(OrderAddress oldAddress, string selectedCountry, string selectedRegion,
            string selectedCity, IReadOnlyCollection<ShippingAddress> shippingAddresses)
        {
            var addresses = shippingAddresses
                .Where(address => address.Country == selectedCountry && address.Region == selectedRegion && address.City == selectedCity)
                .OrderBy(address => address.Position)
                .ThenBy(address => address.AddressName)
                .Select(address => new ListItem(!string.IsNullOrWhiteSpace(address.AddressName) && !string.IsNullOrWhiteSpace(address.AddressLine1)
                    ? $"{address.AddressName} ({address.AddressLine1})" : string.Empty, 
                    address.Id.ToString(CultureInfo.InvariantCulture)))
                .Where(a => !string.IsNullOrWhiteSpace(a.Text))
                .Distinct(new DistinctListItem())
                .ToArray();
            ListItem selectedItem = null;
            if (addresses.Length > 0)
            {
                ddlAddresses.Items.Clear();

                ddlAddresses.Items.Add(new ListItem(RM.GetString(RS.Shipping.Address.SelectPoint), "0"));
                ddlAddresses.Items.AddRange(addresses);

                selectedItem = addresses.FirstOrDefault(c => c.Value == Request.Form[ddlAddresses.UniqueID])
                    ?? (oldAddress != null
                        ? addresses.FirstOrDefault(li => Regex.IsMatch(li.Text, $@".* \({oldAddress.AddressLine1}\)")) 
                        : null);

                if (selectedItem != null)
                    ddlAddresses.SelectedValue = selectedItem.Value;
                ddlAddresses.DataBind();
                plhdSelectorAddresses.Visible = SelectedPostal.PostalType == PostalType.ToStorageDelivery;
                plhdManualAddress.Visible = SelectedPostal.PostalType == PostalType.ToClientDelivery;
            }
            else 
            {
                plhdSelectorAddresses.Visible = false;
                plhdManualAddress.Visible = true;

                if (!Page.IsPostBack)
                {
                    txtAddressStreet.Text = oldAddress?.Street;
                    txtAddressHouse.Text = oldAddress?.House;
                    txtAddressFlat.Text = oldAddress?.Flat;

                    txtAddressLine1.Text = oldAddress?.AddressLine1;
                    txtAddressLine2.Text = oldAddress?.AddressLine2;
                }
            }


            return shippingAddresses.FirstOrDefault(sa => sa.Id == int.Parse(selectedItem?.Value ?? "0"));

        }

        public override bool ValidateInput() => 
            (!reqPostalCode.Enabled || reqPostalCode.IsValid)
            && ValidateSelectableInput()
            && ValidateAddress();

        private bool ValidateSelectableInput()
        {
            if (ddlCountries.Items.Count > 1 && ddlCountries.SelectedValue == "0")
                return false;

            if (ddlRegions.Items.Count > 1 && ddlRegions.SelectedValue == "0")
                return false;

            if (ddlCities.Items.Count > 1 && ddlCities.SelectedValue == "0")
                return false;

            return true;
        }

        private bool ValidateAddress()
        {
            if (plhdSelectorAddresses.Visible) return ddlAddresses.SelectedValue != "0";

            return SelectedPostal.IsMultipleAddressLines 
                ? !(string.IsNullOrWhiteSpace(txtAddressLine1.Text) || string.IsNullOrWhiteSpace(txtAddressLine2.Text)) 
                : !(string.IsNullOrWhiteSpace(txtAddressHouse.Text) || string.IsNullOrWhiteSpace(txtAddressStreet.Text));
        }

        private class DistinctListItem : IEqualityComparer<ListItem>
        {
            public bool Equals(ListItem x, ListItem y)
            {
                if (x == null || y == null)
                    return false;
                return x.Text == y.Text;
            }

            public int GetHashCode(ListItem address) => address.Text.GetHashCode();
        }

    }
}