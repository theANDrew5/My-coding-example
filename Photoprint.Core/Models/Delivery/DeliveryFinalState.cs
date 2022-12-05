using Newtonsoft.Json.Linq;

namespace Photoprint.Core.Models
{

    public class DeliveryFinalState
    {
        public ShippingAddressDTO ShippingData { get; set; }
        public DeliveryFinalStateUserData UserData { get; set; }
    }

    public class DeliveryFinalStateUserData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string OrderCommentary { get; set; }
        public string AdditionalEmail { get; set; }
        public string AdditionalPhone { get; set; }
        public bool FromUserCompany { get; set; }
    }

}
