using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("TrackResponse")]
    public class UspsTrackResponse
    {
        [XmlElement("Status")]
        public string Status { get; set; }

        [XmlElement("StatusSummary")]
        public string StatusSummary { get; set; }

        [XmlElement("Error")]
        public UspsError Error { get; set; }
    }

}
