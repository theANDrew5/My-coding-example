using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("PTSEmailRequest")]
    public class UspsPTSEmailRequest
    {
        [XmlElement("TrackId")]
        public string TrackId { get; set; }
        [XmlElement("ClientIp")]
        public string ClientIp { get; set; }
        [XmlElement("SourceId")]
        public string SourceId { get; set; }
        [XmlElement("MpSuffix")]
        public string MpSuffix { get; set; }
        [XmlElement("MpDate")]
        public string MpDate { get; set; }
        [XmlElement("RequestType")]
        public string RequestType { get; set; }
        [XmlElement("FirstName")]
        public string FirstName { get; set; }
        [XmlElement("LastName")]
        public string LastName { get; set; }
        [XmlElement("Email1")]
        public string Email1 { get; set; }
        [XmlAttribute("USERID")]
        public string UserID { get; set; }
    }

}
