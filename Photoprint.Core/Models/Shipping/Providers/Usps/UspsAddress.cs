using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("Address")]
    public class UspsAddress
    {
        [XmlElement("Address1")]
        public string Address1 { get; set; } = string.Empty;
        [XmlElement("Address2")]
        public string Address2 { get; set; }
        [XmlElement("City")]
        public string City { get; set; } = string.Empty;
        [XmlElement("CityAbbreviation")]
        public string CityAbbreviation { get; set; }
        [XmlElement("State")]
        public string State { get; set; } = string.Empty;
        [XmlElement("Zip5")]
        public string Zip5 { get; set; } = string.Empty;
        [XmlElement("Zip4")]
        public string Zip4 { get; set; } = string.Empty;
        [XmlElement("DeliveryPoint")]
        public string DeliveryPoint { get; set; }
        [XmlElement("CarrierRoute")]
        public string CarrierRoute { get; set; }
        [XmlElement("Footnotes")]
        public string Footnotes { get; set; }
        [XmlElement("DPVConfirmation")]
        public string DPVConfirmation { get; set; }
        [XmlElement("DPVCMRA")]
        public string DPVCMRA { get; set; }
        [XmlElement("DPVFootnotes")]
        public string DPVFootnotes { get; set; }
        [XmlElement("Business")]
        public string Business { get; set; }
        [XmlElement("CentralDeliveryPoint")]
        public string CentralDeliveryPoint { get; set; }
        [XmlElement("Vacant")]
        public string Vacant { get; set; }

        [XmlElement("ReturnText")]
        public string ReturnText { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }

        [XmlElement("Error")]
        public UspsError Error { get; set; }
    }
}
