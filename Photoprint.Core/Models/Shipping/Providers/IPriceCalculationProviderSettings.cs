namespace Photoprint.Core.Models
{
    public interface IPriceCalculationProviderSettings : IShippingServiceProviderSettings
    {
        bool IsDeliveryPriceCalculationEnabled { get; }
        bool GetDefaultPriceFromAddressList { get; }

        decimal AdditionalPrice { get; set; }

        decimal PriceMultiplier { get; set; }

        decimal DefaultPrice { get; set; }

        int? DefaultShippingPriceId { get; set; }
    }
}