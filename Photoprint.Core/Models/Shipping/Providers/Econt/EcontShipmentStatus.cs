namespace Photoprint.Core.Models
{
    public class EcontShipmentStatus
    {
        public string shipmentNumber { get; set; }
        public string storageOfficeName { get; set; }
        public string storagePersonName { get; set; }
        public string createdTime { get; set; }
        public string sendTime { get; set; }
        public string deliveryTime { get; set; }
        public string shipmentType { get; set; }
        public int? packCount { get; set; }
        public double? weight { get; set; }
        public string shipmentDescription { get; set; }
        public string senderDeliveryType { get; set; }
        public EcontPerson senderClient { get; set; }
        public string senderAgent { get; set; }
        public string senderOfficeCode { get; set; }
        public string senderAddress { get; set; }
        public string receiverDeliveryType { get; set; }
        public EcontPerson receiverClient { get; set; }
        public string receiverAgent { get; set; }
        public string receiverOfficeCode { get; set; }
        public string receiverAddress { get; set; }
        public decimal? cdCollectedAmount { get; set; }
        public string cdCollectedCurrency { get; set; }
        public string cdCollectedTime { get; set; }
        public decimal? cdPaidAmount { get; set; }
        public string cdPaidCurrency { get; set; }
        public string cdPaidTime { get; set; }
        public decimal? totalPrice { get; set; }
        public string currency { get; set; }
        public decimal? discountPercent { get; set; }
        public decimal? discountAmount { get; set; }
        public string discountDescription { get; set; }
        public decimal? senderDueAmount { get; set; }
        public decimal? receiverDueAmount { get; set; }
        public decimal? otherDueAmount { get; set; }
        public int? deliveryAttemptCount { get; set; }
        public string previousShipmentNumber { get; set; }
        public EcontService[] services { get; set; }
        public object[] nextShipments { get; set; }
        public object[] trackingEvents { get; set; }
        public string pdfURL { get; set; }
        public string expectedDeliveryDate { get; set; }
    }


}
