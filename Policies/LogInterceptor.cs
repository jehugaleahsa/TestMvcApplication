using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;
using Ninject.Extensions.Interception;
using NLog;

namespace Policies
{
    public class LogInterceptor : AttributeInterceptor<LogAttribute>
    {
        public override void Intercept(IInvocation invocation)
        {
            MethodInfo method = invocation.Request.Method;
            LogAttribute attribute = GetAttributes(invocation).SingleOrDefault();
            Logger logger = attribute == null ? LogManager.GetLogger("null") : LogManager.GetLogger(attribute.LogName ?? String.Empty);
            try
            {
                logger.Log(LogLevel.Trace, "Entering {0}.{1}", invocation.Request.Target.GetType(), method.Name);
                invocation.Proceed();
            }
            catch (Exception exception)
            {
                logger.LogException(LogLevel.Error, exception.Message, exception);
            }
            finally
            {
                logger.Log(LogLevel.Trace, "Exiting {0}.{1}", invocation.Request.Target.GetType(), method.Name);
            }
        }
    }
}
