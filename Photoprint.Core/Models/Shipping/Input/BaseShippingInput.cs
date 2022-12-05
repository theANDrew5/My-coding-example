using System;

namespace Photoprint.Core.Models
{
    public abstract class BaseShippingInput
    {
        public int PhotolabId { get; set; }
        public string AdminTitle { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsEnabledForMobileApp { get; set; }
        public bool IsShippingPricePaidSeparately { get; set; }       
        public string Email { get; set; }
        public string Phone { get; set; }

        public BaseShippingInput() { }
        public BaseShippingInput(Shipping shipping)
        {
            if (shipping == null) throw new ArgumentNullException(nameof(shipping));

            PhotolabId = shipping.PhotolabId;
            AdminTitle = shipping.AdminTitle;
            IsEnabled = shipping.IsEnabled;
            IsEnabledForMobileApp = shipping.IsEnabledForMobileApp;
            IsShippingPricePaidSeparately = shipping.IsShippingPricePaidSeparately;
            Email = shipping.Email;
            Phone = shipping.Phone;
        }
    }
}
