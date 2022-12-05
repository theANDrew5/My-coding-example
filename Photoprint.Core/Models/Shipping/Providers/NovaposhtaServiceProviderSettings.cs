namespace Photoprint.Core.Models
{
	public class NovaposhtaServiceProviderSettings : IShippingServiceProviderSettings
	{
		public bool UpdateShippingAddressesAutomatically { get; set; }
		public bool RegisterOrderInProviderService { get; set; }
		public bool ChangeOrderStatusToShippedAfterAutomaticRegistration { get; set; }
        public bool SupportAddresesSynchronization => true;
        public bool ShowAddressTab => true;
        public bool MuteNotificationAfterAddressesUpdated { get; set; }

        public string ApiKey { get; set; }
		public string AuthKey { get; set; }
		public string SenderCity { get; set; }
		public string SenderCompany { get; set; }
		public string SenderAddress { get; set; }
		public string SenderContact { get; set; }
		public string SenderPhone { get; set; }
		public string ContentDescription { get; set; }


		public string ApiKey2 { get; set; }
		public string AuthKey2 { get; set; }
		public string SenderCity2 { get; set; }
		public string SenderCompany2 { get; set; }
		public string SenderAddress2 { get; set; }
		public string SenderContact2 { get; set; }
		public string SenderPhone2 { get; set; }
		public string ContentDescription2 { get; set; }

		public bool IsValid
		{
			get
			{
				return !string.IsNullOrWhiteSpace(SenderCity) &&
				       !string.IsNullOrWhiteSpace(SenderCompany) &&
				       !string.IsNullOrWhiteSpace(SenderAddress) &&
				       !string.IsNullOrWhiteSpace(SenderContact) &&
				       !string.IsNullOrWhiteSpace(SenderPhone) &&
				       !string.IsNullOrWhiteSpace(ApiKey) &&
				       !string.IsNullOrWhiteSpace(AuthKey);
			}
		}

		public NovaposhtaServiceProviderSettings()
		{
		}

		public NovaposhtaServiceProviderSettings(NovaposhtaServiceProviderSettings serviceProviderSettings, bool useSecondSettings = false)
		{
			UpdateShippingAddressesAutomatically = serviceProviderSettings.UpdateShippingAddressesAutomatically;
			RegisterOrderInProviderService = serviceProviderSettings.RegisterOrderInProviderService;
			ChangeOrderStatusToShippedAfterAutomaticRegistration = serviceProviderSettings.ChangeOrderStatusToShippedAfterAutomaticRegistration;

			ApiKey = useSecondSettings ? serviceProviderSettings.ApiKey2 : serviceProviderSettings.ApiKey;
			AuthKey = useSecondSettings ? serviceProviderSettings.AuthKey2 : serviceProviderSettings.AuthKey;
			SenderCity = useSecondSettings ? serviceProviderSettings.SenderCity2 : serviceProviderSettings.SenderCity;
			SenderCompany = useSecondSettings ? serviceProviderSettings.SenderCompany2 : serviceProviderSettings.SenderCompany;
			SenderAddress = useSecondSettings ? serviceProviderSettings.SenderAddress2 : serviceProviderSettings.SenderAddress;
			SenderContact = useSecondSettings ? serviceProviderSettings.SenderContact2 : serviceProviderSettings.SenderContact;
			SenderPhone = useSecondSettings ? serviceProviderSettings.SenderPhone2 : serviceProviderSettings.SenderPhone;
			ContentDescription = useSecondSettings ? serviceProviderSettings.ContentDescription2 : serviceProviderSettings.ContentDescription;
		}
	}
}