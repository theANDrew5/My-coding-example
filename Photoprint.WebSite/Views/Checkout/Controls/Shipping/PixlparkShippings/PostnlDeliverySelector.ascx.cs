using Newtonsoft.Json;
using Photoprint.Core.Models;
using System;
using System.Web.UI.WebControls;

namespace Photoprint.WebSite.Controls
{
    public partial class PostnlDeliverySelector : BaseShippingSelectorControl
    {
        protected Postal InitializedPostal { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            regFirstName.Enabled = CurrentPhotolab.Properties.IsCyrillicNamesEnabled;
            regLastName.Enabled = CurrentPhotolab.Properties.IsCyrillicNamesEnabled;
            regMiddleName.Enabled = UseMiddleName && CurrentPhotolab.Properties.IsCyrillicNamesEnabled;
            reqMiddleName.Enabled = UseMiddleName;

            plhdRegion.Visible = InitializedPostal.IsRegionRequired;
            reqRegion.Enabled = InitializedPostal.IsRegionRequired;

            plhdClient.Visible = InitializedPostal.PostalType == PostalType.ToClientDelivery;
            plhdStorage.Visible = InitializedPostal.PostalType == PostalType.ToStorageDelivery;

            if (!IsPostBack)
            {
                txtFirstName.Text = LoggedInUser.FirstName;
                if(UseMiddleName) txtMiddleName.Text = string.IsNullOrWhiteSpace(txtMiddleName.Text) ? LoggedInUser.MiddleName : txtMiddleName.Text;
                txtLastName.Text = LoggedInUser.LastName;
                txtPhone.Text = LoggedInUser.GetCleanPhone();
            }
        }

        public override OrderAddress GetSelectedOrderAddress()
        {
            var addressString = Request.Form["address"];
            var result = new OrderAddress();
            
            if (!string.IsNullOrWhiteSpace(addressString))
            {
                result = JsonConvert.DeserializeObject<OrderAddress>(addressString);
                result.FirstName = txtFirstName.Text;
                result.MiddleName = UseMiddleName ? txtMiddleName.Text : string.Empty;
                result.LastName = txtLastName.Text;
                result.Phone = txtPhone.Text;

                if (!string.IsNullOrWhiteSpace(Request.Form["storage"]))
                {
                    result.DeliveryProperties.PostnlAddressInfo = new PostnlAddressInfo { LocationCode = int.Parse(Request.Form["storage"]) };
                }
            }

            OrderAddress = result;
            return OrderAddress;
        }

        protected override void InitSelectedShipping(Shipping shipping)
        {
            InitializedPostal = shipping as Postal;
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

        public override bool ValidateInput() => !string.IsNullOrWhiteSpace(Request.Form["address"]);

        protected void CstNameCorrect(object source, ServerValidateEventArgs args) => args.IsValid = ValidateAddress().IsNameValid.GetValueOrDefault(true);
        protected void CstPhoneCorrect(object source, ServerValidateEventArgs args) => args.IsValid = ValidateAddress().IsPhoneValid.GetValueOrDefault(true);
    }
}