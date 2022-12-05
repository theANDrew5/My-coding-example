using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("ZipCodeLookupResponse")]
    public class UspsZipCodeLookupResponse
    {
        [XmlElement("Address")]
        public UspsAddress Address { get; set; }
    }
}
