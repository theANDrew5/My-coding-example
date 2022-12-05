using System.Collections.Generic;
using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public class DpdOrderStateResponse
    {
        [XmlElement(ElementName = "docId")]
        public string DocId { get; set; }

        [XmlElement(ElementName = "docDate")]
        public string DocDate { get; set; }

        [XmlElement(ElementName = "clientNumber")]
        public string ClientNumber { get; set; }

        [XmlElement(ElementName = "resultComplete")]
        public bool ResultComplete { get; set; }

        [XmlElement(ElementName = "states")]
        public List<DpdState> States { get; set; }
    }
}
