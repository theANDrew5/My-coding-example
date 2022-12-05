using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("PTSEMAILRESULT")]
    public class UspsPTSemailResponse
    {
        [XmlElement("ResultText")]
        public string ResultText { get; set; }
        [XmlElement("ReturnCode")]
        public string ReturnCode { get; set; }
    }

}
