namespace Photoprint.Core.Models
{
	public class PostalInput : BaseShippingInput
	{
		public PostalType PostalType { get; set; }

		public bool IsIndexRequired { get; set; }
		public bool IsRegionRequired { get; set; }
		public bool IsMultipleAddressLines { get; set; }
		

		public PostalInput() { }
		public PostalInput(Postal postal) : base(postal)
		{
			IsIndexRequired = postal.IsIndexRequired;
			IsRegionRequired = postal.IsRegionRequired;
			IsMultipleAddressLines = postal.IsMultipleAddressLines;
			PostalType = postal.PostalType;
		}
	}
}