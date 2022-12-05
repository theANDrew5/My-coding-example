using System;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using Photoprint.Core;
using Photoprint.Core.Models;

namespace Photoprint.WebSite.Controls
{
    public partial class PickpointDeliverySelector : BaseShippingSelectorControl
    {
        protected Postal CurrentPostal { get; set; }
        protected PickpointServiceProviderSettings Settings => CurrentPostal.ServiceProviderSettings as PickpointServiceProviderSettings;

        protected void Page_Load(object sender, EventArgs e) 
        {
            txtFirstName.Text = string.IsNullOrWhiteSpace(txtFirstName.Text) ? LoggedInUser.FirstName : txtFirstName.Text;
            txtLastName.Text = string.IsNullOrWhiteSpace(txtLastName.Text) ? LoggedInUser.LastName : txtLastName.Text;
            txtPhone.Text = string.IsNullOrWhiteSpace(txtPhone.Text) ? LoggedInUser.GetCleanPhone() : txtPhone.Text;
            txtAddress.Text = txtAddress.Text;

            if (UseMiddleName)
                txtMiddleName.Text = string.IsNullOrWhiteSpace(txtMiddleName.Text) ? LoggedInUser.MiddleName : txtMiddleName.Text;
            else
            {
                regMiddleName.Enabled = false;
                reqMiddleName.Enabled = false;
            }
        }

        public override OrderAddress GetSelectedOrderAddress()
        {
            var result = Request.Form["pickpointWidgetResult"];
            if (string.IsNullOrWhiteSpace(result)) return null;

            var resultObj = JObject.Parse(result);
            if (resultObj == null) return null;

            OrderAddress = new OrderAddress
            {
                Country = resultObj["country"]?.Value<string>() ?? string.Empty,
                City = resultObj["cityname"]?.Value<string>() ?? string.Empty,
                AddressLine1 = resultObj["address"]?.Value<string>() ?? string.Empty,
                Region = resultObj["region"]?.Value<string>() ?? string.Empty,
                Description = resultObj["name"]?.Value<string>() ?? string.Empty,
                House = resultObj["house"]?.Value<string>() ?? string.Empty,
                Latitude = resultObj["latitude"]?.Value<string>() ?? string.Empty,
                Longitude = resultObj["longitude"]?.Value<string>() ?? string.Empty,
                PostalCode = resultObj["postcode"]?.Value<string>() ?? string.Empty,
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim(),
                MiddleName = UseMiddleName ? txtMiddleName.Text.Trim() : string.Empty,
                Phone = !string.IsNullOrWhiteSpace(SmsService.ValidatePhone(txtPhone.Text)) ? SmsService.ValidatePhone(txtPhone.Text) : txtPhone.Text.Trim(),
                DeliveryProperties = new DeliveryAddressProperties
                {
                    PickpointAddressInfo = new PickpointAddressInfo
                    {
                        PostamatCode = resultObj["id"]?.Value<string>() ?? string.Empty,
                        CityId = resultObj["cityid"]?.Value<string>() ?? string.Empty,
                        PostamatId = resultObj["dbid"]?.Value<string>() ?? string.Empty
                    }
                }                
            };

            return OrderAddress;
            // MaxWeight = resultObj["maxweight"] != null ? resultObj["maxweight"]?.Value<double?>() : null
        }

        protected override void InitSelectedShipping(Shipping shipping)
        {
            if (shipping == null) throw new ArgumentNullException(nameof(shipping));

            if (shipping is Postal postal)
                CurrentPostal = postal;
            else
                throw new PhotoprintSystemException("Shipping must be postal", string.Empty);
        }

        public override bool ValidateInput() => !string.IsNullOrWhiteSpace(Request.Form["pickpointWidgetResult"]);

        protected void CstNameCorrect(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidateAddress().IsNameValid.GetValueOrDefault(true);
        }
        protected void CstPhoneCorrect(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ValidateAddress().IsPhoneValid.GetValueOrDefault(true);
        }
        private AddressValidationResult _addressValidationResult;
        protected AddressValidationResult ValidateAddress()
        {
            var orderAddress = GetSelectedOrderAddress();
            if (_addressValidationResult == null)
            {
                var provider = ShippingProviderResolverService.GetProvider((Postal)Shipping);
                _addressValidationResult = provider?.TestDelivery(Shipping.ServiceProviderSettings, orderAddress, CurrentPostal) ?? AddressValidationResult.GetValidState();
            }
            return _addressValidationResult;
        }
    }
}