
namespace Photoprint.Core.Models
{
    public enum RussianPostAddressType : byte
    {
        /// <summary>
        /// Стандартный (улица, дом, квартира)
        /// </summary>
        DEFAULT,

        /// <summary>
        /// Абонентский ящик
        /// </summary>
        PO_BOX,

        /// <summary>
        /// До востребования
        /// </summary>
        DEMAND,

        /// <summary>
        ///	Для военных частей
        /// </summary>
        UNIT
    }
}
