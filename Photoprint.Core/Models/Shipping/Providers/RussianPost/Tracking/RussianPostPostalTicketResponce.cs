using System.Xml.Serialization;

namespace Photoprint.Core.Models
{

    public class RussianPostPostalTicketResponce
    {
        [XmlElement("Value")]
        public string Value { get; set; }

        [XmlElement("Error")]
        public (int ErrorTypeID, string ErrorName) Error { get; set; }
    }
}
