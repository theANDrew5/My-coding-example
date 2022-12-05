using Newtonsoft.Json;
using System;
using Photoprint.Core.Infrastructure.Utilities.Extensions;

namespace Photoprint.Core.Models
{
    public class RussianPostCreateOrderRequest
    {
        public RussianPostCreateOrderRequest(
            string orderNum, 
            RussianPostAddress address, 
            int mass, 
            string recipientName, 
            RussianPostMailCategory mailCategory, 
            RussianPostMailType mailType,
            string postofficeCode,
            int mailDirect = 643)
        {
            MailDirect = mailDirect;
            MailCategory = mailCategory.GetTitle<RussianPostMailCategory>();
            MailType = mailType.GetTitle<RussianPostMailType>();
            PostofficeCode = postofficeCode;

            Mass = mass;
            OrderNum = orderNum;

            RecipientName = recipientName;

            IndexTo = address.Index;
            HouseTo = address.House;
            PlaceTo = address.Place;
            RegionTo = address.Region;
            StreetTo = address.Street;
            AddressTypeTo = Enum.GetName(typeof(RussianPostAddressType), address.AddressType);
        }

        [JsonProperty("address-type-to")]
        public string AddressTypeTo { get; set; }

        [JsonProperty("house-to")]
        public string HouseTo { get; set; }

        [JsonProperty("mail-category")]
        public string MailCategory { get; set; }

        /// <summary>
        /// Список стран https://otpravka.pochta.ru/specification#/dictionary-countries
        /// </summary>
        [JsonProperty("mail-direct")]
        public int MailDirect { get; set; }

        [JsonProperty("mail-type")]
        public string MailType { get; set; }

        [JsonProperty("mass")]
        public int Mass { get; set; }

        [JsonProperty("order-num")]
        public string OrderNum { get; set; }

        [JsonProperty("place-to")]
        public string PlaceTo { get; set; }

        [JsonProperty("recipient-name")]
        public string RecipientName { get; set; }

        [JsonProperty("region-to")]
        public string RegionTo { get; set; }

        [JsonProperty("street-to")]
        public string StreetTo { get; set; }
        
        [JsonProperty("index-to")]
        public string IndexTo { get; set; }


        // (Опционально):
        [JsonProperty("surname", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Surname { get; set; }

        [JsonProperty("given-name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string GivenName { get; set; }

        [JsonProperty("fragile", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Fragile { get; set; }

        [JsonProperty("area-to", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string AreaTo { get; set; }

        [JsonProperty("branch-name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string BranchName { get; set; }

        [JsonProperty("brand-name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string BrandName { get; set; }

        [JsonProperty("building-to", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string BuildingTo { get; set; }

        [JsonProperty("comment", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Comment { get; set; }

        [JsonProperty("completeness-checking", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool CompletenessChecking { get; set; }

        [JsonProperty("compulsory-payment", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int CompulsoryPayment { get; set; }

        [JsonProperty("corpus-to", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CorpusTo { get; set; }

        [JsonProperty("courier", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Courier { get; set; }

        [JsonProperty("customs-declaration", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public RussianPostCustomsDeclaration CustomsDeclaration { get; set; }

        [JsonProperty("delivery-with-cod", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool DeliveryWithCod { get; set; }

        [JsonProperty("dimension", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public RussianPostDimension Dimension { get; set; }

        [JsonProperty("dimension-type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public RussianPostDimensionType? DimensionType { get; set; }

        [JsonProperty("easy-return", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool EasyReturn { get; set; }

        [JsonProperty("envelope-type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string EnvelopeType { get; set; }

        [JsonProperty("fiscal-data", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public RussianPostFiscalData FiscalData { get; set; }

        [JsonProperty("goods", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public RussianPostGoods Goods { get; set; }

        [JsonProperty("hotel-to", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string HotelTo { get; set; }

        [JsonProperty("insr-value", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int InsrValue { get; set; }

        [JsonProperty("inventory", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Inventory { get; set; }

        [JsonProperty("letter-to", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Letterto { get; set; }

        [JsonProperty("location-to", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string LocationTo { get; set; }

        [JsonProperty("manual-address-input", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool ManualAddressInput { get; set; }

        [JsonProperty("middle-name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string MiddleName { get; set; }

        [JsonProperty("no-return", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool NoReturn { get; set; }

        [JsonProperty("notice-payment-method", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string NoticePaymentMethod { get; set; }

        [JsonProperty("num-address-type-to", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string NumAddressTypeTo { get; set; }

        [JsonProperty("office-to", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string OfficeTo { get; set; }

        [JsonProperty("payment", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Payment { get; set; }

        [JsonProperty("payment-method", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public RussianPostPaymentMethod? PaymentMethod { get; set; }

        [JsonProperty("postoffice-code", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string PostofficeCode { get; set; }

        [JsonProperty("raw-address", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string RawAddress { get; set; }

        [JsonProperty("raw-tel-address", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string RawtelAddress { get; set; }

        [JsonProperty("roomt-o", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string RoomTo { get; set; }

        [JsonProperty("slash-to", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SlashTo { get; set; }

        [JsonProperty("sms-notice-recipient", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int SmsNoticeRecipient { get; set; }

        [JsonProperty("str-index-to", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string StrIndexTo { get; set; }

        [JsonProperty("tel-address", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string TelAddress { get; set; }

        [JsonProperty("time-slot-id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int TimeSlotId { get; set; }

        [JsonProperty("transport-mode", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string TransportMode { get; set; }

        [JsonProperty("transport-type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public RussianPostTransportType? TransportType { get; set; }

        [JsonProperty("vladenie-to", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string VladenieTo { get; set; }

        [JsonProperty("vsd", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Vsd { get; set; }

        [JsonProperty("with-electronic-notice", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool WithElectronicNotice { get; set; }

        [JsonProperty("with-order-of-notice", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool WithOrderOfNotice { get; set; }

        [JsonProperty("with-simple-notice", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool WithSimpleNotice { get; set; }

        [JsonProperty("wo-mail-rank", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool WoMailRank { get; set; }
    }
}
