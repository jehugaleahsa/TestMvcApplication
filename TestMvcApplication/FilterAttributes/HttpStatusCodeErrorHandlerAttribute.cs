using System;
using System.Net;
using System.Web.Mvc;
using Adapters;

namespace TestMvcApplication.FilterAttributes
{
    public class HttpStatusCodeErrorHandlerAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            AdapterException exception = filterContext.Exception as AdapterException;
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