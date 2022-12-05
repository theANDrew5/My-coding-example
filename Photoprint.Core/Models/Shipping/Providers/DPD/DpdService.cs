using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public class DpdService
    {
        [XmlElement(ElementName = "serviceCode")]
        public string ServiceCode { get; set; }
        
        [XmlElement(ElementName = "days")]
        public string Days { get; set; }
    }
}
