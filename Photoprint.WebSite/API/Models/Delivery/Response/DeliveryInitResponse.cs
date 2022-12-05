using System;
using System.Collections.Generic;
using System.Linq;
using Photoprint.Core.Models;
using Photoprint.Services;
using Photoprint.WebSite.Modules;

namespace Photoprint.WebSite.API.Models.Delivery
{
    public class DeliveryInitResponse
    {
        public DeliverySettings Settings { get; set; }
        public DeliveryLocalization Localization { get; }
        public DeliveryTotalPriceControllerInitData TotalPriceInitData { get; }
        public IReadOnlyCollection<DeliveryMessage> OnInitMessages { get; }
        public string InstanceGuid { get; }

        public DeliveryInitResponse(Photolab photolab, Language language, ShoppingCart cart, DeliveryWindowSettings settings)
        {
            Settings = new DeliverySettings(settings, photolab);
            Localization = GetLocalization(photolab, language);

            var cartItems = cart?.Items;
            TotalPriceInitData = new DeliveryTotalPriceControllerInitData
            {
                ShoppingCartPrice = cartItems?.Sum(s => s.Price) ?? 0,
                DiscountPrice = cartItems?.Sum(s => s.DiscountsPriceTotal) ?? 0
            };
            OnInitMessages = GetMessages();
            InstanceGuid = Guid.NewGuid().ToString();
        }

        public class DeliverySettings
        {
            public DeliveryTotalPriceBlockSettings TotalPriceBlockSettings { get; set; }
            public DeliverySettingsRecepient RecepientSettings { get; set; }
            public DeliverySettingSelector SelectorSettings { get; set; }

            public DeliverySettings(DeliveryWindowSettings settings, Photolab photolab)
            {
                var mapType = settings.MapSettings?.MapType ?? DeliveryMapType.Yandex;

                TotalPriceBlockSettings = new DeliveryTotalPriceBlockSettings
                {
                    ShowLinkToBackInShoppingCart = photolab.Properties?.SkipShoppingCartAfterEditor ?? false,
                    LinkToBackInShoppingCart = new UrlManager { CurrentLanguageCulture = WebSiteGlobal.CurrentCultureCode }.GetHRefUrl(SiteLinkType.ShoppingCart)
                };
                RecepientSettings = new DeliverySettingsRecepient
                {
                    UseMiddleName = settings?.UseMiddleName ?? false,
                    IsCyrillic = photolab.Properties?.IsCyrillicNamesEnabled ?? true,
                    UseAdditionalEmail = settings?.IsAdditionalNotificationEmailEnabled ?? false,
                    UseAdditionalPhone = settings?.IsAdditionalNotificationPhoneNumberEnabled ?? false
                };
                SelectorSettings = new DeliverySettingSelector
                {
                    UseShippingFromPreviousOrder = settings.UseShippingFromPreviousOrder,
                    MapSettings = new DeliverySettingSelector.DeliverySettingsMap
                    {
                        MapType = mapType,
                        Data = new DeliverySettingSelector.DeliveryMapData
                        {
                            CountryLimiter = mapType==DeliveryMapType.Yandex?  
                                settings.MapSettings?.YandexMapSettings?.CountryLimiter?? CityAddressCountry.NoCountry:
                                settings.MapSettings?.GoogleMapSettings?.CountryLimiter?? CityAddressCountry.NoCountry,
                            ApiKey = mapType == DeliveryMapType.Yandex ? settings.MapSettings?.YandexMapSettings?.ApiKey : settings.MapSettings?.GoogleMapSettings?.ApiKey
                        }
                    },
                    PickpointSettings = new DeliverySettingSelector.DeliverySettingsPickpoint
                    {
                        IsFilterProviderTypeEnabled = settings.PickpointsSettings?.IsFilterProviderTypeEnabled ?? false,
                        IsSearchFilterEnabled = settings.PickpointsSettings?.IsSerchStringEnabled ?? false
                    },
                    AddressSelectSettings = new DeliverySettingSelector.DeliverySettingsAddressSelect
                    {
                        UsePostcode = settings.AddressSelectSettings?.UsePostcode ?? false,
                        UseAddressLines = settings.AddressSelectSettings?.UseAddressLines ?? false,
                    }
                };
            }

