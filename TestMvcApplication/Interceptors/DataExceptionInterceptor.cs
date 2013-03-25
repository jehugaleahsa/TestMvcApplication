using System;
using Policies;
using ServiceInterfaces;

namespace TestMvcApplication.Interceptors
{
    public class DataExceptionInterceptor : ExceptionInterceptor<Exception>
    {
        protected override void Rethrow(Exception exception, string message)
        {
            throw new ServiceException(message, exception);
        }
    }
}
