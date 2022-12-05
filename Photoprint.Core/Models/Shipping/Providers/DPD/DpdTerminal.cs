using System.Collections.Generic;
using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public class DpdTerminal
    {
        [XmlElement(ElementName = "terminalCоde")]
        public string TerminalCоde { get; set; }

        [XmlElement(ElementName = "terminalName")]
        public string TerminalName { get; set; }

        [XmlElement(ElementName = "geoCoordinates")]
        public DpdGeoCoordinates GeoCoordinates { get; set; }

        [XmlElement(ElementName = "address")]
        public DpdAddress Address { get; set; }

        [XmlElement(ElementName = "schedule")]
        public DpdSchedule Schedule { get; set; }

        [XmlElement(ElementName = "services")]
        public List<DpdService> Services { get; set; }
    }
}
