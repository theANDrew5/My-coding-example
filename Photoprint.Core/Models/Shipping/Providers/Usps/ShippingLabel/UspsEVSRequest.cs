using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("eVSRequest")]
    public class UspsEVSRequest
    {
        [XmlElement("Option")]
        public string Option { get; set; } = string.Empty;
        [XmlElement("Revision")]
        public string Revision { get; set; } = "2";
        [XmlElement("ImageParameters")]
        public UspsImageParameters ImageParameters { get; set; } = new UspsImageParameters();
        [XmlElement("FromName")]
        public string FromName { get; set; } = string.Empty;
        [XmlElement("FromFirm")]
        public string FromFirm { get; set; } = string.Empty;
        [XmlElement("FromAddress1")]
        public string FromAddress1 { get; set; } = string.Empty;
        [XmlElement("FromAddress2")]
        public string FromAddress2 { get; set; } = string.Empty;
        [XmlElement("FromCity")]
        public string FromCity { get; set; } = string.Empty;
        [XmlElement("FromState")]
        public string FromState { get; set; } = string.Empty;
        [XmlElement("FromZip5")]
        public string FromZip5 { get; set; } = string.Empty;
        [XmlElement("FromZip4")]
        public string FromZip4 { get; set; } = string.Empty;
        [XmlElement("FromPhone")]
        public string FromPhone { get; set; } = string.Empty;
        [XmlElement("POZipCode")]
        public string POZipCode { get; set; } = string.Empty;
        [XmlElement("AllowNonCleansedOriginAddr")]
        public bool AllowNonCleansedOriginAddr { get; set; }
        [XmlElement("ToName")]
        public string ToName { get; set; } = string.Empty;
        [XmlElement("ToFirm")]
        public string ToFirm { get; set; } = string.Empty;
        [XmlElement("ToAddress1")]
        public string ToAddress1 { get; set; } = string.Empty;
        [XmlElement("ToAddress2")]
        public string ToAddress2 { get; set; } = string.Empty;
        [XmlElement("ToCity")]
        public string ToCity { get; set; } = string.Empty;
        [XmlElement("ToState")]
        public string ToState { get; set; } = string.Empty;
        [XmlElement("ToZip5")]
        public string ToZip5 { get; set; } = string.Empty;
        [XmlElement("ToZip4")]
        public string ToZip4 { get; set; } = string.Empty;
        [XmlElement("ToPhone")]
        public string ToPhone { get; set; } = string.Empty;
        [XmlElement("POBox")]
        public string POBox { get; set; } = string.Empty;
        [XmlElement("AllowNonCleansedDestAddr")]
        public string AllowNonCleansedDestAddr { get; set; } = string.Empty;
        [XmlElement("WeightInOunces")]
        public double WeightInOunces { get; set; }
        [XmlElement("ServiceType")]
        public UspsServiceType ServiceType { get; set; }
        [XmlElement("Container")]
        public UspsContainerType Container { get; set; }
        [XmlElement("Width")]
        public string Width { get; set; } = string.Empty;
        [XmlElement("Length")]
        public string Length { get; set; } = string.Empty;
        [XmlElement("Height")]
        public string Height { get; set; } = string.Empty;
        [XmlElement("Machinable")]
        public string Machinable { get; set; } = string.Empty;
        [XmlElement("ProcessingCategory")]
        public string ProcessingCategory { get; set; } = string.Empty;
        [XmlElement("PriceOptions")]
        public string PriceOptions { get; set; } = string.Empty;
        [XmlElement("InsuredAmount")]
        public string InsuredAmount { get; set; } = string.Empty;
        [XmlElement("AddressServiceRequested")]
        public string AddressServiceRequested { get; set; } = string.Empty;
        [XmlElement("ExpressMailOptions")]
        public UspsExpressMailOptions ExpressMailOptions { get; set; } = new UspsExpressMailOptions();
        [XmlElement("ShipDate")]
        public string ShipDate { get; set; } = string.Empty;
        [XmlElement("CustomerRefNo")]
        public string CustomerRefNo { get; set; } = string.Empty;
        [XmlElement("ExtraServices")]
        public UspsExtraServices ExtraServices { get; set; } = new UspsExtraServices();
        [XmlElement("HoldForPickup")]
        public string HoldForPickup { get; set; } = string.Empty;
        [XmlElement("OpenDistribute")]
        public string OpenDistribute { get; set; } = string.Empty;
        [XmlElement("PermitNumber")]
        public string PermitNumber { get; set; } = string.Empty;
        [XmlElement("PermitZIPCode")]
        public string PermitZIPCode { get; set; } = string.Empty;
        [XmlElement("PermitHolderName")]
        public string PermitHolderName { get; set; } = string.Empty;
        [XmlElement("SenderName")]
        public string SenderName { get; set; } = string.Empty;
        [XmlElement("SenderEMail")]
        public string SenderEMail { get; set; } = string.Empty;
        [XmlElement("RecipientName")]
        public string RecipientName { get; set; } = string.Empty;
        [XmlElement("RecipientEMail")]
        public string RecipientEMail { get; set; } = string.Empty;
        [XmlElement("ReceiptOption")]
        public string ReceiptOption { get; set; } = string.Empty;
        [XmlElement("ImageType")]
        public UspsImageType ImageType { get; set; }
        [XmlElement("HoldForManifest")]
        public string HoldForManifest { get; set; } = string.Empty;
        [XmlElement("NineDigitRoutingZip")]
        public string NineDigitRoutingZip { get; set; } = string.Empty;
        [XmlElement("ShipInfo")]
        public string ShipInfo { get; set; } = string.Empty;
        [XmlElement("CarrierRelease")]
        public string CarrierRelease { get; set; } = string.Empty;
        [XmlElement("ReturnCommitments")]
        public string ReturnCommitments { get; set; } = string.Empty;
        [XmlElement("PrintCustomerRefNo")]
        public string PrintCustomerRefNo { get; set; } = string.Empty;
        [XmlElement("Content")]
        public UspsContent Content { get; set; }
        [XmlElement("ShippingContents")]
        public UspsShippingContents ShippingContents { get; set; } = new UspsShippingContents();
        [XmlElement("CustomsContentType")]
        public UspsCustomsContentType CustomsContentType { get; set; }
        [XmlElement("ContentComments")]
        public string ContentComments { get; set; } = string.Empty;
        [XmlElement("RestrictionType")]
        public string RestrictionType { get; set; } = string.Empty;
        [XmlElement("RestrictionComments")]
        public string RestrictionComments { get; set; } = string.Empty;
        [XmlElement("AESITN")]
        public string AESITN { get; set; } = string.Empty;
        [XmlElement("ImportersReference")]
        public string ImportersReference { get; set; } = string.Empty;
        [XmlElement("ImportersContact")]
        public string ImportersContact { get; set; } = string.Empty;
        [XmlElement("ExportersReference")]
        public string ExportersReference { get; set; } = string.Empty;
        [XmlElement("ExportersContact")]
        public string ExportersContact { get; set; } = string.Empty;
        [XmlElement("InvoiceNumber")]
        public string InvoiceNumber { get; set; } = string.Empty;
        [XmlElement("LicenseNumber")]
        public string LicenseNumber { get; set; } = string.Empty;
        [XmlElement("CertificateNumber")]
        public string CertificateNumber { get; set; } = string.Empty;
        [XmlAttribute("USERID")]
        public string UserID { get; set; } = string.Empty;
    }

}
