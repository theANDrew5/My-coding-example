using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Photoprint.Core.Configuration;
using Photoprint.Core.Infrastructure.Cache;
using Photoprint.Core.Infrastructure.Logs;
using Photoprint.Core.Services;
using Photoprint.WebSite.Controls;
using Photoprint.WebSite.Modules;
using Photoprint.WebSite.Shared;

namespace Photoprint.WebSite.API
{
    public class AdminController : BaseApiController
    {
        private readonly ICacheService _cacheService;
        private readonly IPhotolabService _photolabService;
        private readonly ILogService _logService;
        private readonly IUserService _userService;
        private readonly IPhotolabDomainService _domainService;

        public AdminController(IAuthenticationService authenticationService, ICacheService cacheService,
            IPhotolabService photolabService, ILogService logService, IUserService userService, IPhotolabDomainService domainService) : base(authenticationService)
        {
            _cacheService = cacheService;
            _photolabService = photolabService;
            _logService = logService;
            _userService = userService;
            _domainService = domainService;
        }

        [HttpGet]
        [Route("api/admin/signInAs")]
        public HttpResponseMessage SignInAs(string key, int photolabId)
        {
            var currentFrontend = WebSiteGlobal.CurrentPhotolab;
            if (currentFrontend == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Frontend not found");

            var data = Core.Models.User.GetFromString(key);
            if (data == null || !data.IsValid) 
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Key invalid");

            var user = _userService.GetById(data.Id);
            if (user == null) 
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User not found");

            if (user.CompanyAccountId != currentFrontend.CompanyAccountId)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User not from this Frontend");

            AuthenticationService.SignIn(user, false, currentFrontend, false, true);

            var response = Request.CreateResponse(HttpStatusCode.Redirect);
            var photolab = _photolabService.GetById(photolabId);
            var domain = _domainService.GetDefaultDomain(photolab).Domain;

            var url = $"https://{domain}";
            response.Headers.Location = new Uri(url);
            return response;
        }

        [HttpGet]
        [Route("api/admin/generateError")]
        public HttpResponseMessage GenerateError()
        {
            throw new Exception("Generate error " + DateTime.UtcNow);
        }
        
        [HttpGet]
        [Route("api/admin/version")]
        public HttpResponseMessage Version()
        {
            return Request.CreateResponse(HttpStatusCode.OK, WebSiteGlobal.AssemblyBuildDate.ToString("HH:mm, dd.MM.yyyy"));
        }

        [HttpGet]
        [Route("api/admin/redis")]
        public HttpResponseMessage GetRedisStatus()
        {
            var redisState = _cacheService.GetRedisStatus();
            return Request.CreateResponse(HttpStatusCode.OK, redisState);
        }

        [HttpGet]
        [Route("api/admin/cache/info")]
        public HttpResponseMessage CacheInfo(string secret)
        {
            if (string.IsNullOrWhiteSpace(secret) || !secret.Equals(Settings.AdminToSaasApiSecret, StringComparison.InvariantCultureIgnoreCase))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Secret invalid");
            }
            try
            {
                var caches = PixlparkCacheManager.GetInstances();
                var result = new List<PixlparkCacheInfo>();
                foreach (var cache in caches)
                {
                    result.Add(cache.GetCacheInfo());
                }

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }



        [HttpGet]
        [Route("api/admin/cache/clearAll")]
        public object ClearCache(string secret)
        {
            if (string.IsNullOrWhiteSpace(secret) || !secret.Equals(Settings.AdminToSaasApiSecret, StringComparison.InvariantCultureIgnoreCase))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Secret invalid");
            }
            try
            {
                _cacheService.ClearAll();
                MaterialCmsRenderer.ClearCache();
                PriceTextManager.ClearCache();
                GiftsCmsRenderer.ClearCache();
                return new {Success = true, Messege = string.Empty};
            }
            catch (Exception ex)
            {
                return new {Success = false, Messege = ex.Message};
            }
        }

        [HttpGet]
        [Route("api/admin/cache/clear")]
        public object ClearCache(int frontendId, string secret, string tag = null)
        {
            if (string.IsNullOrWhiteSpace(secret) ||
                !secret.Equals(Settings.AdminToSaasApiSecret, StringComparison.InvariantCultureIgnoreCase))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Secret invalid");
            }

            try
            {
                var frontend = _photolabService.GetById(frontendId);
                if (tag != null)
                {
                    _cacheService.Invalidate(tag);
                }
                else if (frontend != null)
                {
                    if(frontend.CompanyAccountId == 1056)
                    {
                        var frontends = _photolabService.GetList(frontend.CompanyAccount);

                        foreach(var photolab in frontends)
                        {
                            MaterialCmsRenderer.ClearCache(photolab);
                            PriceTextManager.ClearCache(photolab);
                            GiftsCmsRenderer.ClearCache(photolab);
                            _cacheService.InvalidateFrontend(photolab);
                        }
                    }
                    else
                    {
                        MaterialCmsRenderer.ClearCache(frontend);
                        PriceTextManager.ClearCache(frontend);
                        GiftsCmsRenderer.ClearCache(frontend);
                        _cacheService.InvalidateFrontend(frontend);
                    }
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Lab not found");
                }
                return new {Success = true, Messege = string.Empty};
            }
            catch (Exception ex)
            {
                return new {Success = false, Messege = ex.Message};
            }
        }

