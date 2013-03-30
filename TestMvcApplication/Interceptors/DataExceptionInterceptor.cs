using System;
using Policies;
using ServiceInterfaces;

namespace TestMvcApplication.Interceptors
{
    public class DataExceptionInterceptor : ExceptionInterceptor<Exception>
    {
        protected override Exception Wrap(Exception exception, string message)
        {
            return new ServiceException(message, exception);
        }
    }
}
