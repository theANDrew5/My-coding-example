namespace Photoprint.Core.Models
{
    public class DpdSmallOrder
    {
        public string OrderNumberInternal { get; set; }
        public int CargoNumPack { get; set; }//Количество грузомест (посылок) в отправке 
        public double CargoWeight { get; set; }
        public bool CargoRegistered { get; set; }
        public string CargoCategory { get; set; }
        public string DatePickup { get; set; }
    }
}
