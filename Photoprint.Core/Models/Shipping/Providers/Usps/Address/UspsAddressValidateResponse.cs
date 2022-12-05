using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("AddressValidateResponse")]
    public class UspsAddressValidateResponse
    {
        [XmlElement("Address")]
        public UspsAddress Address { get; set; }
    }
}
