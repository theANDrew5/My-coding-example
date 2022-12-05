namespace Photoprint.Core.Models
{
    public class DistributionPointInput : BaseShippingInput
	{
		public string OfficeHours { get; set; }
        public string SitePageUrl { get; set; }

		public ShippingAddress Address { get; set; }
		public ShippingPrices ShippingPrices { get; set; }

		public DistributionPointInput() { }		
		public DistributionPointInput(DistributionPoint distributionPoint) : base(distributionPoint)
		{
			OfficeHours = distributionPoint.OfficeHours;

			Address = new ShippingAddress(distributionPoint.Address);
			ShippingPrices = new ShippingPrices(distributionPoint.PriceList);
		}
	}
}