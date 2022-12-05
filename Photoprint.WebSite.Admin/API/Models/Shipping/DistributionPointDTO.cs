using Photoprint.Core.Models;

namespace Photoprint.WebSite.Admin.API.Models
{
    public class DistributionPointDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Flat { get; set; }

        public DistributionPointDTO(DistributionPoint point, SystemLanguage language)
        {
            Id = point.Id;
            Title = point.AdminTitle;
            Street = point.Address.Street;
            House = point.Address.House;
            Flat = point.Address.Flat;


        }
    }
}