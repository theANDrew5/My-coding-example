using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public class DpdSchedule
    {
        [XmlElement(ElementName = "operation")]
        public string Operation { get; set; }
        
        [XmlElement(ElementName = "timetable")]
        public string Timetable { get; set; }
    }
}
