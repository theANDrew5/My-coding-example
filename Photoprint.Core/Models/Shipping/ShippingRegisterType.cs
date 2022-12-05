namespace Photoprint.Core.Models
{
    public enum ShippingRegisterType
    {
        /// <summary>
        /// Регистрировать каждый заказ отдельно
        /// </summary>
        RegisterAsSeparated = 1,
        /// <summary>
        /// Регистрировать связанные заказы на один трек-номер
        /// </summary>
        RegisterAsLinked = 2
    }
}