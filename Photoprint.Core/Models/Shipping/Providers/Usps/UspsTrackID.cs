using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot( "TrackID")]
    public class UspsTrackID
    {
        [XmlAttribute("ID")]
        public string ID { get; set; }
    }
}
