using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("RateV4Request")]
    public class UspsRateV4Request
    {
        [XmlElement("Revision")]
        public int Revision { get; set; } = 2;

        [XmlElement("Package")]
        public UspsPackage Package { get; set; }

        [XmlAttribute( "USERID")]
        public string UserID { get; set; }

        [XmlAttribute( "PASSWORD")]
        public string Password { get; set; }
    }
}
