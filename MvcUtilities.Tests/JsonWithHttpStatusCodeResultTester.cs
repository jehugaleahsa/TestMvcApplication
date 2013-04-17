using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcUtilities.ActionResults;
using NSubstitute;

namespace MvcUtilities.Tests
{
    [TestClass]
    public class JsonWithHttpStatusCodeResultTester
    {
        [TestMethod]
        public void ShouldStoreStatusCode()
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            JsonWithHttpStatusCodeResult result = new JsonWithHttpStatusCodeResult(statusCode);
            Assert.AreEqual((int)statusCode, result.StatusCode, "The status code was not set.");
        }

        [TestMethod]
        public void ShouldSetTheResponseStatusCode()
        {
            HttpContextBase httpContext = Substitute.For<HttpContextBase>();
            RouteData routeData = new RouteData();
            ControllerBase controller = Substitute.For<ControllerBase>();
            ControllerContext context = new ControllerContext(httpContext, routeData, controller);

            JsonWithHttpStatusCodeResult result = new JsonWithHttpStatusCodeResult((int)HttpStatusCode.InternalServerError);
            result.ExecuteResult(context);

            Assert.AreEqual(result.StatusCode, httpContext.Response.StatusCode, "The status code was not set.");
        }

        [TestMethod]
        public void ShouldSetTheContentType()
        {
            HttpContextBase httpContext = Substitute.For<HttpContextBase>();
            RouteData routeData = new RouteData();
            ControllerBase controller = Substitute.For<ControllerBase>();
            ControllerContext context = new ControllerContext(httpContext, routeData, controller);

            JsonWithHttpStatusCodeResult result = new JsonWithHttpStatusCodeResult(HttpStatusCode.InternalServerError);
            result.ExecuteResult(context);

            Assert.AreEqual("application/json", httpContext.Response.ContentType, "The content type was not set.");
        }

        [TestMethod]
        public void ShouldJsonifyData()
        {
            using (StringWriter writer = new StringWriter())
            {
                HttpContextBase httpContext = Substitute.For<HttpContextBase>();
                httpContext.Response.When(c => c.Write(Arg.Any<string>())).Do(c => writer.Write(c.Arg<string>()));

                RouteData routeData = new RouteData();
                ControllerBase controller = Substitute.For<ControllerBase>();
                ControllerContext context = new ControllerContext(httpContext, routeData, controller);

                JsonWithHttpStatusCodeResult result = new JsonWithHttpStatusCodeResult(HttpStatusCode.InternalServerError);
                result.Data = new { Value = 123 };
                result.ExecuteResult(context);

                writer.Flush();
                string content = writer.ToString();
                Assert.AreEqual(@"{""Value"":123}", content, "The content was not written.");
            }
        }
    }
}
