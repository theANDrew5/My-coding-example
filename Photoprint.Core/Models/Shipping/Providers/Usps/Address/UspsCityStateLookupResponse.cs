using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("CityStateLookupResponse")]
    public class UspsCityStateLookupResponse
    {
        [XmlElement("ZipCode")]
        public UspsZipCode ZipCode { get; set; }
    }
}