        [HttpGet]
        [ApiAuthorize]
        [Route("api/admin/cache")]
        public object GetCache()
        {
            try
            {
                if (!AuthenticationService.LoggedInUser.IsAdministrator)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "No access");
                }

                var content = _cacheService.GetContent();
                var result = new Dictionary<string, string>();
                var keys = content.Select(item => item.Key.ToString()).OrderBy(k => k).ToList();
                var tags = new List<string>();
                foreach (var key in keys)
                {
                    if (!key.StartsWith("__System.Web.WebPages.Deployment") && !key.StartsWith("_tag"))
                        result.Add(key, _cacheService.Get(key)?.GetType().ToString() ?? "null");

                    if (key.StartsWith("_tag"))
                    {
                        tags.Add(key.Replace("_tag:", string.Empty));
                    }
                }
                return new
                {
                    keys = result,
                    tags = tags
                };
            }
            catch (Exception ex)
            {
                return new {Success = false, Messege = ex.Message};
            }
        }

        [HttpGet]
        [ApiAuthorize]
        [Route("api/admin/cache")]
        public object GetCache(string key)
        {
            try
            {
                if (!AuthenticationService.LoggedInUser.IsAdministrator)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "No access");
                }

                var @object = _cacheService.Get(key);

                return new
                {
                    key,
                    @object
                };
            }
            catch (Exception ex)
            {
                return new {Success = false, Messege = ex.Message};
            }
        }

        [HttpGet]
        [Route("api/admin/cache/urls")]
        public object UrlsCache(string secret)
        {
            if (string.IsNullOrWhiteSpace(secret) || !secret.Equals(Settings.AdminToSaasApiSecret, StringComparison.Ordinal)) return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Secret invalid");
            
            var ulrCache = UrlManagerInitializer.GetUrlCache();

            var sb = new StringBuilder(10000);
            sb.Append(@"<html><head><style>
    body,html{padding:0;margin:0;}
    table{border:solid 1px #ddd;border-collapse:collapse;width:100%}
    td {padding:0 6px;white-space:nowrap;font-size:11px;font-family:monospace;border:solid 1px #ddd;vertical-align:top;}
    th {font-size: 11px;font-family: sans-serif;border: solid 1px #ddd;padding: 0 20px;background: #f1f1f1;}
    .params{font-size:9px;}
</style>
</head>
<body>
<table>");
            sb.Append("<thead><tr><th>Path</th><th>Handler</th><th>Params</th></tr></thead>");
            foreach (var urlCache in ulrCache)
            {
                var key = urlCache.Key;
                var parsedUrl = urlCache.Value;
                var nodeName = parsedUrl.Node?.ToString() ?? "-";
                var @params = parsedUrl.ParsedParams;

                IReadOnlyCollection<string> parametersStrings = Array.Empty<string>();
                if (@params != null)
                {
                    var parsedParams = new List<string>(@params.Count);
                    foreach (var param in @params)
                    {
                        parsedParams.Add(param.Key + ": " + param.Value);
                    }
                    parametersStrings = parsedParams;
                }

                var paramsString = parametersStrings.Count > 0 ? string.Join("<br/>", parametersStrings) : "-";
                sb.Append("<tr><td>")
                    .Append(key)
                    .Append("</td><td>")
                    .Append(nodeName)
                    .Append("</td><td class='params'>")
                    .Append(paramsString)
                    .Append("</td></tr>");
            }

            sb.Append(@"</table>
</body>
</html>");
            return new HttpResponseMessage
            {
                Content = new StringContent(sb.ToString(), Encoding.UTF8, "text/html")
            };
        }

        [HttpPost]
        [Route("api/admin/log/js/errors/write")]
        public HttpResponseMessage WriteError(JSErrorRequest request)
        {
            if (request == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "no data");
            var userId = AuthenticationService?.LoggedInUser?.Id; 
            var userAgent = Request.Headers?.UserAgent?.ToString();
            var ip = SiteUtils.GetIpAddress(new HttpRequestWrapper(HttpContext.Current.Request));
            var photolab = WebSiteGlobal.CurrentPhotolab;
            var error = new JsLogErrorMessage(photolab, request.Msg, userId, request.Stack, request.LineNum, request.Url, userAgent, ip);
            if (Settings.IsProduction && error.Source == LogMessageSource.JSVectorEditor)
            {
                _logService.LogError(error);
            }
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }
       
        public class JSErrorRequest
        {
            public string Msg { get; set; }
            public string Stack { get; set; }
            public string Url { get; set; }
            public int LineNum { get; set; }
        }
    }
}