using System;
using System.Configuration;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

namespace TestMvcApplication
{
    public class ContextManager
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
    }
}