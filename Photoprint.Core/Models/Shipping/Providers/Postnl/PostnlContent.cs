namespace Photoprint.Core.Models.Postnl
{
    public class PostnlContent
    {
        private string Description { get; set; } // Description of goods
        public string EAN { get; set; } // A unique code for a product. Together with HS number this is mandatory for product code 4992.
        public string ProductURL { get; set; } // Webshop URL of the product which is being shipped. Mandatory for product code 4992
        public string Quantity { get; set; } // Quantity of items in description
        public string Weight { get; set; } // Net weight of goods in gram(gr)
        public string Value { get; set; } // Commercial(customs) value of goods.
        public string HSTariffNr { get; set; } // First 6 numbers of Harmonized System Code
        public string CountryOfOrigin { get; set; } // Origin country code
    }
}
