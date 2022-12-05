using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public class DpdGeoCoordinates
    {
        [XmlElement(ElementName = "latitude")]
        public string Latitude { get; set; }
       
        [XmlElement(ElementName = "longitude")]
        public string Longitude { get; set; }
    }
}
