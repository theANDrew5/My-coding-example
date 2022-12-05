using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("TrackFieldRequest")]
    public class UspsTrackFieldRequest
    {
        [XmlElement("Revision")]
        public string Revision { get; set; }
        [XmlElement("ClientIp")]
        public string ClientIp { get; set; }
        [XmlElement("SourceId")]
        public string SourceId { get; set; }
        [XmlElement("TrackID")]
        public UspsTrackID TrackID { get; set; }
        [XmlAttribute("USERID")]
        public string USERID { get; set; }
    }

}
