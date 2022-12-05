using System.Collections.Generic;
using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("USPSReturnsLabelResponse")]
    public class UspsReturnsLabelResponse
    {
        [XmlElement("BarcodeNumber")]
        public string BarcodeNumber { get; set; }
        [XmlElement("LabelImage")]
        public string LabelImage { get; set; }
        [XmlElement("RetailerFirm")]
        public string RetailerFirm { get; set; }
        [XmlElement("RetailerAddress1")]
        public string RetailerAddress1 { get; set; }
        [XmlElement("RetailerAddress2")]
        public string RetailerAddress2 { get; set; }
        [XmlElement("RetailerCity")]
        public string RetailerCity { get; set; }
        [XmlElement("RetailerState")]
        public string RetailerState { get; set; }
        [XmlElement("RetailerZip5")]
        public string RetailerZip5 { get; set; }
        [XmlElement("RetailerZip4")]
        public string RetailerZip4 { get; set; }
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
        [XmlElement("Fees")]
        public UspsFees Fees { get; set; }
        [XmlElement("Attributes")]
        public UspsAttributes Attributes { get; set; }
    }

    [XmlRoot("Fee")]
    public class UspsFee
    {
        [XmlElement("FeeType")]
        public string FeeType { get; set; }
        [XmlElement("FeePrice")]
        public string FeePrice { get; set; }
    }

    [XmlRoot("Fees")]
    public class UspsFees
    {
        [XmlElement("Fee")]
        public List<UspsFee> Fee { get; set; }
    }

    [XmlRoot("Attribute")]
    public class UspsAttribute
    {
        [XmlAttribute("Key")]
        public string Key { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot("Attributes")]
    public class UspsAttributes
    {
        [XmlElement("Attribute")]
        public UspsAttribute Attribute { get; set; }
    }

}
