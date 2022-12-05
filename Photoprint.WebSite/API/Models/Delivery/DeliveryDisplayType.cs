using Photoprint.Core.Models;

namespace Photoprint.WebSite.API.Models.Delivery
{
    public enum DeliveryDisplayType
    {
        /// <summary>
        /// курьерская доставка. может включать в себя как и нашу курьерку, так и курьерку тк
        /// </summary>
        Courier,
        /// <summary>
        /// доставки без синхронизации адресов НЕ КУРЬЕРКИ
        /// </summary>
        DeliveryByAddress,
        /// <summary>
        /// точки выдачи транспортных компаний
        /// </summary>
        Pickpoint,
        /// <summary>
        /// точки выдачи компаний, что настроены у нас в платформе
        /// </summary>
        Office,
        /// <summary>
        /// тип доставки PickPoint, который работает через сторонний плагин
        /// </summary>
        DeliveryPlaginPickPoint,
        /// <summary>
        /// тип доставки SafeRoute, который работает через сторонний плагин
        /// </summary>
        DeliveryPlaginSafeRoute,
    }

    public static class DeliveryDisplayTypeResolver
    {
        public static DeliveryDisplayType GetDeliveryDisplayType(Shipping shipping, bool officesInPickpoints = false)
        {
            var postalType = (shipping is Postal postal) ? postal.PostalType :
                shipping.Type == ShippingType.Courier ? PostalType.ToClientDelivery : PostalType.ToStorageDelivery;
            return GetDeliveryDisplayType(shipping.Type, postalType, shipping.ShippingServiceProviderType,
                officesInPickpoints, shipping.ServiceProviderSettings);
        }

        public static DeliveryDisplayType GetDeliveryDisplayType(ShippingSmallToDeliveryModel shipping, bool officesInPickpoints = false)
        {
            return GetDeliveryDisplayType(shipping.Type, shipping.PostalType, shipping.ServiceProviderType,
                officesInPickpoints, shipping.ServiceProviderSettings);
        }

        private static DeliveryDisplayType GetDeliveryDisplayType(ShippingType shippingType,
            PostalType shippingPostalType, ShippingServiceProviderType shippingProviderType, bool officesInPickpoints,
            IShippingServiceProviderSettings providerSettings)
        {

            switch (shippingType)
            {
                case ShippingType.Courier:
                    return DeliveryDisplayType.Courier;
                case ShippingType.Point:
                    return !officesInPickpoints && shippingProviderType == ShippingServiceProviderType.General
                        ? DeliveryDisplayType.Office
                        : DeliveryDisplayType.Pickpoint;
                case ShippingType.Postal:
                    {
                        switch (shippingProviderType)
                        {
                            case ShippingServiceProviderType.Pickpoint:
                                return DeliveryDisplayType.DeliveryPlaginPickPoint;
                            case ShippingServiceProviderType.DDeliveryV2:
                                return DeliveryDisplayType.DeliveryPlaginSafeRoute;
                            case ShippingServiceProviderType.General:
                                return shippingPostalType == PostalType.ToStorageDelivery
                                    ? DeliveryDisplayType.DeliveryByAddress
                                    : DeliveryDisplayType.Courier;
                            default:
                            {
                                if (shippingPostalType == PostalType.ToClientDelivery)
                                    return DeliveryDisplayType.Courier;
                                return (!providerSettings.SupportAddresesSynchronization &&
                                        providerSettings.ShowAddressTab)
                                    ? DeliveryDisplayType.DeliveryByAddress
                                    : DeliveryDisplayType.Pickpoint;
                            }
                        }
                        
                    }
                default:
                    return DeliveryDisplayType.DeliveryByAddress;
            }
        }
    }
}