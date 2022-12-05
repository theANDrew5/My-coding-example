namespace Photoprint.Core.Models
{

    public enum YandexDeliveryServiceCode
    {
        /// <summary>
        /// доставка.
        /// </summary>
        DELIVERY,

        /// <summary>
        /// вознаграждение за перечисление денежных средств.
        /// </summary>
        CASH_SERVICE,

        /// <summary>
        ///  сортировка на едином складе.
        /// </summary>
        SORT,

        /// <summary>
        /// объявление ценности заказа.
        /// </summary>
        INSURANCE,

        /// <summary>
        /// ожидание курьера.
        /// </summary>
        WAIT_20,

        /// <summary>
        /// возврат заказа на единый склад.
        /// </summary>
        RETURN,

        /// <summary>
        /// сортировка возвращенного заказа.
        /// </summary>
        RETURN_SORT
    }
}
