using Newtonsoft.Json;

namespace Photoprint.Core.Models
{
    public class GoogleSuggestResult: GooglePlace
    {
        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("structured_formatting")]
        public StructuredFormatting Formatting { get; set; }

    }

    public class StructuredFormatting
    {
        [JsonProperty ("main_text")]
        public string MainText { get; set; }

        [JsonProperty ("secondary_text")]
        public string SecondaryText { get; set; }
    }
    //public class GooglePlaceSuggestResult
    //{
    //    [JsonProperty("formatted_address")]
    //    public string FormattedAddress { get; set; }

    //    [JsonProperty("geometry")]
    //    public GoogleSuggestGeometry Geometry { get; set; }

    //    [JsonProperty ("name")]
    //    public string Name { get; set; }

    //    [JsonProperty ("types")]
    //    public string [] Types { get; set; }
    //}

    //public class GoogleSuggestGeometry
    //{
    //    [JsonProperty("location")]
    //    public GoogleSuggestLocation Location { get; set; }

    //    [JsonProperty("viewport")]
    //    public GoogleSuggestViewport Viewport { get; set; }
    //}

    //public class GoogleSuggestLocation
    //{
    //    [JsonProperty("lat")]
    //    public string Latitude { get; set; }

    //    [JsonProperty("lng")]
    //    public string Longitude { get; set; }
    //}

    //public class GoogleSuggestViewport
    //{
    //    [JsonProperty ("northeast")]
    //    public GoogleSuggestLocation NorthEast { get; set; }

    //    [JsonProperty ("southwest")]
    //    public GoogleSuggestLocation SouthWest { get; set; }
    //}
}
