using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Adapters;
using ServiceInterfaces;

namespace TestMvcApplication.Context
{
    public class ContextManager : IUrlHelper, IPrincipalManager, IConfigurationManager, ILogger
    {
        private readonly RequestContext context;

        public ContextManager()
        {
            HttpContextWrapper wrapper = new HttpContextWrapper(HttpContext.Current);
            RouteData routeData = RouteTable.Routes.GetRouteData(wrapper);
            context = new RequestContext(wrapper, routeData);
        }

        public string UserName
        {
            get { return context.HttpContext.User.Identity.Name; }
        }

        public string ConnectionString
        {
            get 
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["EntityFramework"];
                return settings.ConnectionString;
            }
        }

        public string Action(ActionResult result)
        {
            UrlHelper helper = new UrlHelper(context);
            return helper.Action(result);
        }

        public bool IsSafe(string url)
        {
            UrlHelper helper = new UrlHelper(context);
            return helper.IsLocalUrl(url) && url.Length > 1 && url.StartsWith("/") && !url.StartsWith("//") && !url.StartsWith("/\\");
        }
    }
}