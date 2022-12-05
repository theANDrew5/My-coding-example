using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photoprint.Core.Models;

namespace Photoprint.WebSite.API.Models.Delivery
{

    public abstract class BaseShippingPointDto
    {
        public string ShippingTitle { get;}
        public string ShippingDescription { get;}
        public string Title { get; }
        public abstract ShippingType Type { get; }
        public ShippingServiceProviderType ProviderType { get; }
        public DistributionPointType PointType { get; }
        public ShippingAddressDTO Address { get; }

        protected BaseShippingPointDto(ShippingSmallToDeliveryModel shipping, ShippingAddress shippingAddress,
            Language language)
        {
            ShippingTitle = shipping.GetTitle(language);
            ShippingDescription = shipping.GetDescription(language);
            Title = shippingAddress.TitleLocalized[language];
            ProviderType = shipping.ServiceProviderType;
            PointType = DistributionPointTypeResolver.GetDistributionPointTypeByProvider(shipping.ServiceProviderType, shippingAddress);
            Address = new ShippingAddressDTO(shippingAddress);
        }

        protected BaseShippingPointDto()
        {
        }
    }

    public class DistributionPointDto : BaseShippingPointDto
    {
        public override ShippingType Type => ShippingType.Point;
        public string Description { get; set; }
        public string WorkTime { get; set; }
        public string Phone { get; set; }

        public DistributionPointDto(ShippingSmallToDeliveryModel shipping, ShippingAddress shippingAddress,
            Language language) : base(shipping, shippingAddress, language)
        {
            Description = shippingAddress.Description;
            WorkTime = !string.IsNullOrWhiteSpace(shippingAddress.WorkTime)
                ? shippingAddress.WorkTime
                : shipping.WorkTime;
            Phone = shippingAddress.Phone;
        }
        public DistributionPointDto() : base()
        {
        }

    }
    
    public class CourierPointDto : BaseShippingPointDto
    {
        public override ShippingType Type => ShippingType.Courier;
        public bool PostCodeRequired { get; set; }

        public CourierPointDto(ShippingSmallToDeliveryModel shipping, ShippingAddress shippingAddress,
            Language language) : base(shipping, shippingAddress, language)
        {
            PostCodeRequired = shipping.IsIndexRequired;
        }
    }
}