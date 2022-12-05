using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public enum UspsCustomsContentType
    {
        [XmlEnum("MERCHANDISE")] MERCHANDISE,
        [XmlEnum("GIFT")] GIFT,
        [XmlEnum("DOCUMENTS")] DOCUMENTS,
        [XmlEnum("SAMPLE")] SAMPLE,
        [XmlEnum("RETURN")] RETURN,
        [XmlEnum("OTHER")] OTHER,
        [XmlEnum("HUMANITARIAN")] HUMANITARIAN,
        [XmlEnum("DANGEROUSGOODS")] DANGEROUSGOODS,

    }
}
