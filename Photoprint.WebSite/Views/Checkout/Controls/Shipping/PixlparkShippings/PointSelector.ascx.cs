using System;
using Photoprint.Core.Configuration;
using Photoprint.Core.Models;

namespace Photoprint.WebSite.Controls
{
    public partial class DistributionPointSelector : BaseShippingSelectorControl
    {
		protected DistributionPoint InitializedPoint { get; private set; }

		private bool _isVisibleTitle = true;
    	public bool IsVisibleTitle
    	{
            get => _isVisibleTitle;
            set => _isVisibleTitle = value;
        }

		private bool _isVisibleRecipientFields = true;
		public bool IsVisibleRecipientFields
		{
            get => _isVisibleRecipientFields;
            set => _isVisibleRecipientFields = value;
        }
		
		public override OrderAddress GetSelectedOrderAddress()
		{
            var middleName = UseMiddleName ? txtMiddleName.Text : string.Empty;
            OrderAddress = new OrderAddress(Shipping.Address, txtFirstName.Text, txtLastName.Text, SmsService.ValidatePhone(txtPhone.Text), middleName);
            AdditionalEmail = UseAdditionalEmail ? txtShippingEmail.Text : string.Empty;

			return OrderAddress;
		}

    	protected DistributionPoint SelectedDistributionPoint => Shipping as DistributionPoint;
        protected static string GMapKey => Settings.GMapAdminKey;

        protected DistributionPointSelector()
    	{
    		Load += PageLoad;
    	}
        private void PageLoad(object sender, EventArgs e)
        {
            //imgMap.Attributes.Add("title", RM.GetString("Photolab.Shipping.DPoint.ViewMap", false));
			
			if (LoggedInUser != null)
			{
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
			}

			if (Shipping != null)
				InitSelectedShipping(Shipping);

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

        protected override void InitSelectedShipping(Shipping shipping)
        {
            var point = shipping as DistributionPoint;
            if (point == null) return;

            InitializedPoint = point;

            refFullMap.Attributes.Add("onclick", string.Format("pxp.utilities.imageZoom('{0}?{1}'); return false;", point.ImageUrl, DateTime.UtcNow.Second));
            imgMap.Src = string.Format("{0}?{1}", point.ThumbnailImageUrl, DateTime.UtcNow.Second);
            imgMap.Alt = point.GetTitle(CurrentLanguage);

            litOfficeHours.Text = point.OfficeHours;
            litPhone.Text = point.Phone;
            litTitle.Text = point.GetTitle(CurrentLanguage);
            litAddress.Text = point.Address.ToString();
            litDescription.Text = point.DescriptionLocalized[CurrentLanguage];

            if (UrlManager.CurrentPage == SiteLinkType.UserOrder)
            {
                litModalTitle.Text = point.GetTitle(CurrentLanguage);
                litLinkTitle.Text = point.GetTitle(CurrentLanguage);
            }
        }

    	public override bool ValidateInput()
	    {
	        return !IsVisibleRecipientFields || (reqFirstName.IsValid && reqLastName.IsValid && 
                reqPhone.IsValid && (reqMiddleName?.IsValid ?? true) );
	    }
	}
}