using System;
using System.Net;
using Adapters;
using Policies;
using ServiceInterfaces;

namespace TestMvcApplication.Interceptors
{
    public class AdapterExceptionInterceptor : ExceptionInterceptor<ServiceException>
    {
        protected override Exception Wrap(ServiceException exception, string message)
        {
            return new AdapterException(HttpStatusCode.InternalServerError, message, exception);
        }
    }
}
