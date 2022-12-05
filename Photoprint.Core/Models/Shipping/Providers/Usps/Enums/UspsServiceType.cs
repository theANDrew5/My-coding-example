using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public enum UspsServiceType
    {
        [XmlEnum("PRIORITY")] PRIORITY,
        [XmlEnum("PRIORITY EXPRESS")] PRIORITY_EXPRESS,
        [XmlEnum("IRST CLASS")] IRST_CLASS,
        [XmlEnum("IBRARY")] IBRARY,
        [XmlEnum("EDIA")] EDIA,
        [XmlEnum("BPM")] BPM
    }
}
