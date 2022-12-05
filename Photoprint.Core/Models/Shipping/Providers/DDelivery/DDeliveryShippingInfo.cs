namespace Photoprint.Core.Models.DDelivery
{
	public class DDeliveryShippingInfo
	{
		public DDeliveryShippingTypeInfo Courier { get; set; }
		public DDeliveryShippingTypeInfo Point { get; set; }

		public DDeliveryShippingInfo(DDeliveryShippingTypeInfo courier, DDeliveryShippingTypeInfo point)
		{
			Courier = courier ?? new DDeliveryShippingTypeInfo(DDeliveryShippingType.Courier);
			Point = point ?? new DDeliveryShippingTypeInfo(DDeliveryShippingType.Point);
		}
	}
}