namespace Photoprint.WebSite.API.Models.Delivery
{
    public class DeliveryGetUserDataResponse
    {
        public int LastShippingId { get; set; }
        public string LastShippingType { get; set; }
        public string LastAddress { get; set; }
        public bool CanBeOrderByUserCompany { get; set; }
        public DeliveryRecipient Recipient { get; set; }
    }

}
