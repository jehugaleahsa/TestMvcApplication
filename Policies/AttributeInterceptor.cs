using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject.Extensions.Interception;

namespace Policies
{
    public abstract class AttributeInterceptor<TAttribute> : IInterceptor
    {
        private static readonly Dictionary<MethodInfo, TAttribute[]> cache = new Dictionary<MethodInfo, TAttribute[]>();

        public abstract void Intercept(IInvocation invocation);

        protected virtual IEnumerable<TAttribute> GetAttributes(IInvocation invocation)
        {
            MethodInfo method = invocation.Request.Method;
            TAttribute[] attributes;
            if (!cache.TryGetValue(method, out attributes))
            {
                List<TAttribute> set = new List<TAttribute>();
                MethodInfo targetMethod = method;
                if (method.DeclaringType.IsInterface)
                {
                    set.AddRange(method.GetCustomAttributes(true).OfType<TAttribute>());
                    Type targetType = invocation.Request.Target.GetType();
                    InterfaceMapping mapping = targetType.GetInterfaceMap(method.DeclaringType);
                    int index = Array.IndexOf(mapping.InterfaceMethods, method);
                    targetMethod = mapping.TargetMethods[index];
                }
                set.AddRange(targetMethod.GetCustomAttributes(true).OfType<TAttribute>());
                attributes = set.ToArray();
                cache[method] = attributes;
            }
            return attributes;
        }
    }
}
