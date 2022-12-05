using Photoprint.Core.Models.DDelivery;

namespace Photoprint.Core.Models
{
	public class DDeliveryAddressInfo
	{
		public DDeliveryShippingType Type { get; set; }
		public DDeliveryCity City { get; set; }
		public DDeliveryPickupPoint Point { get; set; }
		public DDeliveryCalculatorResult Result { get; set; }
	}
}