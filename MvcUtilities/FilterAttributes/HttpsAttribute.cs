using System;
using System.Web;
using System.Web.Mvc;

namespace TestMvcApplication.FilterAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public sealed class HttpsAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of a HttpsAttribute.
        /// </summary>
        /// <param name="isEnabled">Determines whether the action requires HTTPS.</param>
        public HttpsAttribute(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }

        public bool IsEnabled { get; private set; }

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            HttpRequestBase request = filterContext.HttpContext.Request;

            if (!request.IsLocal)
            {
                if (!request.IsSecureConnection && IsEnabled)
                {
                    Uri uri = request.Url;
                    UriBuilder builder = new UriBuilder(uri)
                    {
                        Scheme = Uri.UriSchemeHttps,
                        Port = 443,
                    };
                    string url = builder.Uri.ToString();
                    filterContext.Result = new RedirectResult(url);
                }
                else if (request.IsSecureConnection && !IsEnabled)
                {
                    Uri uri = request.Url;
                    UriBuilder builder = new UriBuilder(uri)
                    {
                        Scheme = Uri.UriSchemeHttp,
                        Port = 80,
                    };
                    string url = builder.Uri.ToString();
                    filterContext.Result = new RedirectResult(url);
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}