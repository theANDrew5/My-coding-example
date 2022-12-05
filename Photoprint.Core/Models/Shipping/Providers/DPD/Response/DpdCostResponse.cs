using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public class DpdCostResponse
    {
        [XmlElement(ElementName = "serviceCode")]
        public string ServiceСode { get; set; }

        [XmlElement(ElementName = "serviceName")]
        public string ServiceName { get; set; }

        [XmlElement(ElementName = "cost")]
        public decimal Cost { get; set; }

        [XmlElement(ElementName = "days")]
        public string Days { get; set; }
    }
}
