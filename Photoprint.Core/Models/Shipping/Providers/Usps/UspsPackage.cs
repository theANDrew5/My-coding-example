using System.Collections.Generic;
using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("Package")]
    public class UspsPackage
    {
        [XmlAttribute("ID")]
        public string ID { get; set; }
        [XmlElement("Service")]
        public UspsServiceType Service { get; set; }
        [XmlElement("ZipOrigination")]
        public string ZipOrigination { get; set; }
        [XmlElement("ZipDestination")]
        public string ZipDestination { get; set; }
        [XmlElement("Pounds")]
        public double Pounds { get; set; }
        [XmlElement("Ounces")]
        public double Ounces { get; set; }
        [XmlElement("Container")]
        public UspsContainerType Container { get; set; }
        [XmlElement("Width")]
        public string Width { get; set; }
        [XmlElement("Length")]
        public string Length { get; set; }
        [XmlElement("Height")]
        public string Height { get; set; }
        [XmlElement("Girth")]
        public string Girth { get; set; }

        [XmlElement("Postage")]
        public List<UspsPostage> Postages { get; set; }

        [XmlElement("Error")]
        public UspsError Error { get; set; }
    }
}
