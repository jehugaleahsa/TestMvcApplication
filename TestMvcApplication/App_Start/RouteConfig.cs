using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace TestMvcApplication
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Classic", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute("Error", "Error/Index");
        }
    }
}