using System.Net;
using System.Web.Mvc;

namespace MvcUtilities.FilterAttributes
{
    public class HttpStatusCodeErrorHandlerAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
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