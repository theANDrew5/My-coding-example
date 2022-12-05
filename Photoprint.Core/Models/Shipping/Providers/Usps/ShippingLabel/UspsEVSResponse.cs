using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("eVSResponse")]
    public class UspsEVSResponse
    {
        [XmlElement("BarcodeNumber")]
        public string BarcodeNumber { get; set; }
        [XmlElement("LabelImage")]
        public string LabelImage { get; set; }
        [XmlElement("ToName")]
        public string ToName { get; set; }
        [XmlElement("ToFirm")]
        public string ToFirm { get; set; }
        [XmlElement("ToAddress1")]
        public string ToAddress1 { get; set; }
        [XmlElement("ToAddress2")]
        public string ToAddress2 { get; set; }
        [XmlElement("ToCity")]
        public string ToCity { get; set; }
        [XmlElement("ToState")]
        public string ToState { get; set; }
        [XmlElement("ToZip5")]
        public string ToZip5 { get; set; }
        [XmlElement("ToZip4")]
        public string ToZip4 { get; set; }
        [XmlElement("Postnet")]
        public string Postnet { get; set; }
        [XmlElement("RDC")]
        public string RDC { get; set; }
        [XmlElement("Postage")]
        public string Postage { get; set; }
        [XmlElement("ExtraServices")]
        public UspsExtraServices ExtraServices { get; set; }
        [XmlElement("Zone")]
        public string Zone { get; set; }
        [XmlElement("CarrierRoute")]
        public string CarrierRoute { get; set; }
        [XmlElement("PermitHolderName")]
        public string PermitHolderName { get; set; }
        [XmlElement("InductionType")]
        public string InductionType { get; set; }
        [XmlElement("LogMessage")]
        public string LogMessage { get; set; }
        [XmlElement("Commitment")]
        public UspsCommitment Commitment { get; set; }
    }

}
