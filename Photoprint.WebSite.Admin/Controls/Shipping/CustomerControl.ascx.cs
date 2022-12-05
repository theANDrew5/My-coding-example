using Photoprint.Core.Models;
using Photoprint.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Photoprint.WebSite.Admin.Controls
{
    public partial class CustomerControl : BaseControl
    {
        private IPhotolabSettingsService PhotolabSettingsService => WebSiteGlobal.Container.GetInstance<IPhotolabSettingsService>();
        private DeliveryWindowSettings DeliverySettings => PhotolabSettingsService.GetSettings<DeliveryWindowSettings>(CurrentFrontend);

        protected bool MiddleNameRequired {get; private set;}

        protected bool ShowPhone =>
            LoggedInUser.GetTeamInfoByCompanyId(CurrentCompanyAccount.Id)?.Properties.ShowSensetiveClientData ?? true;
        protected bool AdditionalPhoneEnabled => DeliverySettings?.IsAdditionalNotificationPhoneNumberEnabled ?? false;

        public void InitControl(Shipping shipping, Order order)
        {
            var address = order.DeliveryAddress;
            if (address != null)
            {
                txtFirstName.Text = address.FirstName;
                txtLastName.Text = address.LastName;
                txtMiddleName.Text = address.MiddleName;

                txtPhone.Text = address.Phone;
                txtAdditionalPhone.Text = order.Properties.AdditionalNotificationPhone ?? "";
            }
            switch (shipping)
            {
                case Postal postal:
                    MiddleNameRequired = postal.ShippingServiceProviderType == ShippingServiceProviderType.NovaposhtaV2 ||
                        DeliverySettings is null ?  postal.UseMiddleName : DeliverySettings.UseMiddleName;
                    break;
                default:
                    MiddleNameRequired = DeliverySettings?.UseMiddleName ?? shipping.UseMiddleName;
                    break;
            }


            reqMiddleName.Enabled = MiddleNameRequired;
            plhdMiddleName.Visible = MiddleNameRequired;
        }

        public void GetCustomerData(Order order)
        {
            var newAddress = order.DeliveryAddress;

            newAddress.FirstName = txtFirstName.Text;
            newAddress.LastName = txtLastName.Text;
            newAddress.MiddleName = MiddleNameRequired ? txtMiddleName.Text : string.Empty;
            newAddress.Phone = txtPhone.Text;

            order.Properties.AdditionalNotificationPhone = txtAdditionalPhone.Text;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    }
}