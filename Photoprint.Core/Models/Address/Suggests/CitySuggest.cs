using System;

namespace Photoprint.Core.Models
{
    public class CitySuggest
    {
        public string Title { get; set;}
        public string Description { get; set;}
        public string Longitude { get; set;}
		public string Latitude { get; set;}
        public string GeoId { get; set;}
        public ToponymType Type { get; set;}
        public string Search => Description==null? $"{Title}": $"{Description}, {Title}";
        public string CacheKey => $"{Type}:{Title}:{Description}";

        public bool isCoords => !string.IsNullOrWhiteSpace(Longitude) && !string.IsNullOrWhiteSpace(Latitude);

        public CitySuggest()
        { }

        public CitySuggest(YandexSuggestResult result)
        {
            switch (result.Kind)
            {
                case "country":
                    Type = ToponymType.Country;
                    break;
                case "province":
                    Type = ToponymType.Region;
                    break;
                case "locality":
                    Type = ToponymType.City;
                    break;
                default: throw new ArgumentException("Unknown kind result!");
            }
                
            Title = result.Name;
            Description = result.Description;
            Longitude = result.Longitude;
            Latitude = result.Latitude;
            GeoId = result.GeoId;
        }

        public CitySuggest(GoogleSuggestResult result)
        {
            switch (result.Types[0])
            {
                case "country":
                    Type = ToponymType.Country;
                    break;
                case "administrative_area_level_1":
                    Type = ToponymType.Region;
                    break;
                case "locality":
                    Type = ToponymType.City;
                    break;
                default: throw new ArgumentException("Unknown kind result!");
            }
            Title = result.Formatting.MainText;
            Description = result.Formatting.SecondaryText;
            GeoId = result.PlaceId;
        }
    }
}
