using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot( "ExtraService")]
    public class UspsExtraService
    {
        [XmlElement( "ServiceID")]
        public string ServiceID { get; set; }
        [XmlElement( "ServiceName")]
        public string ServiceName { get; set; }
        [XmlElement( "Price")]
        public string Price { get; set; }
    }
}
