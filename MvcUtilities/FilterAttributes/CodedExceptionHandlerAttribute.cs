using System;
using System.Net;
using System.Web.Mvc;

namespace MvcUtilities.FilterAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple=true, Inherited=true)]
    public sealed class CodedExceptionHandlerAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            CodedException exception = filterContext.Exception as CodedException;
            if (exception == null)
            {
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            else
            {
                filterContext.Result = new HttpStatusCodeResult(exception.StatusCode, exception.Message);
            }
            filterContext.ExceptionHandled = true;
            base.OnException(filterContext);
        }
    }
}