            public class DeliveryTotalPriceBlockSettings
            {
                public bool ShowLinkToBackInShoppingCart { get; set; }
                public string LinkToBackInShoppingCart { get; set; }
            }
            public class DeliverySettingsRecepient
            {
                public bool UseMiddleName { get; set; }
                public bool IsCyrillic { get; set; }
                public bool UseAdditionalEmail { get; set; }
                public bool UseAdditionalPhone { get; set; }
            }
            public class DeliverySettingSelector
            {
                public bool UseShippingFromPreviousOrder { get; set; }
                public DeliverySettingsMap MapSettings { get; set; }
                public DeliverySettingsPickpoint PickpointSettings { get; set; }
                public DeliverySettingsAddressSelect AddressSelectSettings { get; set; }

                public class DeliverySettingsMap
                {
                    public DeliveryMapData Data { get; set; }
                    public DeliveryMapType MapType { get; set; }
                }
                public class DeliveryMapData
                {
                    public CityAddressCountry  CountryLimiter { get; set; } 
                    public string ApiKey { get; set; }
                }
                public class DeliverySettingsPickpoint
                {
                    public bool IsFilterProviderTypeEnabled { get; set; }
                    public bool IsSearchFilterEnabled { get; set; }
                }
                public class DeliverySettingsAddressSelect
                {
                    public bool UsePostcode { get; set; }
                    public bool UseAddressLines { get; set; }
                }
            }
        }
        
        public class DeliveryTotalPriceControllerInitData
        {
            public decimal ShoppingCartPrice { get; set; }
            public decimal DiscountPrice { get; set; }
        }

        private IReadOnlyCollection<DeliveryMessage> GetMessages()
        {
            return Array.Empty<DeliveryMessage>();
        }

