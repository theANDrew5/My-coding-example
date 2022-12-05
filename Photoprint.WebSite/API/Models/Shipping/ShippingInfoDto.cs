using Photoprint.Core.Models;
using System.Collections.Generic;

namespace Photoprint.WebSite.API.Models
{
    public class ShippingInfoDto
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		public string Type { get; set; }
		public string Description { get; set; }
        public string OfficeHours { get; set; }
        public string SitePageUrl { get; set; }
		public IEnumerable<ShippingAddressDto> Addresses { get; set; }

		public ShippingInfoDto(Shipping shipping, Language language, IEnumerable<ShippingAddressDto> addresses)
		{
			Id = shipping.Id;
			Title = shipping.GetTitle(language);
			Phone = shipping.Phone;
			Email = shipping.Email;
			Type = shipping.Type.ToString();
			Description = shipping.DescriptionLocalized[language];
			Addresses = addresses;

		    var distributionPoint = shipping as DistributionPoint;

		    if (distributionPoint != null)
		    {
		        OfficeHours = distributionPoint.OfficeHours;
		        SitePageUrl = distributionPoint.SitePageUrl;
		    }
        }
	}

	public class ShippingAddressDto
	{
		public int Id { get; set; }
		public string Country { get; set; }
		public string State { get; set; }
		public string City { get; set; }
		public string District { get; set; }
		public decimal Price { get; set; }
		public string FullName { get; set; }
		public string AddressLine1 { get; set; }
		public string Phone { get; set; }
		public string Description { get; set; }

		public string Longitude { get; set; }
		public string Latitude { get; set; }

		public ShippingAddressDto(ShippingAddress address)
		{
			Country = address.Country;
			State = address.Region;
			City = address.City;
			District = address.District;
			var adr = address;
			if (adr != null)
			{
				FullName = adr.AddressName;
				AddressLine1 = adr.AddressLine1;
				Phone = adr.Phone;
				Description = adr.Description;

				Longitude = adr.Longitude;
				Latitude = adr.Latitude;
			}
		}
	}
}
