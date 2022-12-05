using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("ExtraServices")]
    public class UspsExtraServices
    {
        [XmlElement("ExtraService")]
        public UspsExtraService ExtraService { get; set; }
    }
}
