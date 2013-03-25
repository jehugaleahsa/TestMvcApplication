using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject;
using Ninject.Extensions.Interception;
using ServiceInterfaces.Repositories;

namespace Policies
{
    public class SynchronizeInterceptor : AttributeInterceptor<SynchronizeAttribute>
    {
        public override void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            MethodInfo method = invocation.Request.Method;
            bool needsSynchronized = GetAttributes(invocation).Any();
            if (needsSynchronized)
            {
                ISynchronizer synchronizer = invocation.Request.Kernel.Get<ISynchronizer>();
                synchronizer.Synchronize();
            }
        }
    }
}