using Photoprint.Core.Models;

namespace Photoprint.WebSite.API.Models.Delivery
{
    // Изменил тут? Не забудь изменить в DeliveryPointItem.ts!
    public enum DistributionPointType
    {
        Unknown = 0,
        Cdek = 1,
        ImLogistics = 2,
        DDelivery = 3,
        Novaposhta = 4,
        YandexDelivery = 5,
        Photomax = 6,
        Exgarant = 7,
        Boxberry = 8,
        Ukrposhta = 9,
        Postnl = 10,
        Justin = 11,
        Omniva = 12,
        Econt = 13,
        Evropochta = 14,
        Pickpoint = 15,
        RussianPost = 16,
        DPD = 17,
        CDEKv2 = 18
    }

    public static class DistributionPointTypeResolver
    {
        public static DistributionPointType GetDistributionPointTypeByProvider(ShippingServiceProviderType type, ShippingAddress address)
        {
            switch (type)
            {
                case ShippingServiceProviderType.Cdek:
                    return DistributionPointType.Cdek;

                case ShippingServiceProviderType.ImLogistics:
                case ShippingServiceProviderType.ImlV2:
                    return DistributionPointType.ImLogistics;

                case ShippingServiceProviderType.DDelivery:
                case ShippingServiceProviderType.DDeliveryV2:
                    return DistributionPointType.DDelivery;

                case ShippingServiceProviderType.NovaposhtaV2:
                    return DistributionPointType.Novaposhta;

                case ShippingServiceProviderType.YandexDelivery:
                    return DistributionPointType.YandexDelivery;

                case ShippingServiceProviderType.Photomax:
                    var ownerId = address?.DeliveryProperties?.PhotomaxAddressInfo?.OwnerId ?? string.Empty;
                    switch (ownerId)
                    {
                        // больше данных по OwnerId не нашел
                        // в документации они не описаны, брал информацию из самих адресов и Product-13114

                        case "527": // PickPoint постаматы                           
                        case "923": // PickPoint ПВЗ 
                            return DistributionPointType.Pickpoint;

                        case "85": // Boxberry ПВЗ
                            return DistributionPointType.Boxberry;

                        case "969": // Доставка курьером (Москва, Питер)
                        case "358": // магазины Мультифото самовывоз (Код: 358)
                        default:
                            return DistributionPointType.Photomax;
                    }

                case ShippingServiceProviderType.Exgarant:
                    return DistributionPointType.Exgarant;

                case ShippingServiceProviderType.Boxberry:
                    return DistributionPointType.Boxberry;

                case ShippingServiceProviderType.Ukrposhta:
                    return DistributionPointType.Ukrposhta;

                case ShippingServiceProviderType.Postnl:
                    return DistributionPointType.Postnl;

                case ShippingServiceProviderType.Justin:
                    return DistributionPointType.Justin;

                case ShippingServiceProviderType.Omniva:
                    return DistributionPointType.Omniva;

                case ShippingServiceProviderType.Econt:
                    return DistributionPointType.Econt;

                case ShippingServiceProviderType.Evropochta:
                    return DistributionPointType.Evropochta;

                case ShippingServiceProviderType.Pickpoint:
                    return DistributionPointType.Pickpoint;

                case ShippingServiceProviderType.RussianPost:
                    return DistributionPointType.RussianPost;

                case ShippingServiceProviderType.CDEKv2:
                    return DistributionPointType.CDEKv2;
                
                case ShippingServiceProviderType.DPD:
                    return DistributionPointType.DPD;
            }

            return DistributionPointType.Unknown;
        }
    }
}