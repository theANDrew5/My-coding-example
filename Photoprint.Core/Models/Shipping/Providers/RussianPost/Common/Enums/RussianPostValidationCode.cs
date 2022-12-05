
namespace Photoprint.Core.Models
{
    public enum RussianPostValidationCode
    {
        /// <summary>
        /// Подтверждено контролером
        /// </summary>
        CONFIRMED_MANUALLY,

        /// <summary>
        /// Уверенное распознавание
        /// </summary>
        VALIDATED,

        /// <summary>
        /// Распознан: адрес был перезаписан в справочнике
        /// </summary>
        OVERRIDDEN,

        /// <summary>
        /// На проверку, неразобранные части
        /// </summary>
        NOT_VALIDATED_HAS_UNPARSED_PARTS,

        /// <summary>
        /// На проверку, предположение
        /// </summary>
        NOT_VALIDATED_HAS_ASSUMPTION,

        /// <summary>
        /// На проверку, нет основных частей
        /// </summary>
        NOT_VALIDATED_HAS_NO_MAIN_POINTS,

        /// <summary>
        /// На проверку, предположение по улице
        /// </summary>
        NOT_VALIDATED_HAS_NUMBER_STREET_ASSUMPTION,

        /// <summary>
        /// На проверку, нет в КЛАДР
        /// </summary>
        NOT_VALIDATED_HAS_NO_KLADR_RECORD,

        /// <summary>
        /// На проверку, нет улицы или населенного пункта
        /// </summary>
        NOT_VALIDATED_HOUSE_WITHOUT_STREET_OR_NP,

        /// <summary>
        /// На проверку, нет дома
        /// </summary>
        NOT_VALIDATED_HOUSE_EXTENSION_WITHOUT_HOUSE,

        /// <summary>
        /// На проверку, неоднозначность
        /// </summary>
        NOT_VALIDATED_HAS_AMBI,

        /// <summary>
        /// На проверку, большой номер дома
        /// </summary>
        NOT_VALIDATED_EXCEDED_HOUSE_NUMBER,

        /// <summary>
        /// На проверку, некорректный дом
        /// </summary>
        NOT_VALIDATED_INCORRECT_HOUSE,

        /// <summary>
        /// На проверку, некорректное расширение дома
        /// </summary>
        NOT_VALIDATED_INCORRECT_HOUSE_EXTENSION,

        /// <summary>
        /// Иностранный адрес
        /// </summary>
        NOT_VALIDATED_FOREIGN,

        /// <summary>
        /// На проверку, не по справочнику
        /// </summary>
        NOT_VALIDATED_DICTIONARY
    }
}
