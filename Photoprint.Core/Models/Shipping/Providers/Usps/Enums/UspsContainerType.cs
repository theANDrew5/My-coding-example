using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public enum UspsContainerType
    {
        [XmlEnum("VARIABLE")] VARIABLE,
    }
}
