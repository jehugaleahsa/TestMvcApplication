using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace MvcUtilities.FilterAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public sealed class WebApiExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext == null)
            {
                throw new ArgumentNullException("actionExecutedContext");
            }
            HttpRequestMessage request = actionExecutedContext.Request;
            CodedException exception = actionExecutedContext.Exception as CodedException;
            if (exception == null)
            {
                actionExecutedContext.Response = request.CreateErrorResponse(HttpStatusCode.InternalServerError, actionExecutedContext.Exception);
            }
            else
            {
                actionExecutedContext.Response = request.CreateErrorResponse(exception.StatusCode, exception);
            }
        }
    }
}
