using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot(ElementName = "getOperationHistoryResponse", Namespace = "http://russianpost.org/operationhistory")]
    public class RussianPostOperationHistoryResponce
    {
        [XmlElement(ElementName = "OperationHistoryData", Namespace = "http://russianpost.org/operationhistory/data")]
        public RussianPostHistoryData Data { get; set; }
    }

    
    public class RussianPostHistoryData
    {
        [XmlElement("historyRecord")]
        public List<RussianPostHistoryRecord> Records  { get; set; }
    }
    public class RussianPostHistoryRecord
    {
        [XmlElement("AddressParameters")]
        public RussianPostAddressParameters AddressParameters { get; set; }

        [XmlElement("FinanceParameters")]
        public RussianPostFinanceParameters FinanceParameters { get; set; }

        [XmlElement("ItemParameters")]
        public RussianPostItemParameters ItemParameters { get; set; }

        [XmlElement("OperationParameters")]
        public RussianPostOperationParameters OperationParameters { get; set; }

        [XmlElement("UserParameters")]
        public RussianPostUserParameters UserParameters { get; set; }
    }

    public class RussianPostAddressParameters
    {
        [XmlElement("DestinationAddress")]
        public RussianPostDestinationAddress DestinationAddress { get; set; }

        [XmlElement("OperationAddress")]
        public RussianPostOperationAddress OperationAddress { get; set; }

        [XmlElement("MailDirect")]
        public RussianPostCountryData MailDirect { get; set; }

        [XmlElement("CountryFrom")]
        public RussianPostCountryData CountryFrom { get; set; }

        [XmlElement("CountryOper")]
        public RussianPostCountryData CountryOper { get; set; }
    }

    public class RussianPostDestinationAddress
    {
        [XmlElement("Index")]
        public int Index { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }
    }

    public class RussianPostOperationAddress
    {
        [XmlElement("Index")]
        public int Index { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }
    }

    public class RussianPostCountryData
    {
        [XmlElement("Id")]
        public string Id { get; set; }

        [XmlElement("Code2A")]
        public string Code2A { get; set; }

        [XmlElement("Code3A")]
        public string Code3A { get; set; }

        [XmlElement("NameRu")]
        public string NameRu { get; set; }

        [XmlElement("NameEN")]
        public string NameEN { get; set; }
    }

    public class RussianPostFinanceParameters
    {
        [XmlElement("Payment")]
        public string Payment { get; set; }

        [XmlElement("Value")]
        public string Value { get; set; }

        [XmlElement("MassRate")]
        public string MassRate { get; set; }

        [XmlElement("InsrRate")]
        public string InsrRate { get; set; }

        [XmlElement("AirRate")]
        public string AirRate { get; set; }

        [XmlElement("Rate")]
        public string Rate { get; set; }

        [XmlElement("CustomDuty")]
        public string CustomDuty { get; set; }
    }

    public class RussianPostItemParameters
    {
        [XmlElement("Barcode")]
        public string Barcode { get; set; }

        [XmlElement("Internum")]
        public string Internum { get; set; }

        [XmlElement("ValidRuType")]
        public string ValidRuType { get; set; }

        [XmlElement("ValidEnType")]
        public string ValidEnType { get; set; }

        [XmlElement("ComplexItemName")]
        public string ComplexItemName { get; set; }

        [XmlElement("MailRank")]
        public RussianPostOper MailRank { get; set; }

        [XmlElement("PostMark")]
        public RussianPostOper PostMark { get; set; }

        [XmlElement("MailType")]
        public RussianPostOper MailType { get; set; }

        [XmlElement("MailCtg")]
        public RussianPostOper MailCtg { get; set; }

        [XmlElement("Mass")]
        public int Mass { get; set; }

        [XmlElement("MaxMassRu")]
        public int MaxMassRu { get; set; }

        [XmlElement("MaxMassEn")]
        public int MaxMassEn { get; set; }
    }

    public class RussianPostOper
    {
        [XmlElement("Id")]
        public int Id { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
    }

    public class RussianPostOperationParameters
    {
        [XmlElement("OperType")]
        public RussianPostOper OperType { get; set; }

        [XmlElement("OperAttr")]
        public RussianPostOper OperAttr { get; set; }

        [XmlElement("OperDate")]
        public DateTime OperDate { get; set; }
    }

    public class RussianPostUserParameters
    {
        [XmlElement("SendCtg")]
        public RussianPostOper SendCtg { get; set; }

        [XmlElement("Sndr")]
        public string Sndr { get; set; }

        [XmlElement("Rcpn")]
        public string Rcpn { get; set; }
    }
}
