using System;
using System.Linq;
using System.Transactions;
using Ninject.Extensions.Interception;

namespace Policies
{
    public class TransactionInterceptor : AttributeInterceptor<TransactionAttribute>
    {
        public override void Intercept(IInvocation invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException("invocation");
            }
            TransactionAttribute attribute = GetAttributes(invocation).SingleOrDefault();
            if (attribute == null)
            {
                invocation.Proceed();
            }
            else
            {
                TransactionOptions options = new TransactionOptions()
                {
                    IsolationLevel = attribute.IsolationLevel,
                    Timeout = attribute.Timeout,
                };
                using (TransactionScope scope = new TransactionScope(attribute.TransactionScopeOption, options))
                {
                    invocation.Proceed();
                    scope.Complete();
                }
            }
        }
    }
}
