namespace Photoprint.Core.Models
{
    public class AddressSuggest
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public CityAddress City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Description { get; set; }
        public string GeoId { get; set; }

        public string SearchString
        {
            get
            {
                var result = !string.IsNullOrWhiteSpace(Street)? $"{Street}, {Description}" : $"{Description}";
                if (!string.IsNullOrWhiteSpace(House))
                    result = string.Concat($"{House}, ", result);
                return result;
            }
        }
        public bool SearchByCoordsEnable => !string.IsNullOrWhiteSpace(Latitude)&&!string.IsNullOrWhiteSpace(Longitude);
    }
}
