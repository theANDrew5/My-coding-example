using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class DeliveryAddressProperties : EntityProperties
	{
	    public DeliveryAddressProperties() { }
        public DeliveryAddressProperties(string serializedProperties) : base(serializedProperties) { }

        private const string _novaposhtaAddressInfoKey = "novaposhtaAddressInfo";
		private NovaposhtaAddressInfo _novaposhtaAddressInfo;
		public NovaposhtaAddressInfo NovaposhtaAddressInfo
		{
			get => _novaposhtaAddressInfo ?? (_novaposhtaAddressInfo = !string.IsNullOrWhiteSpace(this[_novaposhtaAddressInfoKey])
			           ? JsonConvert.DeserializeObject<NovaposhtaAddressInfo>(this[_novaposhtaAddressInfoKey])
			           : null);
		    set => _novaposhtaAddressInfo = value;
		}

		private const string _cdekAddressInfoKey = "cdekAddressInfo";
		private CdekAddressInfo _cdekAddressInfo;
		public CdekAddressInfo CdekAddressInfo
		{
			get => _cdekAddressInfo ?? (_cdekAddressInfo = !string.IsNullOrWhiteSpace(this[_cdekAddressInfoKey])
			                                               ? JsonConvert.DeserializeObject<CdekAddressInfo>(this[_cdekAddressInfoKey])
			                                               : null);
		    set => _cdekAddressInfo = value;
		}

        private const string _omnivaAddressInfoKey = "omnivaAddressInfo";
        private OmnivaAddressInfo _omnivaAddressInfo;
        public OmnivaAddressInfo OmnivaAddressInfo
        {
            get => _omnivaAddressInfo ?? (_omnivaAddressInfo = !string.IsNullOrWhiteSpace(this[_omnivaAddressInfoKey])
                                                           ? JsonConvert.DeserializeObject<OmnivaAddressInfo>(this[_omnivaAddressInfoKey])
                                                           : null);
            set => _omnivaAddressInfo = value;
        }
		private const string _pickpointAddressInfoKey = "pickpointAddressInfo";
		private PickpointAddressInfo _pickpointAddressInfo;
		public PickpointAddressInfo PickpointAddressInfo
		{
			get => _pickpointAddressInfo ?? (_pickpointAddressInfo = !string.IsNullOrWhiteSpace(this[_pickpointAddressInfoKey])
														   ? JsonConvert.DeserializeObject<PickpointAddressInfo>(this[_pickpointAddressInfoKey])
														   : null);
			set => _pickpointAddressInfo = value;
		}
		private const string _EcontAddressInfoKey = "EcontAddressInfo";
		private EcontAddressInfo _EcontAddressInfo;
		public EcontAddressInfo EcontAddressInfo
		{
			get => _EcontAddressInfo ?? (_EcontAddressInfo = !string.IsNullOrWhiteSpace(this[_EcontAddressInfoKey])
														   ? JsonConvert.DeserializeObject<EcontAddressInfo>(this[_EcontAddressInfoKey])
														   : null);
			set => _EcontAddressInfo = value;
		}
		private const string _justinAddressInfoKey = "justinAddressInfo";
        private JustinAddressInfo _justinAddressInfo;
        public JustinAddressInfo JustinAddressInfo
        {
            get => _justinAddressInfo ?? (_justinAddressInfo = !string.IsNullOrWhiteSpace(this[_justinAddressInfoKey])
                       ? JsonConvert.DeserializeObject<JustinAddressInfo>(this[_justinAddressInfoKey])
                       : null);
            set => _justinAddressInfo = value;
        }

        private const string _boxberryAddressInfoKey = "boxberryAddressInfo";
        private BoxberryAddressInfo _boxberryAddressInfo;
	    public BoxberryAddressInfo BoxberryAddressInfo
        {
	        get => _boxberryAddressInfo ?? (_boxberryAddressInfo = !string.IsNullOrWhiteSpace(this[_boxberryAddressInfoKey])
	                   ? JsonConvert.DeserializeObject<BoxberryAddressInfo>(this[_boxberryAddressInfoKey])
	                   : null);
	        set => _boxberryAddressInfo = value;
	    }
        private const string _imLogisticsAddressInfoKey = "imlogisticsAddressInfo";
		private ImLogisticsAddressInfo _imlogisticsAddressInfo;
		public ImLogisticsAddressInfo ImLogisticsAddressInfo
		{
			get => _imlogisticsAddressInfo ?? (_imlogisticsAddressInfo = !string.IsNullOrWhiteSpace(this[_imLogisticsAddressInfoKey])
			                                                             ? JsonConvert.DeserializeObject<ImLogisticsAddressInfo>(this[_imLogisticsAddressInfoKey])
			                                                             : null);
		    set => _imlogisticsAddressInfo = value;
		}

		private const string _dDeliveryAddressInfoKey = "ddeliveryAddressInfo";
		private DDeliveryAddressInfo _ddeliveryAddressInfo;
		public DDeliveryAddressInfo DDeliveryAddressInfo
		{
			get => _ddeliveryAddressInfo ?? (_ddeliveryAddressInfo = !string.IsNullOrWhiteSpace(this[_dDeliveryAddressInfoKey])
			                                                         ? JsonConvert.DeserializeObject<DDeliveryAddressInfo>(this[_dDeliveryAddressInfoKey])
			                                                         : null);
		    set => _ddeliveryAddressInfo = value;
		}

	    private const string _dDeliveryV2AddressInfoKey = "ddeliveryV2AddressInfo";
	    private DDeliveryV2AddressInfo _dDeliveryV2AddressInfo;
	    public DDeliveryV2AddressInfo DDeliveryV2AddressInfo
	    {
	        get => _dDeliveryV2AddressInfo ?? (_dDeliveryV2AddressInfo = !string.IsNullOrWhiteSpace(this[_dDeliveryV2AddressInfoKey])
	                   ? JsonConvert.DeserializeObject<DDeliveryV2AddressInfo>(this[_dDeliveryV2AddressInfoKey])
	                   : null);
	        set => _dDeliveryV2AddressInfo = value;
	    }


        private const string _novaposhtaV2AddressInfoKey = "novaposhtaV2AddressInfo";
        private NovaposhtaV2AddressInfo _novaposhtaV2AddressInfo;
        public NovaposhtaV2AddressInfo NovaposhtaV2AddressInfo
        {
            get => _novaposhtaV2AddressInfo ?? (_novaposhtaV2AddressInfo = !string.IsNullOrWhiteSpace(this[_novaposhtaV2AddressInfoKey])
                                                                           ? JsonConvert.DeserializeObject<NovaposhtaV2AddressInfo>(this[_novaposhtaV2AddressInfoKey])
                                                                           : null);
            set => _novaposhtaV2AddressInfo = value;
        }

		private const string _imlV2AddressInfoKey = "imlV2AddressInfo";
		public ImlV2AddressInfo ImlV2AddressInfo
	    {
            get => DeserializeItem<ImlV2AddressInfo>(_imlV2AddressInfoKey);
	        set => SerializeItem(_imlV2AddressInfoKey, value);
	    }

		private const string _photomaxAddressInfoKey = "photomaxAddressInfo";
		public PhotomaxAddressInfo PhotomaxAddressInfo
	    {
	        get => DeserializeItem<PhotomaxAddressInfo>(_photomaxAddressInfoKey);
	        set => SerializeItem(_photomaxAddressInfoKey, value);
	    }

		private const string _exgarantAddressInfoKey = "exgarantAddressInfo";
		public ExgarantAddressInfo ExgarantAddressInfo
	    {
	        get => DeserializeItem<ExgarantAddressInfo>(_exgarantAddressInfoKey);
	        set => SerializeItem(_exgarantAddressInfoKey, value);
	    }

	    private const string _ukrposhtaAddressInfoKey = "ukrposhtaAddressInfo";
        public UkrposhtaAddressInfo UkrposhtaAddressInfo
	    {
	        get => DeserializeItem<UkrposhtaAddressInfo>(_ukrposhtaAddressInfoKey);
	        set => SerializeItem(_ukrposhtaAddressInfoKey, value);
	    }

        private const string _postnlAddressInfoKey = "postnlAddressInfo";
        public PostnlAddressInfo PostnlAddressInfo
        {
            get => DeserializeItem<PostnlAddressInfo>(_postnlAddressInfoKey);
            set => SerializeItem(_postnlAddressInfoKey, value);
        }

        private const string _evropochtaAddressInfoKey = "evropochtaAddressInfo";
        public EvropochtaAddressInfo EvropochtaAddressInfo
        {
            get => DeserializeItem<EvropochtaAddressInfo>(_evropochtaAddressInfoKey);
            set => SerializeItem(_evropochtaAddressInfoKey, value);
		}

		private const string _yandexDeliveryAddressInfoKey = "yandexDeliveryAddressInfo";
		public YandexDeliveryAddressInfo YandexDeliveryAddressInfo
		{
			get => DeserializeItem<YandexDeliveryAddressInfo>(_yandexDeliveryAddressInfoKey);
			set => SerializeItem(_yandexDeliveryAddressInfoKey, value);
		}

		private const string _rpAddressInfoKey = "RussianPostAddressInfo";
		public RussianPostAddressInfo RussianPostAddressInfo
        {
			get => DeserializeItem<RussianPostAddressInfo>(_rpAddressInfoKey);
			set => SerializeItem(_rpAddressInfoKey, value);
        }

		private const string _yandexGoOrderAddressInfo = "YandexGoOrderAddressInfo";
		public YandexGoOrderAddressInfo YandexGoOrderAddressInfo
		{
			get => DeserializeItem<YandexGoOrderAddressInfo>(_yandexGoOrderAddressInfo);
			set => SerializeItem(_yandexGoOrderAddressInfo, value);
		}

		private const string _yandexShippingGoAddressInfoKey = "YandexGoShippingAddressInfo";
		public YandexGoShippingAddressInfo YandexGoShippingAddressInfo
        {
            get => DeserializeItem<YandexGoShippingAddressInfo>(_yandexShippingGoAddressInfoKey);
            set => SerializeItem(_yandexShippingGoAddressInfoKey, value);
        }

		private const string _dpdAddressInfoKey = "DpdAddressInfo";
		public DpdAddressInfo DpdAddressInfo
		{
			get => DeserializeItem<DpdAddressInfo>(_dpdAddressInfoKey);
			set => SerializeItem(_dpdAddressInfoKey, value);
		}
       

		private const string _cdekV2AddressInfoKey = "CDEKv2AddressInfo";
		public CDEKv2AddressInfo CDEKv2AddressInfo
		{
			get => DeserializeItem<CDEKv2AddressInfo>(_cdekV2AddressInfoKey);
			set => SerializeItem(_cdekV2AddressInfoKey, value);
		}

		public override string SerializeToString()
		{
			if (NovaposhtaAddressInfo != null) this[_novaposhtaAddressInfoKey] = JsonConvert.SerializeObject(NovaposhtaAddressInfo);
			else this[_novaposhtaAddressInfoKey] = null;

			if (CdekAddressInfo != null) this[_cdekAddressInfoKey] = JsonConvert.SerializeObject(CdekAddressInfo);
			else this[_cdekAddressInfoKey] = null;

			if (ImLogisticsAddressInfo != null) this[_imLogisticsAddressInfoKey] = JsonConvert.SerializeObject(ImLogisticsAddressInfo);
			else this[_imLogisticsAddressInfoKey] = null;

			if (DDeliveryAddressInfo != null) this[_dDeliveryAddressInfoKey] = JsonConvert.SerializeObject(DDeliveryAddressInfo);
			else this[_dDeliveryAddressInfoKey] = null;

            if (NovaposhtaV2AddressInfo != null) this[_novaposhtaV2AddressInfoKey] = JsonConvert.SerializeObject(NovaposhtaV2AddressInfo);
            else this[_novaposhtaV2AddressInfoKey] = null;

		    if (BoxberryAddressInfo != null) this[_boxberryAddressInfoKey] = JsonConvert.SerializeObject(BoxberryAddressInfo);
		    else this[_boxberryAddressInfoKey] = null;

		    if (DDeliveryV2AddressInfo != null) this[_dDeliveryV2AddressInfoKey] = JsonConvert.SerializeObject(DDeliveryV2AddressInfo);
		    else this[_dDeliveryV2AddressInfoKey] = null;

		    if (UkrposhtaAddressInfo != null) this[_ukrposhtaAddressInfoKey] = JsonConvert.SerializeObject(UkrposhtaAddressInfo);
		    else this[_ukrposhtaAddressInfoKey] = null;

            if (PostnlAddressInfo != null) this[_postnlAddressInfoKey] = JsonConvert.SerializeObject(PostnlAddressInfo);
            else this[_postnlAddressInfoKey] = null;

            if (JustinAddressInfo != null) this[_justinAddressInfoKey] = JsonConvert.SerializeObject(JustinAddressInfo);
            else this[_justinAddressInfoKey] = null;

            if (OmnivaAddressInfo != null) this[_omnivaAddressInfoKey] = JsonConvert.SerializeObject(OmnivaAddressInfo);
            else this[_omnivaAddressInfoKey] = null;

			if (PickpointAddressInfo != null) this[_pickpointAddressInfoKey] = JsonConvert.SerializeObject(PickpointAddressInfo);
			else this[_pickpointAddressInfoKey] = null;

			if (EcontAddressInfo != null) this[_EcontAddressInfoKey] = JsonConvert.SerializeObject(EcontAddressInfo);
			else this[_EcontAddressInfoKey] = null;

			if (YandexGoOrderAddressInfo != null) this[_yandexGoOrderAddressInfo] = JsonConvert.SerializeObject(YandexGoOrderAddressInfo);
			else this[_yandexGoOrderAddressInfo] = null;

			if (DpdAddressInfo != null) this[_dpdAddressInfoKey] = JsonConvert.SerializeObject(DpdAddressInfo);
			else this[_dpdAddressInfoKey] = null;

			if (CDEKv2AddressInfo != null) this[_cdekV2AddressInfoKey] = JsonConvert.SerializeObject(CDEKv2AddressInfo);
			else this[_cdekV2AddressInfoKey] = null;
			
			return base.SerializeToStringNullable();
		}
        
    }
}