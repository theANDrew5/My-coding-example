
namespace Photoprint.Core.Models
{
    public enum RussianPostPaymentMethod : byte
    {
        /// <summary>
        /// Безналичный расчет
        /// </summary>
        CASHLESS,

        /// <summary>
        /// Оплата марками
        /// </summary>
        STAMP,

        /// <summary>
        /// Франкирование
        /// </summary>
        FRANKING,

        /// <summary>
        /// На франкировку
        /// </summary>
        TO_FRANKING,

        /// <summary>
        ///	Знак онлайн оплаты
        /// </summary>
        ONLINE_PAYMENT_MARK
    }
}
