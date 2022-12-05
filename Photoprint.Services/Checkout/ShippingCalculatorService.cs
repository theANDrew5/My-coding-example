using Photoprint.Core;
using Photoprint.Core.Models;
using Photoprint.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Photoprint.Services
{
    public class ShippingCalculatorService : ServiceBase, IShippingCalculatorService
    {
        private readonly IShippingAddressService _shippingAddressService;
        private readonly IShippingPriceService _shippingPriceService;
        private readonly IPhotolabService _photolabService;
        private readonly IDiscountService _discountService;
        private readonly IShippingProviderResolverService _providerResolver;
        private readonly IPhotolabSettingsService _photolabSettingsService;

        public ShippingCalculatorService(
            IShippingAddressService shippingAddressService,
            IPhotolabService photolabService,
            IDiscountService discountService,
            IShippingPriceService shippingPriceService,
            IShippingProviderResolverService providerResolver,
            IPhotolabSettingsService photolabSettingsService)
        {
            _shippingAddressService = shippingAddressService;
            _photolabService = photolabService;
            _discountService = discountService;
            _shippingPriceService = shippingPriceService;
            _providerResolver = providerResolver;
            _photolabSettingsService = photolabSettingsService;
        }

        public bool TryGetApproximateShippingPrice(Shipping shipping, IReadOnlyCollection<IPurchasedItem> items,
            IReadOnlyCollection<Discount> discounts, out decimal resultPrice, out bool isPriceFixed)
        {
            var lab = _photolabService.GetById(shipping.PhotolabId);
            if (lab == null) throw new ArgumentNullException(nameof(lab));

            resultPrice = 0m;

            if (!_shippingPriceService.TryCalculatePriceForShipping(shipping, items, out var shippingPrice,
                    out isPriceFixed))
                return false;

            var photomaxAddressInfo = shipping is DistributionPoint point ? point.NetprintSettings : shipping.Properties.PhotomaxAddressInfo;

            var isExportToBackofficeEnabled = _photolabSettingsService.IsExportToBackofficeEnabled(lab);
            if (isExportToBackofficeEnabled && photomaxAddressInfo != null && items != null && items.Any(x => x.IsExportToPhotoexpertItem))
            {
                shippingPrice += photomaxAddressInfo.DeliveryAdditionalPrice;
            }

            if (shippingPrice > 0 && discounts != null)
            {
                var filteredDiscounts = _discountService.FilterDiscountsByShipping(lab, shipping, discounts);
                foreach (var discount in filteredDiscounts.Where(d => d.ValueForShipping.HasValue))
                    shippingPrice = discount.ValueForShipping.Apply(shippingPrice, items.Sum(i => i.Quantity), items.Sum(i => i.ItemPartsQuantity));
            }

            resultPrice = shippingPrice;
            return true;
        }

        public DeliveryPriceCalculationResult GetShippingPrice(Shipping shipping, OrderAddress selectedAddress,
            IReadOnlyCollection<IPurchasedItem> items, IReadOnlyCollection<Discount> discounts)
        {
            if(shipping is null) return new DeliveryPriceCalculationResult(0);
      
            var lab = _photolabService.GetById(shipping.PhotolabId);
            var calculationResult = GetShippingPriceWithoutDiscount(shipping, selectedAddress, items);

            if (!calculationResult.Success || discounts == null) return calculationResult;

            var filteredDiscounts = _discountService.FilterDiscountsByShipping(lab, shipping, discounts);
            calculationResult.ApplyDiscounts(filteredDiscounts, items);

            return calculationResult;
        }

        public DeliveryPriceCalculationResult GetShippingPriceWithoutDiscount(Shipping shipping,
            OrderAddress selectedAddress,
            IReadOnlyCollection<IPurchasedItem> items)
        {
            if (selectedAddress is null) return new DeliveryPriceCalculationResult();
            if (shipping is null) return new DeliveryPriceCalculationResult(properties: selectedAddress.DeliveryProperties);

            DeliveryPriceCalculationResult calculationResult;
            var photomaxAddressInfo = shipping.Properties.PhotomaxAddressInfo;
            var lab = _photolabService.GetById(shipping.PhotolabId);
            switch (shipping)
            {
                case Courier courier:
                {
                    var sourceAddres = selectedAddress.ShippingAddressId is null
                        ? null
                        : _shippingAddressService.GetById((int)selectedAddress.ShippingAddressId, courier);
                    calculationResult = _shippingPriceService.TryCalculatePriceForShippingAddress(courier.PhotolabId, sourceAddres, items, out var price)
                        ? new DeliveryPriceCalculationResult(price, selectedAddress.DeliveryProperties)
                        : new DeliveryPriceCalculationResult(properties: selectedAddress.DeliveryProperties);
                    break;
                }
                case DistributionPoint point:
                {
                    calculationResult = _shippingPriceService.TryCalculatePriceForShipping(point, items, out var price)
                        ? new DeliveryPriceCalculationResult(price, selectedAddress.DeliveryProperties)
                        : new DeliveryPriceCalculationResult(properties: selectedAddress.DeliveryProperties);
                    photomaxAddressInfo = point.NetprintSettings;
                    break;
                }
                case Postal postal:
                {
                    switch (postal.ShippingServiceProviderType)
                    {
                        case (ShippingServiceProviderType.DDelivery): // discount price calculated inside
                            calculationResult = new DeliveryPriceCalculationResult(selectedAddress.DeliveryProperties
                                ?.DDeliveryAddressInfo?.Result?.TotalPriceCorrected, selectedAddress.DeliveryProperties);
                                break;
                        case (ShippingServiceProviderType.DDeliveryV2):
                            calculationResult = new DeliveryPriceCalculationResult(selectedAddress.DeliveryProperties
                                ?.DDeliveryV2AddressInfo?.PriceCalcResult?.TotalPrice, selectedAddress.DeliveryProperties);
                            break;
                        default:
                            if (postal.ServiceProviderSettings is IPriceCalculationProviderSettings settings &&
                                settings.IsDeliveryPriceCalculationEnabled)
                            {
                                try
                                {
                                    var providerService = _providerResolver.GetProvider(postal.ShippingServiceProviderType);
                                    var provederCalculationResult = providerService.CalculateDeliveryPrice(postal, selectedAddress,
                                        items);
                                    if (provederCalculationResult.Success)
                                    {
                                        calculationResult = settings.PriceMultiplier > 0
                                            ? new DeliveryPriceCalculationResult(
                                                provederCalculationResult.Price * settings.PriceMultiplier +
                                                settings.AdditionalPrice, provederCalculationResult.Properties)
                                            : provederCalculationResult;
                                    }
                                    else if (settings.GetDefaultPriceFromAddressList)
                                    {
                                        calculationResult =
                                            _shippingPriceService.TryCalculatePriceForOrderAddress(postal.PhotolabId, selectedAddress, items, out var addressPrice)
                                                ? new DeliveryPriceCalculationResult(addressPrice, selectedAddress.DeliveryProperties)
                                                : new DeliveryPriceCalculationResult(properties: selectedAddress.DeliveryProperties);
                                    }
                                    else
                                    {
                                        calculationResult = new DeliveryPriceCalculationResult(settings.DefaultPrice, selectedAddress.DeliveryProperties);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (ex is PhotoprintSystemException pse && pse.Message == DeliveryExceptionMessages.NoDelivery)
                                        calculationResult = new DeliveryPriceCalculationResult(properties: selectedAddress.DeliveryProperties);
                                    else
                                        calculationResult = new DeliveryPriceCalculationResult(settings.DefaultPrice, selectedAddress.DeliveryProperties);
                                }
                            }
                            else
                            {
                                calculationResult =
                                    _shippingPriceService.TryCalculatePriceForOrderAddress(postal.PhotolabId, selectedAddress, items, out var addressPrice)
                                        ? new DeliveryPriceCalculationResult(addressPrice)
                                        : _shippingPriceService.TryCalculatePriceForShipping(shipping, items,
                                            out var shippingPrice)
                                            ? new DeliveryPriceCalculationResult(shippingPrice, selectedAddress.DeliveryProperties)
                                            : new DeliveryPriceCalculationResult(properties: selectedAddress.DeliveryProperties);
                            }

                            break;
                    }
                    break;
                }
                default:
                {
                    calculationResult =
                        _shippingPriceService.TryCalculatePriceForOrderAddress(shipping.PhotolabId, selectedAddress, items, out var addressPrice)
                            ? new DeliveryPriceCalculationResult(addressPrice, selectedAddress.DeliveryProperties)
                            : _shippingPriceService.TryCalculatePriceForShipping(shipping, items,
                                out var shippingPrice)
                                ? new DeliveryPriceCalculationResult(shippingPrice, selectedAddress.DeliveryProperties)
                                : new DeliveryPriceCalculationResult(properties: selectedAddress.DeliveryProperties);
                    break;
                }
            }

            if (!calculationResult.Success)
                return calculationResult;

            var isExportToBackofficeEnabled = _photolabSettingsService.IsExportToBackofficeEnabled(lab);
            if (isExportToBackofficeEnabled && items?.Any(x => x.IsExportToPhotoexpertItem) == true && photomaxAddressInfo != null)
            {
                calculationResult = new DeliveryPriceCalculationResult(calculationResult.Price + photomaxAddressInfo.DeliveryAdditionalPrice, calculationResult.Properties);
            }

            return calculationResult;
        }

    }
}