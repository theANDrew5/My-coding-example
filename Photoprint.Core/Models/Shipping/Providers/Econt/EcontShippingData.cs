namespace Photoprint.Core.Models
{
    public class EcontShippingData
    {
        public EcontServiceProviderSettings Settings { get; set; }
        public OrderAddress DeliveryAddress { get; set; }
        public ParcelSize Size { get; set; }
        public double TotalWeight { get; set; }
        public int PackCount { get; set; }
        public string EmailOnDelivery { get; set; }
        public string ShipmentDescription { get; set; }
        public int OrderNumber { get; set; }
        public string Mode { get; set; }

    }
}
