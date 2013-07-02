using System.Linq;
using System.Web.Http;

namespace TestMvcApplication
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Uncomment out the following code to use JSON as the default
            //var formatters = from formatter in config.Formatters
            //                 from mediaType in formatter.SupportedMediaTypes
            //                 where mediaType.MediaType == "application/xml" || mediaType.MediaType == "text/xml"
            //                 select new { Formatter = formatter, MediaType = mediaType } into pair
            //                 group pair by pair.Formatter into formatterGroup
            //                 where formatterGroup.Any()
            //                 select formatterGroup.Key;
            //foreach (var formatter in formatters)
            //{
            //    config.Formatters.Remove(formatter);
            //}
        }
    }
}
