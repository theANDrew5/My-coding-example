namespace Photoprint.Core.Models
{
    public enum RussianPostDimensionType : byte
    {
        /// <summary>
        ///до 260х170х80 мм
        /// </summary>
        S,

        /// <summary>
        /// до 300х200х150 мм
        /// </summary>
        M,

        /// <summary>
        /// до 400х270х180 мм
        /// </summary>
        L,

        /// <summary>
        /// 530х260х220 мм
        /// </summary>
        XL,

        /// <summary>
        /// Негабарит (сумма сторон не более 1400 мм, сторона не более 600 мм)
        /// </summary>
        OVERSIZED
    }
}
