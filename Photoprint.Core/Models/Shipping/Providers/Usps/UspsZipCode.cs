using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("ZipCode")]
    public class UspsZipCode
    {
        [XmlElement("Zip5")]
        public string Zip5 { get; set; }
        [XmlElement("City")]
        public string City { get; set; }
        [XmlElement("State")]
        public string State { get; set; }
        [XmlAttribute("ID")]
        public string ID { get; set; }
    }
}
