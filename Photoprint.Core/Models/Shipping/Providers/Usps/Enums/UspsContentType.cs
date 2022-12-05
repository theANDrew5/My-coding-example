using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    public enum UspsContentType
    {
        [XmlEnum("Lives")] Lives,
        [XmlEnum("HAZMAT")] HAZMAT,
        [XmlEnum("CrematedRemains")] CrematedRemains,
        [XmlEnum("Perishable")] Perishable,
        [XmlEnum("Pharmaceuticals")] Pharmaceuticals,
        [XmlEnum("MedicalSupplies")] MedicalSupplies,
    }
}
