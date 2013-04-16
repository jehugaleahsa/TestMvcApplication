using System;
using System.Linq;
using System.Web.Mvc;
using NLog;

namespace MvcUtilities.FilterAttributes
{
    public class TraceAttribute : ActionFilterAttribute
    {
        public string LogName { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Logger logger = LogManager.GetLogger(LogName ?? String.Empty);
            logger.Log(LogLevel.Trace,
                @"Entering {0}Controller.{1}",
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                filterContext.ActionDescriptor.ActionName);
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Logger logger = LogManager.GetLogger(LogName ?? String.Empty);
            if (filterContext.Exception != null && !filterContext.ExceptionHandled)
            {
                logger.LogException(LogLevel.Error, filterContext.Exception.Message, filterContext.Exception);
            }
            logger.Log(LogLevel.Trace,
                @"Exiting {0}Controller.{1}",
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                filterContext.ActionDescriptor.ActionName);
            base.OnActionExecuted(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.Exception != null && !filterContext.ExceptionHandled)
            {
                Logger logger = LogManager.GetLogger(LogName ?? String.Empty);
                logger.LogException(LogLevel.Error, filterContext.Exception.Message, filterContext.Exception);
            }
            base.OnResultExecuted(filterContext);
        }
    }
}
