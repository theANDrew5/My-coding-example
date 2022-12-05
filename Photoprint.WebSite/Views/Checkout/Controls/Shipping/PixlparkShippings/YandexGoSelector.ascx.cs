using Newtonsoft.Json;
using Photoprint.Core.Models;
using Photoprint.Core.Services;

namespace Photoprint.WebSite.Controls
{
    public partial class YandexGoSelector : BaseShippingSelectorControl
    {
        protected static readonly IShoppingCartService ShoppingCartService = Container.GetInstance<IShoppingCartService>();
        private ShoppingCart _cart;
        public ShoppingCart Cart => _cart ?? (_cart = ShoppingCartService.GetCart(WebSiteGlobal.CurrentPhotolab));
        public YandexGoServiceProviderSettings YandexGoSettings { get; set; }
        protected Postal CurrentPostal { get; set; }
        public string YandexMapScript { get; set; }

        private void Page_Load()
        {
            if (LoggedInUser != null)
            {
                txtFirstName.Text = string.IsNullOrWhiteSpace(txtFirstName.Text) ? LoggedInUser.FirstName : txtFirstName.Text;
                txtLastName.Text = string.IsNullOrWhiteSpace(txtLastName.Text) ? LoggedInUser.LastName : txtLastName.Text;
                txtPhone.Text = string.IsNullOrWhiteSpace(txtPhone.Text) ? LoggedInUser.GetCleanPhone() : txtPhone.Text;
            }
        }
        public override bool ValidateInput()
        {
            return reqFirstName.IsValid && reqLastName.IsValid && reqPhone.IsValid &&
                   Request.Form["yandexGoDeliveryData"] != null;
        }

        protected override void InitSelectedShipping(Shipping shipping)
        {
            var postal = shipping as Postal;
            if (postal == null || !(shipping.ServiceProviderSettings is YandexGoServiceProviderSettings settings)) return;
            YandexGoSettings = settings;
            CurrentPostal = postal;
            YandexMapScript = $"<script src=\"https://api-maps.yandex.ru/2.1/?apikey={YandexGoSettings.ApiMapKey}&lang=ru_RU\" type=\"text/javascript\"></script>";
        }

        public override OrderAddress GetSelectedOrderAddress()
        {
            var json = Request.Form["yandexGoDeliveryData"];
            if (string.IsNullOrWhiteSpace(json)) return null;

            var model = JsonConvert.DeserializeObject<YandexGoAddressInfo>(json);
            if (model == null) return null;

            OrderAddress = new OrderAddress()
            {
                Country = model.Address.Country,
                Region = model.Address.Region,
                City = model.Address.City,
                Street = model.Address.Street,
                House = model.Address.House,
                Longitude = model.Longitude,
                Latitude = model.Latitude,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Phone = model.Phone
            };
            return OrderAddress;
        }
    }
}