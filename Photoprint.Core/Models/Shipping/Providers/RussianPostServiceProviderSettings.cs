using Photoprint.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Photoprint.Core.Models
{
    public class RussianPostServiceProviderSettings : IPriceCalculationProviderSettings
    {
        public string BaseUrl { get; set; } = "https://otpravka-api.pochta.ru";
        public string BaseCalcUrl { get; set; } = "https://tariff.pochta.ru/v2/calculate/tariff?json"; //api для калькуляции тарифов

        public string AccessToken { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public string TrackLogin { get; set; }
        public string TrackPassword { get; set; }

        public RussianPostDepartureSettings DepartureSettings { get; set; } = new RussianPostDepartureSettings();

        public bool RegisterOrderInProviderService { get; set; }
        public bool ChangeOrderStatusToShippedAfterAutomaticRegistration { get; set; }

        //не включать пока не придумаем как прожевать 35к адресов
        public bool SupportAddresesSynchronization => false;
        public bool ShowAddressTab => false;
        public bool UpdateShippingAddressesAutomatically => false;
        public bool MuteNotificationAfterAddressesUpdated => false;

        public bool IsDeliveryPriceCalculationEnabled => true;

        public decimal AdditionalPrice { get; set; }
        public decimal PriceMultiplier { get; set; }
        public decimal DefaultPrice { get; set; }
        public bool GetDefaultPriceFromAddressList { get; set; }
        public int? DefaultShippingPriceId { get; set; }

        public string GetAuthorizationToken(string login, string password)
        {
            return $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{login}:{password}"))}" ;
        }
    }

    public class RussianPostDepartureSettings
    {
        public bool PostomatsEnable { get; set; }
        public bool DeclareValue { get; set; }
        public bool SmsNoticeRecipient { get; set; }
        public bool CompletenessChecking { get; set; }
        public bool CompulsoryPaymentAvailable{ get; set; }// Card On Delyvery или обязательный платёж
        public bool CashOnDelyveryPaymentAvailable { get; set; }// наложенный платёж
        public int DefaultWeight { get; set; }
        public string PostalCode { get; set; }
    }
}
