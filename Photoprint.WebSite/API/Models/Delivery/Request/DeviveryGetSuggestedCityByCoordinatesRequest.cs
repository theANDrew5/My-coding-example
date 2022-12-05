namespace Photoprint.WebSite.API.Models.Delivery
{
    public class DeviveryGetSuggestedCityByCoordinatesRequest : BaseDeliveryRequest
    {
        public string Lat { get; set; }
        public string Lng { get; set; }
    }
}