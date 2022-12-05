using JetBrains.Annotations;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryAddressInfo
    {
        public string YandexOrderId { get; set; }
        public string GeoId { get; set; }
        [CanBeNull] public string PickPointId { get; set; }
        public string PartnerId { get; set; }
        public string TariffId { get; set; }
        public YandexDeliveryShippingType? DeliveryType { get; set; }
        //unused in new delivery props
        public YandexDeliveryPickupPointResponse PickupPoint { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
    }
}