        #region Localization
        private DeliveryLocalization GetLocalization(Photolab photolab, Language language)
        {
            return new DeliveryLocalization
            {
                PageTitle = RM.GetString(RS.Delivery.PageTitle),
                DeliverySelectorLocalization = new DeliverySelectorLocalization
                {
                    DeliveryTitle = RM.GetString(RS.Delivery.DeliverySelector.DeliveryTitle, photolab, language),
                    SelectCityAlert = RM.GetString(RS.Delivery.DeliverySelector.SelectCityAlert, photolab, language),
                    NoDeliveryForCity = RM.GetString(RS.Delivery.DeliverySelector.NoDeliveryForCity, photolab, language),
                    PickPointPluginDeliveryTypeLocalization = new PickPointPluginDeliveryTypeLocalization(photolab, language),
                    SafeRoutePluginDeliveryTypeLocalization = new SafeRoutePluginDeliveryTypeLocalization(photolab, language),
                    AddressSelectDeliveryLocalization = new AddressSelectDeliveryTypeLocalization(photolab, language),
                    PointDeliveryLocalization = new PointDeliveryTypeLocalization(photolab, language)
                },
                CitySelectorLocalization = new CitySelectorLocalization
                {
                    SelectorTitle = RM.GetString(RS.Delivery.CitySelector.SelectorTitle, photolab, language),
                    ErrorAutoGeoposAlert = RM.GetString(RS.Delivery.CitySelector.ErrorAutoGeoposAlert, photolab, language),
                    NoCityResults = RM.GetString(RS.Delivery.CitySelector.NoCityResults, photolab, language),
                    Change = RM.GetString(RS.Delivery.CitySelector.Change, photolab, language),
                    DeliveryValidationErrorAlert = RM.GetString(RS.Delivery.CitySelector.DeliveryValidationErrorAlert, photolab, language),
                    Countries = new CountriesLocalization
                    {
                        CountryFilter = RM.GetString(RS.Delivery.CitySelector.CountryFilter, photolab, language),
                        NoCountry = RM.GetString(RS.Delivery.CitySelector.Country.NoCountry, photolab, language),
                        Russia = RM.GetString(RS.Delivery.CitySelector.Country.Russia, photolab, language),
                        Ukraine = RM.GetString(RS.Delivery.CitySelector.Country.Ukraine, photolab, language),
                        Belarus = RM.GetString(RS.Delivery.CitySelector.Country.Belarus, photolab, language),
                        Kazakhstan = RM.GetString(RS.Delivery.CitySelector.Country.Kazakhstan, photolab, language),
                        Bulgaria = RM.GetString(RS.Delivery.CitySelector.Country.Bulgaria)
                    }
                },
                RecipientLocalization = new RecipientLocalization
                {
                    RecipientTitle = RM.GetString(RS.Delivery.Recipient.User, photolab, language),
                    FirstNameLabel = RM.GetString(RS.Delivery.Recipient.FirstName, photolab, language),
                    MiddleNameLabel = RM.GetString(RS.Delivery.Recipient.MiddleName, photolab, language),
                    LastNameLabel = RM.GetString(RS.Delivery.Recipient.LastName, photolab, language),
                    EmailLabel = RM.GetString(RS.Delivery.Recipient.Email, photolab, language),
                    AdditionalEmailLabel = RM.GetString(RS.Delivery.Recipient.AdditionalEmail, photolab, language),
                    AdditionalPhoneLabel = RM.GetString(RS.Delivery.Recipient.AdditionalPhone, photolab, language),
                    PhoneLabel = RM.GetString(RS.Delivery.Recipient.Phone, photolab, language),
                    CommentLabel = RM.GetString(RS.Delivery.Recipient.OrderComment, photolab, language),
                    IsOrderFromUserCompanyLabel = RM.GetString(RS.Delivery.Recipient.CreateOrderFromOrganization, photolab, language)
                },
                TotalPriceControllerLocalization = new DeliveryTotalPriceControllerLocalization
                {
                    CreateOrder = RM.GetString(RS.Delivery.TotalPriceBlock.CreateOrder, photolab, language),
                    YourOrder = RM.GetString(RS.Delivery.TotalPriceBlock.YourOrder, photolab, language),
                    ShoppingCartPrice = RM.GetString(RS.Delivery.TotalPriceBlock.ShoppingCartPrice, photolab, language),
                    DiscountPrice = RM.GetString(RS.Delivery.TotalPriceBlock.DiscountPrice, photolab, language),
                    DeliveryPrice = RM.GetString(RS.Delivery.TotalPriceBlock.DeliveryPrice, photolab, language),
                    TotalPrice = RM.GetString(RS.Delivery.TotalPriceBlock.TotalPrice, photolab, language),
                    GoToShoppingCart = RM.GetString(RS.Delivery.TotalPriceBlock.GoToShoppingCart, photolab, language),
                    CantCreateOrder = RM.GetString(RS.Delivery.TotalPriceBlock.CantCreateOrder, photolab, language),
                    RefershPriceError = RM.GetString(RS.Delivery.TotalPriceBlock.RefershPriceError, photolab, language)
                }
            };
        }

        public class DeliveryLocalization
        {
            public string PageTitle { get; set; }
            public DeliverySelectorLocalization DeliverySelectorLocalization { get; set; }
            public CitySelectorLocalization CitySelectorLocalization { get; set; }
            public RecipientLocalization RecipientLocalization { get; set; }
            public DeliveryTotalPriceControllerLocalization TotalPriceControllerLocalization { get; set; }
        }
        public class DeliverySelectorLocalization
        {
            public string DeliveryTitle { get; set; }
            public string SelectCityAlert { get; set; }
            public string NoDeliveryForCity { get; set; }
            public PickPointPluginDeliveryTypeLocalization PickPointPluginDeliveryTypeLocalization { get; set; }
            public SafeRoutePluginDeliveryTypeLocalization SafeRoutePluginDeliveryTypeLocalization { get; set; }
            public AddressSelectDeliveryTypeLocalization AddressSelectDeliveryLocalization { get; set; }
            public PointDeliveryTypeLocalization  PointDeliveryLocalization { get; set; }
        }
        public class CitySelectorLocalization
        {
            public string SelectorTitle { get; set; }
            public string ErrorAutoGeoposAlert { get; set; }
            public string NoCityResults { get; set; }
            public string Change { get; set; }
            public string DeliveryValidationErrorAlert { get; set; }
            public CountriesLocalization Countries { get; set; }
        }

