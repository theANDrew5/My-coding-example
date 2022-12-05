using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public class DpdOrderRegStatusResponse
    {
        [XmlElement(ElementName = "orderNumberInternal")]
        public string OrderNumberInternal { get; set; }
        
        [XmlElement(ElementName = "orderNum")]
        public string OrderNum { get; set; }
        
        [XmlElement(ElementName = "status")]
        public DpdOrderRegStatus Status { get; set; }
        
        [XmlElement(ElementName = "errorMessage")]
        public string ErrorMessage { get; set; }
    }
}
