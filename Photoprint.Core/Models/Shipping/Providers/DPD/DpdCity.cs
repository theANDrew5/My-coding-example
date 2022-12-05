using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public class DpdCity
    {
        [XmlElement(ElementName = "cityId")]
        public long CityId { get; set; }

        [XmlElement(ElementName = "countryCode")]
        public string CountryCode { get; set; }

        [XmlElement(ElementName = "countryName")]
        public string CountryName { get; set; }

        [XmlElement(ElementName = "regionCode")]
        public int RegionCode { get; set; }

        [XmlElement(ElementName = "regionName")]
        public string RegionName { get; set; }

        [XmlElement(ElementName = "cityCode")]
        public string CityCode { get; set; }

        [XmlElement(ElementName = "cityName")]
        public string CityName { get; set; }

        [XmlElement(ElementName = "abbreviation")]
        public string Abbreviation { get; set; }

        [XmlElement(ElementName = "indexMin")]
        public string IndexMin { get; set; }

        [XmlElement(ElementName = "indexMax")]
        public string IndexMax { get; set; }
    }

}
