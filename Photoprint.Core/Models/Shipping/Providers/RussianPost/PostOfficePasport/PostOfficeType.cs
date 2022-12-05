using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photoprint.Core.Models
{
    /// <summary>
    /// Типы почтовых отделений для запроса GetAllPostalsByType
    /// </summary>
    public enum PostOfficeType
    {
        ALL,    //Все
        OPS,    //ОПС
        PVZ,    //ПВЗ
        APS     //Почтомат
    }
}
