using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("AddressValidateRequest")]
    public class UspsAddressValidateRequest
    {
        [XmlAttribute(AttributeName = "USERID")]
        public string UserID { get; set; }
        [XmlElement("Revision")]
        public int Revision { get; set; } = 1;
        [XmlElement("Address")]
        public UspsAddress Address { get; set; }
    }

}
