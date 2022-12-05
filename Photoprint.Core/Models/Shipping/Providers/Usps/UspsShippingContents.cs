using System.Collections.Generic;
using System.Xml.Serialization;

namespace Photoprint.Core.Models
{
    [XmlRoot("ShippingContents")]
    public class UspsShippingContents
    {
        [XmlElement("ItemDetail")]
        public List<UspsItemDetail> ItemDetail { get; set; }
    }

}
