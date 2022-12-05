
namespace Photoprint.Core.Models
{
    //поменял здесь поменяй в MailDeliveryItem.ts
    public enum RussianPostMailCategory
    {
        /// <summary>
        ///	Простое
        /// </summary>
        SIMPLE,

        /// <summary>
        /// Заказное
        /// </summary>
        ORDERED,

        /// <summary>
        ///	Обыкновенное
        /// </summary>
        ORDINARY = 30,

        /// <summary>
        ///	С объявленной ценностью
        /// </summary>
        WITH_DECLARED_VALUE = 20,

        /// <summary>
        ///	С объявленной ценностью и наложенным платежом
        /// </summary>
        WITH_DECLARED_VALUE_AND_CASH_ON_DELIVERY,

        /// <summary>
        ///	С объявленной ценностью и обязательным платежом
        /// </summary>
        WITH_DECLARED_VALUE_AND_COMPULSORY_PAYMENT,

        /// <summary>
        ///	С обязательным платежом
        /// </summary>
        WITH_COMPULSORY_PAYMENT,

        /// <summary>
        /// Комбинированное обыкновенное
        /// </summary>
        COMBINED_ORDINARY = 80,

        /// <summary>
        ///	Комбинированное с объявленной ценностью
        /// </summary>
        COMBINED_WITH_DECLARED_VALUE,

        /// <summary>
        ///	Комбинированное с объявленной ценностью и наложенным платежом
        /// </summary>
        COMBINED_WITH_DECLARED_VALUE_AND_CASH_ON_DELIVERY
    }
}
