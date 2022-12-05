namespace Photoprint.Core.Models
{
	public class ImLogisticsServiceProviderSettings : IShippingServiceProviderSettings
	{
		
		public bool UpdateShippingAddressesAutomatically { get; set; }
		public bool RegisterOrderInProviderService { get; set; }
		public bool ChangeOrderStatusToShippedAfterAutomaticRegistration { get; set; }
        public bool SupportAddresesSynchronization => true;
        public bool ShowAddressTab => true;
        public bool MuteNotificationAfterAddressesUpdated { get; set; }

	    public bool IsTestMode { get; set; }
		public string Login { get; set; }

		private string _recipient;
		public string Recipient
		{
			get => "0001";
            set => _recipient = value;
        }

		public string ServiceCode { get; set; }
		public string ServiceCodeWithPayment { get; set; }

		public string Password { get; set; }
		public string Departure { get; set; }


		public ImLogisticsServiceProviderSettings()
		{
		}
	}
}