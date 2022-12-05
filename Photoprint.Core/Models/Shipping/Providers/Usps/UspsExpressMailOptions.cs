using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("ExpressMailOptions")]
    public class UspsExpressMailOptions
    {
        [XmlElement("DeliveryOption")]
        public string DeliveryOption { get; set; } = string.Empty;
        [XmlElement("WaiverOfSignature")]
        public string WaiverOfSignature { get; set; } = string.Empty;
    }

}
