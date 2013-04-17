using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Adapters;
using NLog;
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

        #region IPrincipalManager

        public string UserName
        {
            get { return context.HttpContext.User.Identity.Name; }
        }

        #endregion

        #region ConfigurationManager

        public string ConnectionString
        {
            get 
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["EntityFramework"];
                return settings.ConnectionString;
            }
        }

        #endregion

        #region UrlHelper

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

        #endregion

        #region Logger

        public void Trace(string logName, string message, params object[] arguments)
        {
            Logger logger = LogManager.GetLogger(logName ?? String.Empty);
            logger.Trace(message, arguments);
        }

        public void Debug(string logName, string message, params object[] arguments)
        {
            Logger logger = LogManager.GetLogger(logName ?? String.Empty);
            logger.Debug(message, arguments);
        }

        public void Info(string logName, string message, params object[] arguments)
        {
            Logger logger = LogManager.GetLogger(logName ?? String.Empty);
            logger.Info(message, arguments);
        }

        public void Error(string logName, string message, params object[] arguments)
        {
            Logger logger = LogManager.GetLogger(logName ?? String.Empty);
            logger.Error(message, arguments);
        }

        public void Fatal(string logName, string message, params object[] arguments)
        {
            Logger logger = LogManager.GetLogger(logName ?? String.Empty);
            logger.Fatal(message, arguments);
        }

        public void TraceException(string logName, Exception exception)
        {
            Logger logger = LogManager.GetLogger(logName ?? String.Empty);
            logger.TraceException(exception.Message, exception);
        }

        public void DebugException(string logName, Exception exception)
        {
            Logger logger = LogManager.GetLogger(logName ?? String.Empty);
            logger.DebugException(exception.Message, exception);
        }

        public void InfoException(string logName, Exception exception)
        {
            Logger logger = LogManager.GetLogger(logName ?? String.Empty);
            logger.InfoException(exception.Message, exception);
        }

        public void ErrorException(string logName, Exception exception)
        {
            Logger logger = LogManager.GetLogger(logName ?? String.Empty);
            logger.ErrorException(exception.Message, exception);
        }

        public void FatalException(string logName, Exception exception)
        {
            Logger logger = LogManager.GetLogger(logName ?? String.Empty);
            logger.FatalException(exception.Message, exception);
        }

        #endregion
    }
}