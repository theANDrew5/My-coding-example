using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Photoprint.Core.Models
{
    internal static class ShippingServiceProviderSettingsResolver
    {
        public static IShippingServiceProviderSettings GetSettings(ShippingServiceProviderType type, string settingsText)
        {
            var isShippingServiceProviderSettingsTextNotEmpty = !string.IsNullOrWhiteSpace(settingsText);
            switch (type)
				{
					case ShippingServiceProviderType.Cdek:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<CdekServiceProviderSettings>(settingsText)
							: new CdekServiceProviderSettings();
					case ShippingServiceProviderType.NovaposhtaV2:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<NovaposhtaV2ServiceProviderSettings>(settingsText)
							: new NovaposhtaV2ServiceProviderSettings();
					case ShippingServiceProviderType.ImLogistics:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<ImLogisticsServiceProviderSettings>(settingsText)
							: new ImLogisticsServiceProviderSettings();
					case ShippingServiceProviderType.DDelivery:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<DDeliveryServiceProviderSettings>(settingsText)
							: new DDeliveryServiceProviderSettings();
					case ShippingServiceProviderType.ImlV2:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<ImlV2ServiceProviderSettings>(settingsText)
							: new ImlV2ServiceProviderSettings();
					case ShippingServiceProviderType.Photomax:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<PhotomaxServiceProviderSettings>(settingsText)
							: new PhotomaxServiceProviderSettings();
					case ShippingServiceProviderType.Exgarant:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<ExgarantServiceProviderSettings>(settingsText)
							: new ExgarantServiceProviderSettings();
					case ShippingServiceProviderType.Boxberry:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<BoxberryServiceProviderSettings>(settingsText)
							: new BoxberryServiceProviderSettings();
					case ShippingServiceProviderType.DDeliveryV2:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<DDeliveryV2ServiceProviderSettings>(settingsText)
							: new DDeliveryV2ServiceProviderSettings();
					case ShippingServiceProviderType.Ukrposhta:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<UkrposhtaServiceProviderSettings>(settingsText)
							: new UkrposhtaServiceProviderSettings();
					case ShippingServiceProviderType.Postnl:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<PostnlServiceProviderSettings>(settingsText)
							: new PostnlServiceProviderSettings();
					case ShippingServiceProviderType.Justin:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<JustinServiceProviderSettings>(settingsText)
							: new JustinServiceProviderSettings();
					case ShippingServiceProviderType.YandexDelivery:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<YandexDeliveryServiceProviderSettings>(settingsText)
							: new YandexDeliveryServiceProviderSettings();
					case ShippingServiceProviderType.Omniva:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<OmnivaServiceProviderSettings>(settingsText)
							: new OmnivaServiceProviderSettings();
					case ShippingServiceProviderType.Econt:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<EcontServiceProviderSettings>(settingsText)
							: new EcontServiceProviderSettings();
					case ShippingServiceProviderType.Evropochta:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<EvropochtaServiceProviderSettings>(settingsText)
							: new EvropochtaServiceProviderSettings();
					case ShippingServiceProviderType.Pickpoint:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<PickpointServiceProviderSettings>(settingsText)
							: new PickpointServiceProviderSettings();
					case ShippingServiceProviderType.RussianPost:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<RussianPostServiceProviderSettings>(settingsText)
							: new RussianPostServiceProviderSettings();
					case ShippingServiceProviderType.YandexGo:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<YandexGoServiceProviderSettings>(settingsText)
							: new YandexGoServiceProviderSettings();
					case ShippingServiceProviderType.DPD:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<DpdServiceProviderSettings>(settingsText)
							: new DpdServiceProviderSettings();
					case ShippingServiceProviderType.CDEKv2:
						return isShippingServiceProviderSettingsTextNotEmpty
							? JsonConvert.DeserializeObject<CDEKv2ServiceProviderSettings>(settingsText)
							: new CDEKv2ServiceProviderSettings();
                    case ShippingServiceProviderType.USPS:
                        return isShippingServiceProviderSettingsTextNotEmpty
                            ? JsonConvert.DeserializeObject<UspsServiceProviderSettings>(settingsText)
                            : new UspsServiceProviderSettings();
                    default:
						return null;
            }

        }
    }
    public static class ShippingDBConstraints
	{
		public static class Max
		{
			public const int PhoneLength = 200;
			public const int EmailLength = 200;
			public const int CustomReportShippingIdLength = 200;
			public const int OfficeHoursLength = 200;
		}
	}

	public class ShippingWithUsersDTO
    {
        public int ShippingId { get; }
        public int PhotolabId { get; }
        public IReadOnlyCollection<int> UserIds { get; }

        public ShippingWithUsersDTO(int shippingId, int photolabId, IReadOnlyCollection<int> userIds)
        {
            ShippingId = shippingId;
            UserIds = userIds;
            PhotolabId = photolabId;
        }
    }

    public class ShippingSmallInCustomWorkItem : ShippingSmall
    {
        public bool IsActive { get; set; }
    }

    public class ShippingSmall : EntityBase, IAdminNamedEntity
    {
        public int PhotolabId { get; set; }
        public virtual ShippingType Type { get; set; }
        public string AdminTitle { get; set; }
    }

    public abstract class Shipping : ShippingSmall
    {
		public bool IsEnabled { get; set; }
        public bool IsEnabledForMobileApp { get; set; }
        public int Position { get; set; }
        public string Phone { get; set; }
        public bool IsEnableCompanyName { get; set; }
        public string Email { get; set; }
        public bool IsShippingPricePaidSeparately { get; set; }
        public bool UseMiddleName { get; set; }
        public decimal? OrderSalesTax { get; set; }
        public decimal? DeliveryTax { get; set; }
        public decimal? MinimumPrice { get; set; }
        public decimal? MaximumPrice { get; set; }
        public string CustomReportShippingId { get; set; }
        public string OfficeHours { get; set; }
        public DateTime? WorkStartTime { get; set; }
        public DateTime? WorkEndTime { get; set; }
        public ShippingServiceProviderType ShippingServiceProviderType { get; set; }
        public LocalizableString TitleLocalized { get; set; }
		public ShippingProperties Properties { get; set; }
		public LocalizableString DescriptionLocalized { get; set; }
		public virtual ShippingAddress ContractorAddress { get; set; }
		public virtual ShippingAddress Address { get; set; }
        public virtual double MaximumWeight { get; }
        public virtual bool IsAvailableWeightConstrain { get; }

		private IShippingServiceProviderSettings _serviceProviderSettings;
		public IShippingServiceProviderSettings ServiceProviderSettings
		{
			get
			{
				if (_serviceProviderSettings != null) return _serviceProviderSettings;
                _serviceProviderSettings = ShippingServiceProviderSettingsResolver.GetSettings(
                    ShippingServiceProviderType,
                    Properties.ShippingServiceProviderSettingsText);
                return _serviceProviderSettings;
            }
            set
            {
				if (value != null)
				{
					_serviceProviderSettings = value;
					Properties.ShippingServiceProviderSettingsText = JsonConvert.SerializeObject(value);
				}
				else
				{
					_serviceProviderSettings = null;
					Properties.ShippingServiceProviderSettingsText = null;
				}
			}
		}

		public override string ToString() => AdminTitle;

        /// <summary>
        /// данный метод существует для уменьшения дублирования кода, 
        /// потому что по старой логике на сайте могли выводиться Title с админки
        /// </summary>
        public string GetTitle(Language language)
        {
            return language == null
				? !TitleLocalized.IsEmpty() ? TitleLocalized[GeneralLanguageType.English] : AdminTitle
				: !string.IsNullOrEmpty(TitleLocalized[language]) ? TitleLocalized[language] : AdminTitle;
        }

        protected Shipping()
        {
            Properties = new ShippingProperties();
            AdminTitle = string.Empty;
			TitleLocalized = new LocalizableString();
			DescriptionLocalized = new LocalizableString();
        }
    }

	// модель используется для укорения запроса получения общих видов доставок для новой доставки
    public class ShippingSmallToDeliveryModel: ShippingSmall
    {
        public LocalizableString TitleLocalized { get; set; }
        public LocalizableString DescriptionLocalized { get; set; }
		public PostalType PostalType { get; set; }
        public ShippingServiceProviderType ServiceProviderType { get; set; }
		public virtual IShippingServiceProviderSettings ServiceProviderSettings { get; set; }
		public string WorkTime { get; set; }
		public virtual bool IsIndexRequired { get; set; }
        public int AddressesCount { get; set; }
        public string GetTitle(Language language)
        {
            return language == null
                ? !TitleLocalized.IsEmpty() ? TitleLocalized[GeneralLanguageType.Russian] : AdminTitle
                : !string.IsNullOrEmpty(TitleLocalized[language]) ? TitleLocalized[language] : AdminTitle;
        }

        public string GetDescription(Language language)
        {
            return language == null
                ? !DescriptionLocalized.IsEmpty() ? DescriptionLocalized[GeneralLanguageType.Russian] : null
                : !string.IsNullOrEmpty(DescriptionLocalized[language]) ? DescriptionLocalized[language] : null;
        }
    }

    public class ShippingSmallToDeliveryModelProxy: ShippingSmallToDeliveryModel
    {
		public ShippingProperties Properties { get; set; }
		private IShippingServiceProviderSettings _serviceProviderSettings;
        public override IShippingServiceProviderSettings ServiceProviderSettings
        {
            get
            {
                if (_serviceProviderSettings != null) return _serviceProviderSettings;
                _serviceProviderSettings = ShippingServiceProviderSettingsResolver.GetSettings(
                    ServiceProviderType,
                    Properties.ShippingServiceProviderSettingsText);
                return _serviceProviderSettings;
            }
            set
            {
                if (value != null)
                {
                    _serviceProviderSettings = value;
                    Properties.ShippingServiceProviderSettingsText = JsonConvert.SerializeObject(value);
                }
                else
                {
                    _serviceProviderSettings = null;
                    Properties.ShippingServiceProviderSettingsText = null;
                }
            }
        }
        private const string _isIndexRequiredKey = "IsIndexRequired";
        public override bool IsIndexRequired
        {
            get => Properties.DeserializeItem<bool>(_isIndexRequiredKey);
            set => Properties.SerializeItem(_isIndexRequiredKey, value);
        }
    }
}