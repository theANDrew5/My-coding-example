using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Photoprint.Core.Models.DDelivery
{
	public class DDeliveryPickupPoint
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public int CityId { get; set; }
		public string City { get; set; }
		public string Region { get; set; }
		public int RegionId { get; set; }
		public string CityType { get; set; }
		public string PostalCode { get; set; }


		public string Company { get; set; }
		public int CompanyId { get; set; }
		
		public string Address { get; set; }

		public string Schedule { get; set; }

		public bool HasFittingRoom { get; set; }

		public bool IsCash { get; set; }
		public bool IsCard { get; set; }

		public string Longitude { get; set; }
		public string Latitude { get; set; }

		public string Metro { get; set; }

		public string DescriptionIn { get; set; }
		public string DescriptionOut { get; set; }

		public string IndoorPlace { get; set; }

		public string Type { get; set; }

		public string CompanyCode { get; set; }
		public string Status { get; set; }
	}
}