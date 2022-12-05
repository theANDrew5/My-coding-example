using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("CityStateLookupRequest")]
    public class UspsCityStateLookupRequest
    {
        [XmlElement("ZipCode")]
        public UspsZipCode ZipCode { get; set; }
        [XmlAttribute("USERID")]
        public string UserID { get; set; }
    }
}
