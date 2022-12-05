
namespace Photoprint.Core.Models
{
    public enum RussianPostEnvelopeType : byte
    {
        /// <summary>
        /// 229мм x 324мм
        /// </summary>
        C4,

        /// <summary>
        ///	162мм x 229мм
        /// </summary>
        C5,

        /// <summary>
        /// 114мм x 162мм
        /// </summary>
        C6,

        /// <summary>
        /// 110мм x 220мм
        /// </summary>
        DL,


        /// <summary>
        /// 105мм x 148мм
        /// </summary>
        A6,

        /// <summary>
        /// 74мм x 105мм
        /// </summary>
        A7,

        /// <summary>
        /// Стикер ЗОО X6 (99 x 105 мм)
        /// </summary>
        OX,

        /// <summary>
        ///	Стикер ЗОО А6 (105 x 148,5 мм)
        /// </summary>
        OA
    }
}
