using System.Runtime.Serialization;

namespace Photoprint.Core.Models
{
    public enum DpdServiceCode
    {
        /// <summary>
        /// DPD 18:00
        /// </summary>
        BZP,
        /// <summary>
        /// DPD ECONOMY
        /// </summary>
        ECN,
        /// <summary>
        /// DPD ECONOMY CU
        /// </summary>
        ECU,
        /// <summary>
        /// DPD CLASSIC
        /// </summary>
        CUR,
        /// <summary>
        /// DPD EXPRESS
        /// </summary>
        NDY,
        /// <summary>
        /// DPD Online Express
        /// </summary>
        CSM,
        /// <summary>
        /// DPD OPTIMUM
        /// </summary>
        PCL,
        /// <summary>
        /// DPD SHOP (только для serviceVariant ТТ)
        /// </summary>
        PUP,
        /// <summary>
        /// DPD CLASSIC international IMPORT
        /// </summary>
        DPI,
        /// <summary>
        /// DPD CLASSIC international EXPORT
        /// </summary>
        DPE,
        /// <summary>
        /// DPD MAX domestic
        /// </summary>
        MAX,
        /// <summary>
        /// DPD Standart
        /// </summary>
        MXO
    }
}
