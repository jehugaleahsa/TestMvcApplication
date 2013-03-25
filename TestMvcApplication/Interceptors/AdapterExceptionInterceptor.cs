using System;
using System.Net;
using Adapters;
using Policies;
using ServiceInterfaces;

namespace TestMvcApplication.Interceptors
{
    public class AdapterExceptionInterceptor : ExceptionInterceptor<ServiceException>
    {
        protected override void Rethrow(ServiceException exception, string message)
        {
            throw new AdapterException(HttpStatusCode.InternalServerError, message, exception);
        }
    }
}
