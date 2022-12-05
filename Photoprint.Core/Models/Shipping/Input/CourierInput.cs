namespace Photoprint.Core.Models
{
	public class CourierInput : BaseShippingInput
	{
		public bool IsIndexRequired { get; set; }
		public bool IsRegionRequired { get; set; }
		public bool IsEnableCompanyName { get; set; }


		public CourierInput() { }
		public CourierInput(Courier courier) : base(courier)
		{
			IsIndexRequired = courier.IsIndexRequired;
			IsRegionRequired = courier.IsRegionRequired;
			IsEnableCompanyName = courier.IsEnableCompanyName;
        }
	}
}