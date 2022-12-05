using Newtonsoft.Json;
using System;

namespace Photoprint.Core.Models
{
    public class RussianPostShippingCalculationCoastRequest
    {
        public RussianPostShippingCalculationCoastRequest(string indexTo, int massInGrams, RussianPostMailType mailType, RussianPostMailCategory mailCategory)
        {
            IndexTo = indexTo;
            MailCategory = Enum.GetName(typeof(RussianPostMailCategory), mailCategory);
            MailType = Enum.GetName(typeof(RussianPostMailType), mailType);;
            Mass = massInGrams;
        }

        [JsonProperty("mail-category")]
        public string MailCategory { get; set; }

        [JsonProperty("mail-type")]
        public string MailType { get; set; }

        [JsonProperty("mass")]
        public int Mass { get; set; }

        [JsonProperty("index-to")]
        public string IndexTo { get; set; }
        [JsonProperty("index-from")]
        public string IndexFrom { get; set; }

        //(Опционально):
        [JsonProperty("dimension")]
        public RussianPostDimension Dimension { get; set; }

        [JsonProperty("dimension-type")]
        public RussianPostDimensionType? DimensionType { get; set; }

        [JsonProperty("entries-type")]
        public RussianPostEntriesType? EntriesType { get; set; }

        [JsonProperty("fragile")]
        public bool Fragile { get; set; }

        [JsonProperty("completeness-checking")]
        public bool CompletenessChecking { get; set; }

        [JsonProperty("contents-checking")]
        public bool ContentsChecking { get; set; }

        [JsonProperty("courier")]
        public bool Courier { get; set; }

        [JsonProperty("declared-value")]
        public int DeclaredValue { get; set; }

        [JsonProperty("delivery-point-index")]
        public string DeliveryPointIndex { get; set; }

        [JsonProperty("inventory")]
        public bool Inventory { get; set; }

        [JsonProperty("sms-notice-recipient")]
        public int SMSNoticeRecipient { get; set; }

        [JsonProperty("with-order-of-notice")]
        public bool WithOrderOfNotice { get; set; }

        [JsonProperty("with-simple-notice")]
        public bool WithSimpleNotice { get; set; }

        [JsonProperty("payment-method")]
        public RussianPostPaymentMethod? PaymentMethod { get; set; }

        [JsonProperty("transport-type")]
        public RussianPostTransportType? TransportType { get; set; }

      
    }
}
