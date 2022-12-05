using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("USPSReturnsLabelRequest")]
    public class UspsReturnsLabelRequest
    {
        [XmlElement("Option")]
        public string Option { get; set; } = string.Empty;
        [XmlElement("Revision")]
        public string Revision { get; set; } = string.Empty;
        [XmlElement("ImageParameters")]
        public UspsImageParameters ImageParameters { get; set; } = new UspsImageParameters();
        [XmlElement("CustomerFirstName")]
        public string CustomerFirstName { get; set; } = string.Empty;
        [XmlElement("CustomerLastName")]
        public string CustomerLastName { get; set; } = string.Empty;
        [XmlElement("CustomerFirm")]
        public string CustomerFirm { get; set; } = string.Empty;
        [XmlElement("CustomerAddress1")]
        public string CustomerAddress1 { get; set; } = string.Empty;
        [XmlElement("CustomerAddress2")]
        public string CustomerAddress2 { get; set; } = string.Empty;
        [XmlElement("CustomerUrbanization")]
        public string CustomerUrbanization { get; set; } = string.Empty;
        [XmlElement("CustomerCity")]
        public string CustomerCity { get; set; } = string.Empty;
        [XmlElement("CustomerState")]
        public string CustomerState { get; set; } = string.Empty;
        [XmlElement("CustomerZip5")]
        public string CustomerZip5 { get; set; } = string.Empty;
        [XmlElement("CustomerZip4")]
        public string CustomerZip4 { get; set; } = string.Empty;
        [XmlElement("POZipCode")]
        public string POZipCode { get; set; } = string.Empty;
        [XmlElement("AllowNonCleansedOriginAddr")]
        public string AllowNonCleansedOriginAddr { get; set; } = string.Empty;
        [XmlElement("RetailerATTN")]
        public string RetailerATTN { get; set; } = string.Empty;
        [XmlElement("RetailerFirm")]
        public string RetailerFirm { get; set; } = string.Empty;
        [XmlElement("WeightInOunces")]
        public string WeightInOunces { get; set; } = string.Empty;
        [XmlElement("ServiceType")]
        public string ServiceType { get; set; } = string.Empty;
        [XmlElement("Width")]
        public string Width { get; set; } = string.Empty;
        [XmlElement("Length")]
        public string Length { get; set; } = string.Empty;
        [XmlElement("Height")]
        public string Height { get; set; } = string.Empty;
        [XmlElement("Girth")]
        public string Girth { get; set; } = string.Empty;
        [XmlElement("Machinable")]
        public string Machinable { get; set; } = string.Empty;
        [XmlElement("CustomerRefNo")]
        public string CustomerRefNo { get; set; } = string.Empty;
        [XmlElement("PrintCustomerRefNo")]
        public string PrintCustomerRefNo { get; set; } = string.Empty;
        [XmlElement("CustomerRefNo2")]
        public string CustomerRefNo2 { get; set; } = string.Empty;
        [XmlElement("PrintCustomerRefNo2")]
        public string PrintCustomerRefNo2 { get; set; } = string.Empty;
        [XmlElement("SenderName")]
        public string SenderName { get; set; } = string.Empty;
        [XmlElement("SenderEmail")]
        public string SenderEmail { get; set; } = string.Empty;
        [XmlElement("RecipientName")]
        public string RecipientName { get; set; } = string.Empty;
        [XmlElement("RecipientEmail")]
        public string RecipientEmail { get; set; } = string.Empty;
        [XmlElement("TrackingEmailPDF")]
        public string TrackingEmailPDF { get; set; } = string.Empty;
        [XmlElement("ExtraServices")]
        public UspsExtraServices ExtraServices { get; set; } = new UspsExtraServices();
        [XmlAttribute( "USERID")]
        public string UserID { get; set; } = string.Empty;
    }

}
