using Newtonsoft.Json.Linq;

namespace Photoprint.Core.Models
{
    public class AutoSelectShippingInfo
    {
        public string Title { get; set; }
        public int? ShippingId { get; set; }
        public int? AddressId { get; set; }
        public AddressDTO Address { get; set; }
        public JObject Properties { get; set; }
    }

}