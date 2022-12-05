using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("ItemDetail")]
    public class UspsItemDetail
    {
        [XmlElement("Description")]
        public string Description { get; set; } = string.Empty;
        [XmlElement("Quantity")]
        public int Quantity { get; set; } 
        [XmlElement("Value")]
        public decimal Value { get; set; }
        [XmlElement("NetPounds")]
        public string NetPounds { get; set; } = string.Empty;
        [XmlElement("NetOunces")]
        public string NetOunces { get; set; } = string.Empty;
        [XmlElement("HSTariffNumber")]
        public string HSTariffNumber { get; set; } = string.Empty;
        [XmlElement("CountryOfOrigin")]
        public string CountryOfOrigin { get; set; } = string.Empty;
    }

}
