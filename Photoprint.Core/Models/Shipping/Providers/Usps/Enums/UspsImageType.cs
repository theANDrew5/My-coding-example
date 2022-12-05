using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public enum UspsImageType
    {
        [XmlEnum("NONE")] NONE,
        [XmlEnum("PDF")] PDF,
        [XmlEnum("TIF")] TIF,
    }
}
