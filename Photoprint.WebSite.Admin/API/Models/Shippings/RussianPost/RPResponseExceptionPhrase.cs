using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Photoprint.WebSite.Admin.API.Models
{
    public class RPResponseExceptionPhrase
    {
        public static string BadRequest => "Urls params cannot be empty!";
        public static string PhotolabNotFound => "photolab not found!";
        public static string CompanyAccountNotFound => "company account not found!";
        public static string OrderNotFound => "order not found!";
        public static string OrderRegisterError => "Can't register order in delivery servise";
        public static string OrderUnRegisterError => "Can't unregister order in delivery servise";

    }
}