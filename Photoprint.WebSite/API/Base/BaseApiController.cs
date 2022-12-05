using System.Net.Http;
using System.Web;
using System.Web.Http;
using Photoprint.Core.Services;

namespace Photoprint.WebSite.API
{
	public class BaseApiController : ApiController
	{
        protected IAuthenticationService AuthenticationService { get; }

        public BaseApiController(IAuthenticationService authenticationService)
		{
			AuthenticationService = authenticationService;
		}


		public HttpContextWrapper GetHttpContext(HttpRequestMessage request = null)
		{
			request = request ?? Request;

			if (request.Properties.ContainsKey("MS_HttpContext"))
			{
				return ((HttpContextWrapper)request.Properties["MS_HttpContext"]);
			}
			else if (HttpContext.Current != null)
			{
				return new HttpContextWrapper(HttpContext.Current);
			}
			else
			{
				return null;
			}
		}

        protected static class ExceptionPhrase
        {
            public static string UserNotAutorized => "user must be logged in!";
            public static string CompanyAccountNotFound => "account not found!";
            public static string PhotolabNotFound => "photolab not found!";
            public static string UserNotFound => "user not found!";
            public static string BadRequest => "bad request!";
            public static string Forbidden => "you don't have permission to access!";
            public static string MaterialTypeNotFound => "material type not found!";
        }
	}
}