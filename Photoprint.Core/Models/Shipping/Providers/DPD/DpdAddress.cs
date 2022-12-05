using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public class DpdAddress
    {
        [XmlElement(ElementName = "cityId")]
        public string CityId { get; set; }
        
        [XmlElement(ElementName = "countryCode")]
        public string CountryCode { get; set; }
        
        [XmlElement(ElementName = "regionCode")]
        public string RegionCode { get; set; }
        
        [XmlElement(ElementName = "regionName")]
        public string RegionName { get; set; }
        
        [XmlElement(ElementName = "cityCode")]
        public string CityCode { get; set; }
        
        [XmlElement(ElementName = "cityName")]
        public string CityName { get; set; }
        
        [XmlElement(ElementName = "street")]
        public string Street { get; set; }
        
        [XmlElement(ElementName = "streetAbbr")]
        public string StreetAbbr { get; set; }
        
        [XmlElement(ElementName = "houseNo")]
        public string HouseNo { get; set; }
        
        [XmlElement(ElementName = "building")]
        public string Building { get; set; }
        
        [XmlElement(ElementName = "structure")]
        public string Structure { get; set; }
        
        [XmlElement(ElementName = "ownership")]
        public string Ownership { get; set; }
        
        [XmlElement(ElementName = "descript")]
        public string Descript { get; set; }
    }
}
