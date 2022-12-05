using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public class DpdLimits
    {
        [XmlElement(ElementName = "maxShipmentWeight")]
        public string MaxShipmentWeight { get; set; }
        
        [XmlElement(ElementName = "maxWeight")]
        public string MaxWeight { get; set; }
        
        [XmlElement(ElementName = "maxLength")]
        public string MaxLength { get; set; }
        
        [XmlElement(ElementName = "maxWidth")]
        public string MaxWidth { get; set; }
        
        [XmlElement(ElementName = "maxHeight")]
        public string MaxHeight { get; set; }
        
        [XmlElement(ElementName = "dimensionSum")]
        public string DimensionSum { get; set; }
    }
}
