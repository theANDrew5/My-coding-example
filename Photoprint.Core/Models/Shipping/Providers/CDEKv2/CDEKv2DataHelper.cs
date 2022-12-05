using System;
using System.Collections.Generic;

namespace Photoprint.Core.Models
{
    public static class CDEKv2DataHelper
    {
        public static readonly IReadOnlyDictionary<string, string> ContryCodes = new Dictionary<string, string>
        {
            { "RU", "Россия"    },
            { "KZ", "Казахстан" },
            { "BY", "Беларусь"  },
            { "KG", "Киргизия"  },
            { "AM", "Армения"   },
        };

        public static readonly IReadOnlyCollection<CDEKv2Tariff> ToStorageTariffs = new List<CDEKv2Tariff>
        {
                new CDEKv2Tariff { Code = 136, Name = "Посылка склад-склад (до 30кг)", Mode = CDEKv2TariffMode.PostalPostal, MaxWeightGram = 29989 },
                new CDEKv2Tariff { Code = 138, Name = "Посылка дверь-склад (до 30кг)", Mode = CDEKv2TariffMode.DoorPostal, MaxWeightGram = 29989 },
                new CDEKv2Tariff { Code = 232, Name = "Экономичная посылка дверь-склад (до 50кг)", Mode = CDEKv2TariffMode.DoorPostal, MaxWeightGram = 49989 },
                new CDEKv2Tariff { Code = 234, Name = "Экономичная посылка склад-склад (до 50кг)", Mode = CDEKv2TariffMode.PostalPostal, MaxWeightGram = 49989 },
                new CDEKv2Tariff { Code = 291, Name = "CDEK Express склад-склад", Mode = CDEKv2TariffMode.PostalPostal, MaxWeightGram = 100000 },
                new CDEKv2Tariff { Code = 295, Name = "CDEK Express дверь-склад", Mode = CDEKv2TariffMode.DoorPostal, MaxWeightGram = 100000 },
        };

        public static readonly IReadOnlyCollection<CDEKv2Tariff> ToClientTariffs = new List<CDEKv2Tariff>
        {
                new CDEKv2Tariff { Code = 137, Name = "Посылка склад-дверь", Mode = CDEKv2TariffMode.PostalDoor, MaxWeightGram = 29989 },
                new CDEKv2Tariff { Code = 139, Name = "Посылка дверь-дверь", Mode = CDEKv2TariffMode.DoorDoor, MaxWeightGram = 29989 },
                new CDEKv2Tariff { Code = 233, Name = "Экономичная посылка склад-дверь (до 50кг)", Mode = CDEKv2TariffMode.PostalDoor, MaxWeightGram = 49989 },
                new CDEKv2Tariff { Code = 293, Name = "CDEK Express дверь-дверь", Mode = CDEKv2TariffMode.DoorDoor, MaxWeightGram = 100000 },
                new CDEKv2Tariff { Code = 294, Name = "CDEK Express склад-дверь", Mode = CDEKv2TariffMode.PostalDoor, MaxWeightGram = 100000 }
        };

        public static readonly IReadOnlyCollection<CDEKv2Tariff> ToPostamatTariffs = new List<CDEKv2Tariff>
        {
                new CDEKv2Tariff { Code = 366, Name = "Посылка дверь-постамат (до 30кг)", Mode = CDEKv2TariffMode.DoorPostomat, MaxWeightGram = 29989 },
                new CDEKv2Tariff { Code = 368, Name = "Посылка склад-постамат (до 30кг)", Mode = CDEKv2TariffMode.PostalPostomat, MaxWeightGram = 29989 },
                new CDEKv2Tariff { Code = 378, Name = "Экономичная посылка склад-постамат (до 50кг)", Mode = CDEKv2TariffMode.PostalPostomat, MaxWeightGram = 49989 },
        };

        public static void FixTariffMode(CDEKv2Tariff tariff)
        {
            switch (tariff.Code)
            {   
                case 136: 
                    tariff.Mode = CDEKv2TariffMode.PostalPostal;
                    break;
                case 138: 
                    tariff.Mode = CDEKv2TariffMode.DoorPostal;
                    break;
                case 232: 
                    tariff.Mode = CDEKv2TariffMode.DoorPostal;
                    break;
                case 234: 
                    tariff.Mode = CDEKv2TariffMode.PostalPostal;
                    break;
                case 291: 
                    tariff.Mode = CDEKv2TariffMode.PostalPostal;
                    break;
                case 295: 
                    tariff.Mode = CDEKv2TariffMode.DoorPostal;
                    break;
                case 137: 
                    tariff.Mode = CDEKv2TariffMode.PostalDoor;
                    break;
                case 139: 
                    tariff.Mode = CDEKv2TariffMode.DoorDoor;
                    break;
                case 233: 
                    tariff.Mode = CDEKv2TariffMode.PostalDoor;
                    break;
                case 293: 
                    tariff.Mode = CDEKv2TariffMode.DoorDoor;
                    break;
                case 294: 
                    tariff.Mode = CDEKv2TariffMode.PostalDoor;
                    break;
                case 366: 
                    tariff.Mode = CDEKv2TariffMode.DoorPostomat;
                    break;
                case 368: 
                    tariff.Mode = CDEKv2TariffMode.PostalPostomat;
                    break;
                case 378: 
                    tariff.Mode = CDEKv2TariffMode.PostalPostomat;
                    break;

                default: throw new Exception($"Unknown tariff code: {tariff.Code}");
            }
        }
    }
}
