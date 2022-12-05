using System.Collections.Generic;
using System.Xml.Serialization;

namespace Photoprint.Core.Models
{

    public class RussianPostPostalOrderEventResponce
    {
        [XmlElement("PostalOrderEvent")]
        public List<RussianPostPostalOrderEvent> PostalOrderEvent { get; set; }
    }

    public class RussianPostPostalOrderEvent
    {
        [XmlAttribute("Number")]
        public string Number { get; set; }

        [XmlAttribute("EventDateTime")]
        public string EventDateTime { get; set; }

        [XmlAttribute("EventType")]
        public int EventType { get; set; }//https://tracking.pochta.ru/support/dictionaries/event_type

        [XmlAttribute("EventName")]
        public string EventName { get; set; }//https://tracking.pochta.ru/support/dictionaries/event_type

        [XmlAttribute("IndexTo")]
        public string IndexTo { get; set; }

        [XmlAttribute("IndexEvent")]
        public string IndexEvent { get; set; }

        [XmlAttribute("SumPaymentForward")]
        public string SumPaymentForward { get; set; }

        [XmlAttribute("CountryEventCode")]
        public string CountryEventCode { get; set; }

        [XmlAttribute("CountryToCode")]
        public string CountryToCode { get; set; }
    }
}
