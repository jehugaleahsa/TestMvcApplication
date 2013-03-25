using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject.Extensions.Interception;

namespace Policies
{
    public abstract class ExceptionInterceptor<TSourceException> : AttributeInterceptor<ErrorMessageAttribute>
        where TSourceException : Exception
    {
        public ExceptionInterceptor()
        {
        }

        public override void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (TSourceException exception)
            {
                MethodInfo method = invocation.Request.Method;
                ErrorMessageAttribute attribute = GetAttributes(invocation).SingleOrDefault();
                string message = attribute == null ? exception.Message : attribute.ErrorMessage;
                Rethrow(exception, message);
            }
        }

        protected abstract void Rethrow(TSourceException exception, string message);
    }
}