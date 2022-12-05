using Photoprint.Core.Models;
using static Photoprint.WebSite.API.Controllers.DeliveryController;

namespace Photoprint.WebSite.API.Models.Delivery
{
    public class DeliveryCreateOrderRequest : BaseDeliveryRequest
    {
        public DeliveryFinalState DeliveryModel { get; set; }
    }
}