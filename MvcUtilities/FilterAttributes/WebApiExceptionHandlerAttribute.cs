using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace MvcUtilities.FilterAttributes
{
    public class WebApiExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            CodedException exception = actionExecutedContext.Exception as CodedException;
            if (exception == null)
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception);
            }
            else
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(exception.StatusCode, exception.Message, exception);
            }
            base.OnException(actionExecutedContext);
        }
    }
}
