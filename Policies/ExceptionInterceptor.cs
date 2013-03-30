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
                ErrorMessageAttribute attribute = GetAttributes(invocation).SingleOrDefault();
                string message = attribute == null ? exception.Message : attribute.ErrorMessage;
                throw Wrap(exception, message);
            }
        }

        protected abstract Exception Wrap(TSourceException exception, string message);
    }
}