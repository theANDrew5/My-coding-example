using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("RateV4Response")]
    public class UspsRateV4Response
    {
        [XmlElement("Package")]
        public UspsPackage Package { get; set; }
    }

}
