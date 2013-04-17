using System;
using System.Linq;
using System.Reflection;
using Ninject;
using Ninject.Extensions.Interception;
using ServiceInterfaces;

namespace Policies
{
    public class LogInterceptor : AttributeInterceptor<LogAttribute>
    {
        public override void Intercept(IInvocation invocation)
        {
            MethodInfo method = invocation.Request.Method;
            LogAttribute attribute = GetAttributes(invocation).SingleOrDefault();
            ILogger logger = invocation.Request.Kernel.Get<ILogger>();
            string logName = attribute == null ? "null" : attribute.LogName;
            try
            {             
                logger.Trace(logName, "Entering {0}.{1}", invocation.Request.Target.GetType(), method.Name);
                invocation.Proceed();
            }
            catch (Exception exception)
            {
                logger.ErrorException(logName, exception);
                throw;
            }
            finally
            {
                logger.Trace(logName, "Exiting {0}.{1}", invocation.Request.Target.GetType(), method.Name);
            }
        }
    }
}
