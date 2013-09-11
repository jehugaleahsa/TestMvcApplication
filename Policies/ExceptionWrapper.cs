using System;
using System.Linq;
using Ninject.Extensions.Interception;

namespace Policies
{
    public sealed class ExceptionWrapper<TSource> : AttributeInterceptor<ErrorMessageAttribute>
        where TSource : Exception
    {
        private readonly Func<TSource, string, Exception> wrapper;

        public ExceptionWrapper(Func<TSource, string, Exception> wrapper)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }
            this.wrapper = wrapper;
        }

        public override void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (TSource exception)
            {
                // use the most specific error message
                var attributes = from a in GetAttributes(invocation)
                                 where a.ExceptionType == null || a.ExceptionType.IsAssignableFrom(exception.GetType())
                                 select new
                                 {
                                     Message = a.ErrorMessage,
                                     ExceptionType = a.ExceptionType ?? typeof(Object),
                                 } into d
                                 orderby distanceToBase(d.ExceptionType, exception.GetType())
                                 select d;
                var attribute = attributes.FirstOrDefault();
                string message = attribute == null ? exception.Message : attribute.Message;
                Exception wrapped = wrapper(exception, message);
                throw wrapped;
            }
        }

        private static int distanceToBase(Type baseType, Type actualType)
        {
            int distance = 0;
            while (baseType != actualType)
            {
                ++distance;
                actualType = actualType.BaseType;
            }
            return distance;
        }
    }
}