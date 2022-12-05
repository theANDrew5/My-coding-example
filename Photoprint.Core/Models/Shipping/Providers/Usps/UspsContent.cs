using System.Net.Mime;
using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("Content")]
    public class UspsContent
    {
        [XmlElement("ContentType")]
        public UspsContentType ContentType { get; set; }
        [XmlElement("ContentDescription")]
        public string ContentDescription { get; set; } = string.Empty;
    }

}
