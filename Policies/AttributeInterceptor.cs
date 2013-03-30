using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject.Extensions.Interception;

namespace Policies
{
    public abstract class AttributeInterceptor<TAttribute> : IInterceptor
    {
        private static readonly Dictionary<Tuple<Type, MethodInfo>, TAttribute[]> cache = new Dictionary<Tuple<Type, MethodInfo>, TAttribute[]>();

        public abstract void Intercept(IInvocation invocation);

        protected virtual IEnumerable<TAttribute> GetAttributes(IInvocation invocation)
        {
            Type targetType = invocation.Request.Target.GetType();
            MethodInfo method = invocation.Request.Method;
            Tuple<Type, MethodInfo> key = Tuple.Create(targetType, method);
            TAttribute[] attributes;
            if (cache.TryGetValue(key, out attributes))
            {
                return attributes;
            }
            List<TAttribute> set = new List<TAttribute>();
            MethodInfo targetMethod = method;
            if (method.DeclaringType.IsInterface)
            {
                set.AddRange(method.GetCustomAttributes(true).OfType<TAttribute>());
                InterfaceMapping mapping = targetType.GetInterfaceMap(method.DeclaringType);
                int index = Array.IndexOf(mapping.InterfaceMethods, method);
                targetMethod = mapping.TargetMethods[index];
            }
            set.AddRange(targetMethod.GetCustomAttributes(true).OfType<TAttribute>());
            attributes = set.ToArray();
            cache[key] = attributes;
            return attributes;
        }
    }
}
