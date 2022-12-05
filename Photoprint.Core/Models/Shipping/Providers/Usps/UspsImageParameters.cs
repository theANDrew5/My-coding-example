using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("ImageParameters")]
    public class UspsImageParameters
    {
        [XmlElement("LabelSequence")]
        public UspsLabelSequence LabelSequence { get; set; }

        [XmlElement("ImageType")]
        public UspsImageType ImageType { get; set; }

        [XmlElement("SeparateReceiptPage")]
        public bool SeparateReceiptPage { get; set; }
    }

}
