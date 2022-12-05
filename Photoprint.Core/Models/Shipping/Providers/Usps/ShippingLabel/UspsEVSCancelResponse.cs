using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("eVSCancelResponse")]
    public class UspsEVSCancelResponse
    {
        [XmlElement("BarcodeNumber")]
        public string BarcodeNumber { get; set; }
        [XmlElement("Status")]
        public string Status { get; set; }
        [XmlElement("Reason")]
        public string Reason { get; set; }
    }

}
