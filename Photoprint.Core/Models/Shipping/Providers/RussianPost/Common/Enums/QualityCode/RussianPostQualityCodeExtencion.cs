
namespace Photoprint.Core.Models
{
    public static class RussianPostQualityCodeExtencionMethods
    {
        public static bool QualityCheck(this RussianPostQualityAddressCode code, out string error)
        {
            error = null;
            switch (code)
            { 
                case RussianPostQualityAddressCode.GOOD:
                case RussianPostQualityAddressCode.POSTAL_BOX:
                case RussianPostQualityAddressCode.ON_DEMAND:
                    return true;
                case RussianPostQualityAddressCode.UNDEF_01:
                    error = "Не определен регион";
                    goto default;
                case RussianPostQualityAddressCode.UNDEF_02:
                    error = "Не определен город или населенный пункт";
                    goto default;
                case RussianPostQualityAddressCode.UNDEF_03:
                    error = "Не определена улица";
                    goto default;
                case RussianPostQualityAddressCode.UNDEF_04:
                    error = "Не определен номер дома";
                    goto default;
                case RussianPostQualityAddressCode.UNDEF_05:
                    error = "Не определена квартира/офис";
                    goto default;
                case RussianPostQualityAddressCode.UNDEF_06:
                    error = "Не определен";
                    goto default;
                case RussianPostQualityAddressCode.UNDEF_07:                
                    error = "Иностранный адрес";
                    goto default;
                default:
                    return false;
            }
        }

        public static bool QualityCheck(this RussianPostQualityPhoneCode code, out string error)
        {
            error = null;
            switch (code)
            { 
                case RussianPostQualityPhoneCode.CONFIRMED_MANUALLY:
                case RussianPostQualityPhoneCode.GOOD:
                case RussianPostQualityPhoneCode.GOOD_REPLACED_CODE:
                case RussianPostQualityPhoneCode.GOOD_REPLACED_NUMBER:
                case RussianPostQualityPhoneCode.GOOD_REPLACED_CODE_NUMBER:
                case RussianPostQualityPhoneCode.GOOD_CITY_CONFLICT:
                case RussianPostQualityPhoneCode.GOOD_REGION_CONFLICT:
                case RussianPostQualityPhoneCode.GOOD_CITY:
                case RussianPostQualityPhoneCode.GOOD_EXTRA_PHONE:
                    return true;
                case RussianPostQualityPhoneCode.FOREIGN:
                    error = "Иностранный телефонный номер";
                    goto default;
                case RussianPostQualityPhoneCode.CODE_AMBI:
                    error = "Неоднозначный код телефонного номера";
                    goto default;
                case RussianPostQualityPhoneCode.EMPTY:
                    error = "Пустой телефонный номер";
                    goto default;
                case RussianPostQualityPhoneCode.GARBAGE:
                    error = "Телефонный номер содержит некорректные символы";
                    goto default;
                case RussianPostQualityPhoneCode.UNDEF:
                    error = "Телефон не может быть распознан";
                    goto default;
                case RussianPostQualityPhoneCode.INCORRECT_DATA	:
                    error = "Телефон не может быть распознан";
                    goto default;
                default:
                    return false;
            }
        }

        public static bool QualityCheck(this RussianPostQualityNameCode code, out string error)
        {
            error = null;
            switch (code)
            { 
                case RussianPostQualityNameCode.CONFIRMED_MANUALLY:
                case RussianPostQualityNameCode.EDITED:
                    return true;
                case RussianPostQualityNameCode.NOT_SURE:
                    error = "Сомнительное значение";
                    goto default;
                default:
                    return false;
            }
        }
    }
}
