using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("Commitment")]
    public class UspsCommitment
    {
        [XmlElement("CommitmentName")]
        public string CommitmentName { get; set; }
        [XmlElement("ScheduledDeliveryDate")]
        public string ScheduledDeliveryDate { get; set; }
    }
}
