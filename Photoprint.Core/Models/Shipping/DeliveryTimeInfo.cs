using System;
using System.Text;

namespace Photoprint.Core.Models
{
	[Serializable]
	public class DeliveryTimeInfo 
    {
		public int MinimumShippingHours { get; set; }

		public int MaximumShippingHours { get; set; }

		public string ShippingHoursDescription { get; set; }
    }
}
