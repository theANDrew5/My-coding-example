using System;

namespace Photoprint.Core.Models
{
	public class ShippingActionEventArgs : EventArgs
	{
		public Shipping Shipping { get; set; }

		public ShippingActionEventArgs(Shipping shipping)
		{
			Shipping = shipping;
		}
	}
}