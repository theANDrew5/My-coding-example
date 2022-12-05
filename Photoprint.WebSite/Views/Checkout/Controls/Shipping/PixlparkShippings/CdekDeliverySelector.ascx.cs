using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photoprint.Core;
using Photoprint.Core.Models;

namespace Photoprint.WebSite.Controls
{
    public partial class CdekDeliverySelector : BaseShippingSelectorControl
    {
        protected Postal CurrentPostal { get; set; }

        protected string ProductsListJson
        {
            get
            {
                if (ShippableItems?.Any() != true || CurrentPostal?.Properties == null) return string.Empty;

                var size = CurrentPostal.Properties.ParcelPacking.GetParcelSize(ShippableItems);

                var weight = ShippableItems.Sum(x => x.TotalWeight)*1000;
                if (weight < CdekServiceProviderSettings.MinWeight)
                {
                    weight = ((CdekServiceProviderSettings)CurrentPostal.ServiceProviderSettings).DefaultWeight*1000;
                }
                else if (weight > CdekServiceProviderSettings.MaxWeight)
                {
                    weight = CdekServiceProviderSettings.MaxWeight;
                }
                return size != null
                    ? JsonConvert.SerializeObject(new[]
                    {
                        new
                        {
                            weight,
                            length = size.LengthCm,
                            width = size.WidthCm,
                            height = size.HeightCm
                        }
                    })
                    : string.Empty;
            }
        }

        protected string CityFromId => CurrentPostal?.ServiceProviderSettings is CdekServiceProviderSettings settings
            ? settings.SendCityCode
            : string.Empty;

        protected string YandexMapApiKey => FrontendAuthenticationSettings.YandexMap?.Apikey ?? string.Empty;

        protected void Page_Load(object sender, EventArgs e) { }

        public override OrderAddress GetSelectedOrderAddress()
        {
            var result = Request.Form["cdekWidgetResult"];
            if (string.IsNullOrWhiteSpace(result)) return null;

            var resultObj = JObject.Parse(result);
            OrderAddress = new OrderAddress
            {
                Country = "Россия",
                City = resultObj?["cityName"].Value<string>() ?? string.Empty,
                AddressLine1 = resultObj?["PVZ"]?["Address"].Value<string>(),
                FirstName = LoggedInUser.FullName,
                Phone = LoggedInUser.GetCleanPhone(),
                PostalCode = string.Empty,
                Description = resultObj?["PVZ"]?["Note"].Value<string>(),
                Latitude = resultObj?["PVZ"]?["cX"].Value<string>(),
                Longitude = resultObj?["PVZ"]?["cY"].Value<string>(),
                DeliveryProperties = new DeliveryAddressProperties
                {
                    CdekAddressInfo = new CdekAddressInfo
                    {
                        PvzCityCode = resultObj?["city"].Value<string>(),
                        PvzCode = resultObj?["id"].Value<string>(),
                        PvzName = resultObj?["PVZ"]?["Name"].Value<string>(),
                        PvzPhone = resultObj?["PVZ"]?["Phone"].Value<string>(),
                        PvzWorkTime = resultObj?["PVZ"]?["WorkTime"].Value<string>(),
                        Tariff = resultObj?["tarif"].Value<int>() ?? 0
                    }
                }
            };
            return OrderAddress;
        }

        protected override void InitSelectedShipping(Shipping shipping)
        {
            if (shipping == null) throw new ArgumentNullException(nameof(shipping));

            if (shipping is Postal postal)
            {
                CurrentPostal = postal;
            }
            else
            {
                throw new PhotoprintSystemException("Shipping must be postal", string.Empty);
            }
        }

        public override bool ValidateInput() => !string.IsNullOrWhiteSpace(Request.Form["cdekWidgetResult"]);
    }
}