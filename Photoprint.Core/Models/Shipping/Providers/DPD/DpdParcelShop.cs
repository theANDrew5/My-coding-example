using System.Collections.Generic;
using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public class DpdParcelShop
    {
        [XmlElement(ElementName = "code")]
        public string Code { get; set; }

        [XmlElement(ElementName = "parcelShopType")]
        public string ParcelShopType { get; set; }

        [XmlElement(ElementName = "state")]
        public string State { get; set; }

        [XmlElement(ElementName = "address")]
        public DpdAddress Address { get; set; }

        [XmlElement(ElementName = "geoCoordinates")]
        public DpdGeoCoordinates GeoCoordinates { get; set; }

        [XmlElement(ElementName = "limits")]
        public DpdLimits Limits { get; set; }

        [XmlElement(ElementName = "schedule")]
        public DpdSchedule Schedule { get; set; }

        [XmlElement(ElementName = "extraService")]
        public string[] ExtraService { get; set; }

        [XmlElement(ElementName = "services")]
        public List<DpdService> Services { get; set; }
    }
}
