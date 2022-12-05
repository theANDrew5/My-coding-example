using System;
using System.Collections.Generic;

namespace Photoprint.Core.Models
{

	[Serializable]
	public class PostalSalesTax
	{
		public string ZipCode { get; set; }
		public double TaxRate { get; set; }

		public PostalSalesTax()
		{
			ZipCode = string.Empty;
			TaxRate = 0;
		}
	}

	[Serializable]
	public class PostalSalesTaxes
    {
		public List<PostalSalesTax> PostalSalesTaxesList { get; set; }

		public PostalSalesTaxes()
		{
			PostalSalesTaxesList = new List<PostalSalesTax>();
		}
    }
}
