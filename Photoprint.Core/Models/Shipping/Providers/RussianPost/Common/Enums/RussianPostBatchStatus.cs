
namespace Photoprint.Core.Models
{

    public enum RussianPostBatchStatus : byte
    {
        /// <summary>
        ///	Партия создана
        /// </summary>
        CREATED,

        /// <summary>
        ///	Партия в процессе приема, редактирование запрещено
        /// </summary>
        FROZEN,

        /// <summary>
        ///	Партия принята в отделении связи
        /// </summary>
        ACCEPTED,

        /// <summary>
        ///	По заказам в партии существуют данные в сервисе трекинга
        /// </summary>
        SENT,

        /// <summary>
        ///	Партия находится в архиве
        /// </summary>
        ARCHIVED
    }
}
