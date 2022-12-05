using System.Collections.Generic;

namespace Photoprint.Core.Models
{
    public class YandexDeliveryShipping
    {
        public string TariffName { get; set; }
        public string TariffId { get; set; }
        public YandexDeliveryShippingType Type { get; set; }
        public string Tag { get; set; }
        public string PartnerId { get; set; }
        public string PartnerName { get; set; }
        public string DeliveryForCustomer { get; set; }
        public string DeliveryForSender { get; set; }
        public List<string> PickupPointIds { get; set; }
        public decimal Cost { get; set; }
    }
}
