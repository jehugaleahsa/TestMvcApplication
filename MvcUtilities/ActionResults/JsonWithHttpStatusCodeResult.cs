using System;
using System.Net;
using System.Web.Mvc;

namespace MvcUtilities.ActionResults
{
    public class JsonWithHttpStatusCodeResult : JsonResult
    {
        public JsonWithHttpStatusCodeResult(int statusCode)
        {
            StatusCode = statusCode;
        }

        public JsonWithHttpStatusCodeResult(HttpStatusCode statusCode)
        {
            StatusCode = (int)statusCode;
        }

        public int StatusCode { get; private set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            context.HttpContext.Response.StatusCode = StatusCode;
            base.ExecuteResult(context);
        }
    }
}