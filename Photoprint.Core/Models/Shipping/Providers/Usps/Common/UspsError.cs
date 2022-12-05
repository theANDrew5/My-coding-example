using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("Error")]
    public class UspsError
    {
        [XmlElement("Number")]
        public string Number { get; set; }
        [XmlElement("Source")]
        public string Source { get; set; }
        [XmlElement("Description")]
        public string Description { get; set; }
        [XmlElement("HelpFile")]
        public string HelpFile { get; set; }
        [XmlElement("HelpContext")]
        public string HelpContext { get; set; }
    }

}