        public class CountriesLocalization
        {
            public string CountryFilter { get; set; }
            public string NoCountry { get; set; }
            public string Russia { get; set; }
            public string Ukraine { get; set; }
            public string Belarus { get; set; }
            public string Kazakhstan { get; set; }
            public string Bulgaria { get; set; }
        }
        public class RecipientLocalization
        {
            public string RecipientTitle { get; set; }
            public string FirstNameLabel { get; set; }
            public string MiddleNameLabel { get; set; }
            public string LastNameLabel { get; set; }
            public string EmailLabel { get; set; }
            public string AdditionalEmailLabel { get; set; }
            public string AdditionalPhoneLabel { get; set; }
            public string PhoneLabel { get; set; }
            public string CommentLabel { get; set; }
            public string IsOrderFromUserCompanyLabel { get; set; }
        }
        public class DeliveryTotalPriceControllerLocalization
        {
            public string CreateOrder { get; set; }
            public string YourOrder { get; set; }
            public string ShoppingCartPrice { get; set; }
            public string DiscountPrice { get; set; }
            public string DeliveryPrice { get; set; }
            public string TotalPrice { get; set; }
            public string GoToShoppingCart { get; set; }
            public string CantCreateOrder { get; set; }
            public string RefershPriceError { get; set; }
        }


        public class AddressModelLocalization
        {
            public string AddressStreetLabel { get; set; }
            public string AddressHouseLabel { get; set; }
            public string AddressFlatLabel { get; set; }
            public string AddressLine1Label { get; set; }
            public string AddressLine2Label { get; set; }
            public string PostalCodeLabel { get; set; }
            public string NoDeliveryAddressAlert { get; set; }

            public AddressModelLocalization(Photolab photolab, Language language)
            {
                AddressStreetLabel = RM.GetString(RS.Delivery.DeliverySelector.CourierDelivery.Address.Street, photolab, language);
                AddressHouseLabel = RM.GetString(RS.Delivery.DeliverySelector.CourierDelivery.Address.House, photolab, language);
                AddressFlatLabel = RM.GetString(RS.Delivery.DeliverySelector.CourierDelivery.Address.Flat, photolab, language);
                AddressLine1Label = RM.GetString(RS.Delivery.DeliverySelector.CourierDelivery.Address.AddressLine1, photolab, language);
                AddressLine2Label = RM.GetString(RS.Delivery.DeliverySelector.CourierDelivery.Address.AddressLine2, photolab, language);
                PostalCodeLabel = RM.GetString(RS.Delivery.DeliverySelector.CourierDelivery.Address.PostalCode, photolab, language);
                NoDeliveryAddressAlert = RM.GetString(RS.Delivery.DeliverySelector.CourierDelivery.Address.NoDeliveryAddressAlert, photolab, language);
            }
        }

        public abstract class BaseDeliveryTypeLocalization
        {

        }
        public abstract class BasePluginDeliveryTypeLocalization : BaseDeliveryTypeLocalization
        {
            public string PointSelectedTitle { get; set; }
            public BasePluginDeliveryTypeLocalization(Photolab photolab, Language language)
            {
                PointSelectedTitle = RM.GetString(RS.Delivery.DeliverySelector.PluginDelivery.PointSelectedTitle, photolab, language);
            }
        }
        public class PickPointPluginDeliveryTypeLocalization : BasePluginDeliveryTypeLocalization
        {
            public PickPointPluginDeliveryPointItemLocalization PickPointPluginDeliveryPointItemLocalization { get; set; }

            public PickPointPluginDeliveryTypeLocalization(Photolab photolab, Language language) : base(photolab, language) 
            {
                PickPointPluginDeliveryPointItemLocalization = new PickPointPluginDeliveryPointItemLocalization(photolab, language);
            }
        } 
        public class SafeRoutePluginDeliveryTypeLocalization : BasePluginDeliveryTypeLocalization
        {
            public SafeRoutePluginDeliveryPointItemLocalization SafeRoutePluginDeliveryPointItemLocalization { get; set; }

