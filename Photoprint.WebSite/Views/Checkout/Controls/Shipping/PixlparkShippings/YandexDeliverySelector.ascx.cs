using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Photoprint.Core.Models;
using Photoprint.Core.Services;

namespace Photoprint.WebSite.Controls
{
    public partial class YandexDeliverySelector : BaseShippingSelectorControl
    {
        protected static readonly IShoppingCartService ShoppingCartService = Container.GetInstance<IShoppingCartService>();

        protected Postal InitializedPostal { get; private set; }
        protected List<int> DiscountIds { get { return Discounts?.Select(d => d.Id).ToList(); } }

        public YandexDeliveryServiceProviderSettings YandexDeliverySettings { get; set; }

        private ShoppingCart _cart;
        public ShoppingCart Cart => _cart ?? (_cart = ShoppingCartService.GetCart(WebSiteGlobal.CurrentPhotolab));


        public override OrderAddress GetSelectedOrderAddress()
        {
            var json = Request.Form["yandexDeliveryData"];
            if (string.IsNullOrWhiteSpace(json)) return null;

            var model = JsonConvert.DeserializeObject<YandexDeliveryAddressInfo>(json);
            if (model == null) return null;
            
                 OrderAddress = new OrderAddress
                {
                    Latitude = model.PickupPoint.Address.Latitude.ToString(),
                    Longitude = model.PickupPoint.Address.Longitude.ToString(),
                    Country = "Россия",
                    City = model.PickupPoint.Address.Locality,
                    Street = model.PickupPoint.Address.Street,
                    House = model.PickupPoint.Address.House,
                    Flat = model.PickupPoint.Address.Apartment,
                    Phone = model.Phone,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DeliveryProperties = new DeliveryAddressProperties  { YandexDeliveryAddressInfo = model }
                };
            return OrderAddress;
        }

        protected YandexDeliverySelector()
        {
            Load += PageLoad;
        }

        private void PageLoad(object sender, EventArgs e)
        {
            Response.AppendHeader("Access-Control-Allow-Origin", "*");
            if (!WebSiteGlobal.CurrentPhotolab.Properties.IsCyrillicNamesEnabled || IsCompanyTeamMember)
            {
                if (Shipping.ShippingServiceProviderType != ShippingServiceProviderType.Photomax)
                {
                    regFirstName.Enabled = false;
                    regLastName.Enabled = false;
                }
                else
                {
                    const string regExpr = "^[а-яА-ЯёЁa-zA-Z0-9]+$";

                    regFirstName.ValidationExpression = regExpr;
                    regFirstName.ErrorMessage = RM.GetString(RS.Common.FirstNameRequiredValidation);

                    regLastName.ValidationExpression = regExpr;
                    regLastName.ErrorMessage = RM.GetString(RS.Common.LastNameRequiredValidation);
                }
            }
        }

        public override bool ValidateInput() => OrderAddress?.DeliveryProperties?.YandexDeliveryAddressInfo != null;

        protected override void InitSelectedShipping(Shipping shipping)
        {
            var postal = shipping as Postal;
            if (postal == null || !(shipping.ServiceProviderSettings is YandexDeliveryServiceProviderSettings settings)) return;
            YandexDeliverySettings = settings;
        }
    }
}