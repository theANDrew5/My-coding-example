using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("Postage")]
    public class UspsPostage
    {
        [XmlElement("MailService")]
        public string MailService { get; set; }
        [XmlElement("Rate")]
        public decimal Rate { get; set; }
        [XmlAttribute("CLASSID")]
        public string ClassID { get; set; }
        [XmlElement("Zone")]
        public string Zone { get; set; }
    }
}
