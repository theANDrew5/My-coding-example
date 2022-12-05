using System;

namespace Photoprint.Core.Models
{
    public class CDEKv2Tariff
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public int MaxWeightGram { get; set; }

        public CDEKv2TariffMode Mode { get; set; }

        public bool FromDoor => Mode == CDEKv2TariffMode.DoorDoor
                                || Mode == CDEKv2TariffMode.DoorPostal || Mode == CDEKv2TariffMode.DoorPostomat;

        public bool ToDoor => Mode == CDEKv2TariffMode.PostalDoor || Mode == CDEKv2TariffMode.DoorDoor;

        public bool ToPostomat => Mode == CDEKv2TariffMode.DoorPostomat || Mode == CDEKv2TariffMode.PostalPostomat;
    }

    public enum CDEKv2TariffMode
    {
        DoorDoor = 1,
        DoorPostal = 2,
        PostalDoor = 3,
        PostalPostal = 4,
        DoorPostomat = 5,
        PostalPostomat = 6
    }
}