            public SafeRoutePluginDeliveryTypeLocalization(Photolab photolab, Language language) : base(photolab, language)
            {
                SafeRoutePluginDeliveryPointItemLocalization = new SafeRoutePluginDeliveryPointItemLocalization(photolab, language);
            }
        } 
        public abstract class BaseStandartDeliveryTypeLocalization : BaseDeliveryTypeLocalization
        {

        }
        public class AddressSelectDeliveryTypeLocalization : BaseStandartDeliveryTypeLocalization
        {
            public string RecipientTitle { get; set; }
            public string DeliverySelection { get; set; }
            public string NoDelivery { get; set; }
            public AddressModelLocalization AddressModelLocalization { get; set; }
            public AddressSelectDeliveryPointItemLocalization AddressSelectDeliveryPointItemLocalization { get; set; }

            public AddressSelectDeliveryTypeLocalization(Photolab photolab, Language language)
            {
                RecipientTitle = RM.GetString(RS.Delivery.Recipient.Address, photolab, language);
                DeliverySelection = RM.GetString(RS.Delivery.DeliverySelector.CourierDelivery.CourierDeliverySelection, photolab, language);
                NoDelivery = RM.GetString(RS.Delivery.DeliverySelector.NoDelivery);
                AddressModelLocalization = new AddressModelLocalization(photolab, language);
                AddressSelectDeliveryPointItemLocalization = new AddressSelectDeliveryPointItemLocalization(photolab, language);
            }
        }
        public class PointDeliveryTypeLocalization : BaseStandartDeliveryTypeLocalization
        {
            public string PointsTitle { get; set; }
            public string PointSelectedTitle { get; set; }
            public string PlaceholderText { get; set; }
            public string Found { get; set; }
            public string From { get; set; }
            public string PointsViewMapLabel { get; set; }
            public string PointsViewListLabel { get; set; }
            public string ShowOnTheMap { get; set; }
            public string FilterByBounds { get; set; }
            public string AllCompanies { get; set; }
            public string SelectPointWarning { get; set; }
            public PointDeliveryPointItemLocalization PointItemLocalization { get; set; } 
            public PointDeliveryTypeLocalization(Photolab photolab, Language language)
            {
                PointsTitle = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.PointsTitle, photolab, language);
                PointSelectedTitle = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.PointSelectedTitle, photolab, language);
                PlaceholderText = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.PlaceholderText, photolab, language);
                Found = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.Found, photolab, language);
                From = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.From, photolab, language);
                PointsViewMapLabel = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.PointsViewMap, photolab, language);
                PointsViewListLabel = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.PointsViewList, photolab, language);
                ShowOnTheMap = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.ShowOnTheMap, photolab, language);
                FilterByBounds = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.FilterByBounds, photolab, language);
                AllCompanies = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.Pickpoint.AllCompaniesFilter, photolab, language);
                SelectPointWarning = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.SelectPointWarning,
                    photolab, language);
                PointItemLocalization = new PointDeliveryPointItemLocalization(photolab, language);
            }
        }

        public abstract class BaseDeliveryPointItemLocalization
        {
            public string PriceTitle { get; set; }
            public string FreeTitle { get; set; }
            public string PeriodTitle { get; set; }
            public string PeriodArrange { get; set; }
            public string PeriodFrom { get; set; }
            public string PeriodTo { get; set; }
            public BaseDeliveryPointItemLocalization(Photolab photolab, Language language)
            {
                FreeTitle = RM.GetString(RS.Delivery.DeliverySelector.DeliveryPointItem.FreeTitle, photolab, language);
                PriceTitle = RM.GetString(RS.Delivery.DeliverySelector.DeliveryPointItem.PriceTitle, photolab, language);
                PeriodTitle = RM.GetString(RS.Delivery.DeliverySelector.DeliveryPointItem.PeriodTitle, photolab, language);
                PeriodArrange = RM.GetString(RS.Delivery.DeliverySelector.DeliveryPointItem.PeriodArrange, photolab, language);
                PeriodFrom = RM.GetString(RS.Delivery.DeliverySelector.DeliveryPointItem.PeriodFrom, photolab, language);
                PeriodTo = RM.GetString(RS.Delivery.DeliverySelector.DeliveryPointItem.PeriodTo, photolab, language);
            }
        }
        public abstract class BasePluginDeliveryPointItemLocalization : BaseDeliveryPointItemLocalization
        {
            public string AddressTitle { get; set; }
            public string PointNameTitle { get; set; }

            public BasePluginDeliveryPointItemLocalization(Photolab photolab, Language language) : base(photolab, language) 
            {
                AddressTitle = RM.GetString(RS.Delivery.DeliverySelector.DeliveryPointItem.PluginPoint.AddressTitle, photolab, language); ;
                PointNameTitle = RM.GetString(RS.Delivery.DeliverySelector.DeliveryPointItem.PluginPoint.PointDescriptionTitle, photolab, language); ;
            }
        }
        public class PickPointPluginDeliveryPointItemLocalization : BasePluginDeliveryPointItemLocalization
        {
            public PickPointPluginDeliveryPointItemLocalization(Photolab photolab, Language language) : base(photolab, language)
            {

            }
        }
        public class SafeRoutePluginDeliveryPointItemLocalization : BasePluginDeliveryPointItemLocalization
        {
            public string CourierTypeTitle { get; set; }
            public string PointTypeTitle { get; set; }
            public string MailTypeTitle { get; set; }

            public SafeRoutePluginDeliveryPointItemLocalization(Photolab photolab, Language language) : base(photolab, language)
            {
                CourierTypeTitle = "Выбрана доставка курьером";
                PointTypeTitle = "Выбрана доставка на точку выдачи";
                MailTypeTitle = "Выбрана доставка Почтой России";
            }
        }
        public abstract class BaseStandartDeliveryPointItemLocalization : BaseDeliveryPointItemLocalization
        {
            public BaseStandartDeliveryPointItemLocalization(Photolab photolab, Language language) : base(photolab, language) { }
        }
        public class AddressSelectDeliveryPointItemLocalization : BaseStandartDeliveryPointItemLocalization
        {
            public string CourierCdekTitle { get; set; }
            public string CourierIMLTitle { get; set; }
            public string CourierGeneralCourierTitle { get; set; }
            public string DescriptionTitle { get; set; }
            public AddressSelectDeliveryPointItemLocalization(Photolab photolab, Language language) : base(photolab, language)
            {
                CourierCdekTitle = RM.GetString(RS.Delivery.DeliverySelector.CourierDelivery.CourierDeliveryPointItem.CdekTitle, photolab, language);
                CourierIMLTitle = RM.GetString(RS.Delivery.DeliverySelector.CourierDelivery.CourierDeliveryPointItem.IMLTitle, photolab, language);
                CourierGeneralCourierTitle = RM.GetString(RS.Delivery.DeliverySelector.CourierDelivery.CourierDeliveryPointItem.GeneralCourierTitle, photolab, language);
                DescriptionTitle = RM.GetString(RS.Delivery.DeliverySelector.CourierDelivery.CourierDeliveryPointItem
                    .DescriptionTitle);
            }
        }
        public class PointDeliveryPointItemLocalization : BaseStandartDeliveryPointItemLocalization
        {
            public string AddressTitle { get; set; }  
            public string PhoneTitle { get; set; }  
            public string WorkTimeTitle { get; set; } 
            public string DescriptionTitle { get; set; }
            public string SelectTitle { get; set; } 
            public string SelectedTitle { get; set; } 
            public string InCalculate { get; set; } 
            public string SelectError { get; set; } 
            public string GoBack { get; set; }
            public ProviderTitlesLocalization ProviderTitles { get; set; } 
            public PointDeliveryPointItemLocalization(Photolab photolab, Language language) : base(photolab, language)
            {
                AddressTitle = RM.GetString(RS.Delivery.DeliverySelector.DeliveryPointItem.PointDeliveryPointItem.AddressTitle, photolab, language);
                PhoneTitle = RM.GetString(RS.Delivery.DeliverySelector.DeliveryPointItem.PointDeliveryPointItem.PhoneTitle, photolab, language);
                WorkTimeTitle = RM.GetString(RS.Delivery.DeliverySelector.DeliveryPointItem.PointDeliveryPointItem.WorkTimeTitle, photolab, language);
                DescriptionTitle = RM.GetString(RS.Delivery.DeliverySelector.DeliveryPointItem.PointDeliveryPointItem.DescriptionTitle, photolab, language);
                SelectTitle = RM.GetString(RS.Delivery.DeliverySelector.DeliveryPointItem.PointDeliveryPointItem.SelectTitle, photolab, language);
                SelectedTitle = RM.GetString(RS.Delivery.DeliverySelector.DeliveryPointItem.PointDeliveryPointItem.SelectedTitle, photolab, language);
                InCalculate = RM.GetString(RS.Delivery.DeliverySelector.DeliveryPointItem.PointDeliveryPointItem.InCalculate, photolab, language);
                SelectError = RM.GetString(RS.Delivery.DeliverySelector.DeliveryPointItem.PointDeliveryPointItem.SelectError, photolab, language);
                ProviderTitles = new ProviderTitlesLocalization(photolab, language);
                GoBack = RM.GetString(RS.Delivery.DeliverySelector.DeliveryPointItem.PointDeliveryPointItem.GoBack, photolab, language);
            }
        }
        public class ProviderTitlesLocalization
        {
            public string DeliveryPointCdekTitle { get; set; }
            public string DeliveryPointBoxberryTitle { get; set; }
            public string DeliveryPointIMLTitle { get; set; }
            public string DeliveryPointPickPointTitle { get; set; }
            public string DeliveryPointNovaposhtaTitle { get; set; }
            public string DeliveryPointYandexDeliveryTitle { get; set; }
            public string DeliveryPointOfficeTitle { get; set; }
            public string DeliveryPointRussianPostTitle { get; set; }
            public string DeliveryPointGeneralOfficeTitle { get; set; }
            public string DeliveryPointDpdTitle { get; set; }
            public string DeliveryPointCdekV2Title { get; set; }
            public ProviderTitlesLocalization(Photolab photolab, Language language)
            {
                DeliveryPointCdekTitle = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.Pickpoint.Item.CdekTitle, photolab, language);
                DeliveryPointBoxberryTitle = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.Pickpoint.Item.BoxberryTitle, photolab, language);
                DeliveryPointIMLTitle = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.Pickpoint.Item.IMLTitle, photolab, language);
                DeliveryPointPickPointTitle = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.Pickpoint.Item.PickPointTitle, photolab, language);
                DeliveryPointNovaposhtaTitle = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.Pickpoint.Item.NovaposhtaTitle, photolab, language);
                DeliveryPointYandexDeliveryTitle = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.Pickpoint.Item.YandexDeliveryTitle, photolab, language);
                DeliveryPointOfficeTitle = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.Pickpoint.Item.PointTitle, photolab, language);
                DeliveryPointRussianPostTitle = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.Pickpoint.Item.RussianPost, photolab, language);
                DeliveryPointGeneralOfficeTitle = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.Office.Item.GeneralOfficeTitle, photolab, language);
                DeliveryPointDpdTitle = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.Pickpoint.Item.DPD);
                DeliveryPointCdekV2Title = RM.GetString(RS.Delivery.DeliverySelector.PointDelivery.Pickpoint.Item.CdekV2Title, photolab, language);
            }
        }
        public class RussianPostDeliveryPointItemLocalization : PointDeliveryPointItemLocalization
        {
            public string DeliveryPointRussianPostTitle { get; set; }
            public RussianPostDeliveryPointItemLocalization(Photolab photolab, Language language) : base(photolab, language)
            {
                DeliveryPointRussianPostTitle = RM.GetString(RS.Delivery.DeliverySelector.RussianPost.PointItem.RussianPostTitle, photolab, language);
            }
        }
        #endregion 
    }
}
