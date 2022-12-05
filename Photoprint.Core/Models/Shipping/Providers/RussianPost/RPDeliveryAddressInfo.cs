
namespace Photoprint.Core.Models
{
    public class RussianPostAddressInfo
    {
        public UserPostOffice PostalFrom { get; set; }
        public RussianPostAddressType AddressType { get; set;}
        public RussianPostMailType MailType { get; set;}
        public RussianPostMailCategory MailCategory { get; set;}
    }
}
