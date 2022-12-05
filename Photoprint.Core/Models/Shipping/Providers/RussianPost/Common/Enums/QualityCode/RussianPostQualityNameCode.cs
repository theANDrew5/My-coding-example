using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photoprint.Core.Models
{
    public enum RussianPostQualityNameCode
    {
        /// <summary>
        /// Подтверждено контролером
        /// </summary>
        CONFIRMED_MANUALLY, 	
        /// <summary>
        /// Правильное значение
        /// </summary>
        EDITED, 	
        /// <summary>
        /// Сомнительное значение
        /// </summary>
        NOT_SURE 	
    }
}
