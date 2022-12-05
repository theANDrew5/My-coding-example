
namespace Photoprint.Core.Models
{
    /// <summary>
    ///	https://otpravka.pochta.ru/specification#/enums-base-mail-type
    /// </summary>
    //поменял здесь поменяй в MailDeliveryItem.ts
    public enum RussianPostMailType
    {
        POSTAL_PARCEL,
        ONLINE_PARCEL = 23,
        ONLINE_COURIER = 24,
        EMS,
        EMS_OPTIMAL,
        EMS_RT,
        EMS_TENDER,
        LETTER,
        LETTER_CLASS_1,
        BANDEROL,
        BUSINESS_COURIER,
        BUSINESS_COURIER_ES,
        PARCEL_CLASS_1,
        BANDEROL_CLASS_1,
        VGPO_CLASS_1,
        SMALL_PACKET,
        EASY_RETURN = 51,
        VSD,
        ECOM,
        ECOM_MARKETPLACE,
        HYPER_CARGO,
        COMBINED,
    }
}
