using Photoprint.Core.Models;

namespace Photoprint.WebSite.Admin.API.Models
{
    public class ShippingDTO
    {
        public string Title { get; set; }
        public int Value { get; set; }
        public string Address { get; set; }
        public int  AddressId { get; set; }
        public ShippingDTO(Shipping shipping, SystemLanguage language)
        {
            Title = shipping.AdminTitle;
            Value = shipping.Id;
            var distributionPoint = shipping as DistributionPoint;
            if (distributionPoint == null) return;
            Address = distributionPoint.ToString();
            AddressId = distributionPoint.Address.Id;
        }
    }
}