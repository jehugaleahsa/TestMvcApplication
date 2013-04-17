using System.Web.Mvc;
using Policies;
using ServiceInterfaces;

namespace MvcUtilities.FilterAttributes
{
    public class TraceAttribute : ActionFilterAttribute
    {
        public string LogName { get; set; }

        [Inject]
        public ILogger Logger { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Logger.Trace(LogName,
                @"Entering {0}Controller.{1}",
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                filterContext.ActionDescriptor.ActionName);
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception != null && !filterContext.ExceptionHandled)
            {
                Logger.ErrorException(LogName, filterContext.Exception);
            }
            Logger.Trace(LogName,
                @"Exiting {0}Controller.{1}",
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                filterContext.ActionDescriptor.ActionName);
            base.OnActionExecuted(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.Exception != null && !filterContext.ExceptionHandled)
            {
                Logger.ErrorException(LogName, filterContext.Exception);
            }
            base.OnResultExecuted(filterContext);
        }
    }
}
