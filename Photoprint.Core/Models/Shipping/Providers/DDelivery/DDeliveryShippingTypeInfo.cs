namespace Photoprint.Core.Models.DDelivery
{
	public class DDeliveryShippingTypeInfo
	{
		public bool IsDisabled { get; set; }
		public DDeliveryShippingType Type { get; set; }
		public int MinTime { get; set; }
		public decimal MinPrice { get; set; }

		public DDeliveryShippingTypeInfo(DDeliveryShippingType type)
		{
			Type = type;
			IsDisabled = true;
			MinTime = 0;
			MinPrice = 0;
		}
	}
}