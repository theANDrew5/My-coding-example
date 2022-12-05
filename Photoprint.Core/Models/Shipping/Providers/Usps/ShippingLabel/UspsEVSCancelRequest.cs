using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("eVSCancelRequest")]
    public class UspsEVSCancelRequest
    {
        [XmlElement("BarcodeNumber")]
        public string BarcodeNumber { get; set; }
        [XmlAttribute("USERID")]
        public string UserID { get; set; }
    }

}
