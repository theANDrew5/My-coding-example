using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostOrderResponce
    {
        [JsonProperty("online-payment-mark")]
        public RussianPostOnlinePaymentMark OnlinePaymentMark { get; set; }

        [JsonProperty("address-changed")]
        public bool AddressChanged { get; set; }

        [JsonProperty("address-type-to")]
        public RussianPostAddressType AddressTypeTo { get; set; } 

        [JsonProperty("area-to")]
        public string AreaTo { get; set; }

        [JsonProperty("avia-rate")]
        public int AviaRate { get; set; }

        [JsonProperty("avia-rate-with-vat")]
        public int AviaRateWithVat { get; set; }

        [JsonProperty("avia-rate-wo-vat")]
        public int AviaRateWoVat { get; set; }

        [JsonProperty("barcode")]
        public string Barcode { get; set; }

        [JsonProperty("brand-name")]
        public string BrandName { get; set; }

        [JsonProperty("building-to")]
        public string BuildingTo { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("completeness-checking")]
        public bool CompletenessChecking { get; set; }

        [JsonProperty("completeness-checking-rate-with-vat")]
        public int CompletenessCheckingRateWithVat { get; set; }

        [JsonProperty("completeness-checking-rate-wo-vat")]
        public int CompletenessCheckingRateWoVat { get; set; }

        [JsonProperty("corpus-to")]
        public string CorpusTo { get; set; }

        [JsonProperty("customs-declaration")]
        public RussianPostCustomsDeclaration CustomsDeclaration { get; set; }

        [JsonProperty("delivery-time")]
        public RussianPostDeliveryTime DeliveryTime { get; set; }

        [JsonProperty("delivery-with-cod")]
        public bool DeliveryWithCod { get; set; }

        [JsonProperty("dimension")]
        public RussianPostDimension Dimension { get; set; }

        [JsonProperty("dimension-type")]
        public RussianPostDimensionType DimensionType { get; set; } 

        [JsonProperty("envelope-type")]
        public RussianPostEnvelopeType EnvelopeType { get; set; } 

        [JsonProperty("easy-return-rate-with-vat")]
        public int EasyReturnRateWithVat { get; set; }

        [JsonProperty("easy-return-rate-wo-vat")]
        public int EasyReturnRateWoVat { get; set; }

        [JsonProperty("fragile-rate-with-vat")]
        public int FragileRateWithVat { get; set; }

        [JsonProperty("fragile-rate-wo-vat")]
        public int FragilerateWoVat { get; set; }

        [JsonProperty("given-name")]
        public string GivenName { get; set; }

        [JsonProperty("goods")]
        public RussianPostGoods Goods { get; set; }

        [JsonProperty("ground-rate")]
        public int GroundRate { get; set; }

        [JsonProperty("ground-rate-with-vat")]
        public int GroundRateWithVat { get; set; }

        [JsonProperty("ground-rate-wo-vat")]
        public int GroundRateWoVat { get; set; }

        [JsonProperty("hotel-to")]
        public string HotelTo { get; set; }

        [JsonProperty("house-to")]
        public string HouseTo { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("index-to")]
        public int IndexTo { get; set; }

        [JsonProperty("insr-rate")]
        public int InsrRate { get; set; }

        [JsonProperty("insr-rate-with-vat")]
        public int InsrRateWithVat { get; set; }

        [JsonProperty("insr-rate-wo-vat")]
        public int InsrRateWoVat { get; set; }

        [JsonProperty("insrvalue")]
        public int Insrvalue { get; set; }

        [JsonProperty("inventory-rate-with-vat")]
        public int InventoryRateWithVat { get; set; }

        [JsonProperty("inventory-rate-wo-vat")]
        public int InventoryRateWoVat { get; set; }

        [JsonProperty("is-deleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("letter-to")]
        public string LetterTo { get; set; }

        [JsonProperty("location-to")]
        public string LocationTo { get; set; }

        [JsonProperty("mail-category")]
        public RussianPostMailCategory MailCategory { get; set; }

        [JsonProperty("mail-direct")]
        public int MailDirect { get; set; }

        [JsonProperty("mail-rank")]
        public string MailRank { get; set; }

        [JsonProperty("mail-type")]
        public RussianPostMailType MailType { get; set; }

        [JsonProperty("manual-address-input")]
        public bool ManualAddressInput { get; set; }

        [JsonProperty("mass")]
        public int Mass { get; set; }

        [JsonProperty("mass-rate")]
        public int MassRate { get; set; }

        [JsonProperty("mass-rate-with-vat")]
        public int MassRateWithVat { get; set; }

        [JsonProperty("mass-rate-wo-vat")]
        public int MassRateWoVat { get; set; }

        [JsonProperty("middle-name")]
        public string MiddleName { get; set; }

        [JsonProperty("notice-payment-method")]
        public string NoticePaymentMethod { get; set; }

        [JsonProperty("notice-rate-with-vat")]
        public int NoticeRateWithVat { get; set; }

        [JsonProperty("notice-rate-wo-vat")]
        public int NoticeRateWoVat { get; set; }

        [JsonProperty("num-address-type-to")]
        public string NumAddressTypeTo { get; set; }

        [JsonProperty("office-to")]
        public string OfficeTo { get; set; }

        [JsonProperty("order-num")]
        public string OrderNum { get; set; }

        [JsonProperty("oversize-rate-with-vat")]
        public int OversizeRateWithVat { get; set; }

        [JsonProperty("oversize-rate-wo-vat")]
        public int OversizeRatewoVat { get; set; }

        [JsonProperty("payment")]
        public int Payment { get; set; }

        [JsonProperty("payment-method")]
        public RussianPostPaymentMethod PaymentMethod { get; set; }

        [JsonProperty("place-to")]
        public string PlaceTo { get; set; }

        [JsonProperty("postmarks")]
        public string[] Postmarks { get; set; }

        [JsonProperty("postoffice-code")]
        public string PostofficeCode { get; set; }

        [JsonProperty("raw-address")]
        public string RawAddress { get; set; }

        [JsonProperty("recipient-name")]
        public string RecipientName { get; set; }

        [JsonProperty("region-to")]
        public string RegionTo { get; set; }

        [JsonProperty("room-to")]
        public string RoomTo { get; set; }

        [JsonProperty("slash-to")]
        public string SlashTo { get; set; }

        [JsonProperty("sms-notice-recipient")]
        public int SmsNoticeRecipient { get; set; }

        [JsonProperty("sms-notice-recipient-rate-with-vat")]
        public int SmsNoticeRecipientRateWithVat { get; set; }

        [JsonProperty("sms-notice-recipient-rate-wo-vat")]
        public int SmsNoticeRecipientRateWoVat { get; set; }

        [JsonProperty("str-index-to")]
        public string StrIndexTo { get; set; }

        [JsonProperty("street-to")]
        public string StreetTo { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("tel-address")]
        public string TelAddress { get; set; }

        [JsonProperty("total-rate-wo-vat")]
        public int TotalRateWoVat { get; set; }

        [JsonProperty("total-vat")]
        public int TotalVat { get; set; }

        [JsonProperty("transport-mode")]
        public string TransportMode { get; set; }

        [JsonProperty("transport-type")]
        public RussianPostTransportType TransportType { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("vladenie-to")]
        public string VladenieTo { get; set; }
    }
}




