namespace Photoprint.WebSite.API.Models.Delivery
{
    public static class DeliveryResponseExceptionPhrase
    {
        public static string BadRequest => "Urls params cannot be empty!";
        public static string PhotolabNotFound => "photolab not found!";
        public static string CompanyAccountNotFound => "company account not found!";
        public static string UserUnauthorized => "user must be logged in!";
        public static string UserNotFound => "user not found!";
        public static string CityNotFound => "city not found!";
        public static string CartIsEmpty => "cart is empty!";
        public static string ShippingWindowSettingsError => "shipping window settings not found!";
        public static string ShippingNotFound => "shipping not found!";
        public static string WrongCoordinates => "wrong coordinates!";
        public static string ApiUrlLoadIsWarning => "api url load error!";
    }
}