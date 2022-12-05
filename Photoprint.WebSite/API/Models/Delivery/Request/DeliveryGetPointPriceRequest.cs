namespace Photoprint.WebSite.API.Models.Delivery
{
    public class DeliveryGetPointPriceRequest : BaseDeliveryRequest
    {
        public DistributionPointDto DistributionPoint { get; set; }
    }
}