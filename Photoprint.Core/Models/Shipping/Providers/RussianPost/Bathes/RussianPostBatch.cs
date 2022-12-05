using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class RussianPostBatch
    {
        [JsonProperty("online-payment-mark")]
        public RussianPostOnlinePaymentMark OnlinePaymentMark { get; set; }

        [JsonProperty("batch-name")]
        public string BatchName { get; set; }

        [JsonProperty("batch-status")]
        public RussianPostBatchStatus? BatchStatus { get; set; }

        [JsonProperty("batch-status-date")]
        public string BatchStatusDate { get; set; }

        [JsonProperty("combined-batch-mail-types")]
        public string[] CombinedBatchMailTypes { get; set; }

        [JsonProperty("courier-call-rate-with-vat")]
        public int CourierCallRateWithVat { get; set; }

        [JsonProperty("courier-call-rate-wo-vat")]
        public int CourierCallRateWoVat { get; set; }

        [JsonProperty("courier-order-statuses")]
        public string[] CourierOrderStatuses { get; set; }

        [JsonProperty("delivery-notice-payment-method")]
        public string DeliveryNoticePaymentMethod { get; set; }

        [JsonProperty("international")]
        public bool International { get; set; }

        [JsonProperty("list-number")]
        public int ListNumber { get; set; }

        [JsonProperty("list-number-date")]
        public string ListNumberDate { get; set; }

        [JsonProperty("mail-category")]
        public RussianPostMailCategory? MailCategory { get; set; }

        [JsonProperty("mail-category-text")]
        public string MailCategorytext { get; set; }

        [JsonProperty("mail-rank")]
        public string MailRank { get; set; }

        [JsonProperty("mail-type")]
        public RussianPostMailType? MailType { get; set; }

        [JsonProperty("mail-type-text")]
        public string MailTypeText { get; set; }

        [JsonProperty("notice-payment-method")]
        public string NoticePaymentMethod { get; set; }

        [JsonProperty("payment-method")]
        public string PaymentMethod { get; set; }

        [JsonProperty("postmarks")]
        public string[] Postmarks { get; set; }

        [JsonProperty("postoffice-address")]
        public string PostofficeAddress { get; set; }

        [JsonProperty("postoffice-code")]
        public string PostofficeCode { get; set; }

        [JsonProperty("postoffice-name")]
        public string PostofficeName { get; set; }

        [JsonProperty("shipment-avia-rate-sum")]
        public int ShipmentAviaRateSum { get; set; }

        [JsonProperty("shipment-avia-rate-vat-sum")]
        public int ShipmentAviaRateVatSum { get; set; }

        [JsonProperty("shipment-completeness-checking-rate-sum")]
        public int ShipmentCompletenessCheckingRateSum { get; set; }

        [JsonProperty("shipment-completeness-checking-rate-vat-sum")]
        public int ShipmentCompletenessCheckingRateVatSum { get; set; }

        [JsonProperty("shipment-count")]
        public int ShipmentCount { get; set; }

        [JsonProperty("shipment-ground-rate-sum")]
        public int ShipmentGroundRateSum { get; set; }

        [JsonProperty("shipment-ground-rate-vat-sum")]
        public int ShipmentGroundRateVatSum { get; set; }

        [JsonProperty("shipment-insure-rate-sum")]
        public int Shipmentinsureratesum { get; set; }

        [JsonProperty("shipment-insure-rate-vat-sum")]
        public int ShipmentInsureRateVatSum { get; set; }

        [JsonProperty("shipment-inventory-rate-sum")]
        public int Shipmentinventoryratesum { get; set; }

        [JsonProperty("shipment-inventory-rate-vat-sum")]
        public int ShipmentInventoryRateVatSum { get; set; }

        [JsonProperty("shipment-mass")]
        public int ShipmentMass { get; set; }

        [JsonProperty("shipment-mass-rate-sum")]
        public int ShipmentMassRateSum { get; set; }

        [JsonProperty("shipment-mass-rate-vat-sum")]
        public int ShipmentMassRateVatSum { get; set; }

        [JsonProperty("shipment-notice-rate-sum")]
        public int Shipmentnoticeratesum { get; set; }

        [JsonProperty("shipment-notice-rate-vat-sum")]
        public int ShipmentNoticeRateVatSum { get; set; }

        [JsonProperty("shipment-sms-notice-rate-sum")]
        public int ShipmentSmsNoticeRateSum { get; set; }

        [JsonProperty("shipment-sms-notice-rate-vat-sum")]
        public int ShipmentSmsNoticeRateVatSum { get; set; }

        [JsonProperty("shipping-notice-type")]
        public string ShippingNoticeType { get; set; }

        [JsonProperty("transport-type")]
        public RussianPostTransportType? TransportType { get; set; }

        [JsonProperty("use-online-balance")]
        public bool UseOnlineBalance { get; set; }

        [JsonProperty("wo-mass")]
        public bool WoMass { get; set; }
    }
}
