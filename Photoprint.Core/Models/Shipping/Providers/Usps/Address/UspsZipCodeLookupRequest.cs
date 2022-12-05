using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("ZipCodeLookupRequest")]
    public class UspsZipCodeLookupRequest
    {
        [XmlElement("Address")]
        public UspsAddress Address { get; set; }
        [XmlAttribute("USERID")]
        public string UserID { get; set; }
    }
}
