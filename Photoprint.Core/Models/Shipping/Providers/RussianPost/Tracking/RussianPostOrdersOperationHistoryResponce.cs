using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public class RussianPostOrdersOperationHistoryResponce
    {
        [XmlElement("Value")]
        public string Value { get; set; }

        [XmlElement("Item")]
        public RussianPostOperationHistoryItem Item { get; set; }
    }

    public class RussianPostOperationHistoryItem
    {
        [XmlAttribute("Barcode")]
        public string Barcode { get; set; }

        [XmlElement("Operation")]
        public RussianPostOperation Operation { get; set; }

        [XmlElement("Error")]
        public (int ErrorTypeID, string ErrorName) Error { get; set; }
    }

    public class RussianPostOperation
    {
        [XmlAttribute("OperTypeID")]
        public string OperTypeID { get; set; }

        [XmlAttribute("OperCtgID")]
        public string OperCtgID { get; set; }

        [XmlAttribute("OperName")]
        public string OperName { get; set; }

        [XmlAttribute("DateOper")]
        public string DateOper { get; set; }

        [XmlAttribute("IndexOper")]
        public string IndexOper { get; set; }
    }
}
