using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("LabelSequence")]
    public class UspsLabelSequence
    {
        [XmlElement("PackageNumber")]
        public string PackageNumber { get; set; } = string.Empty;
        [XmlElement("TotalPackages")]
        public string TotalPackages { get; set; } = string.Empty;
    }

}
