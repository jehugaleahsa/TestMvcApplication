using System;
using System.Transactions;

namespace Policies
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public sealed class TransactionAttribute : Attribute
    {
        public TransactionScopeOption TransactionScopeOption { get; set; }

        public IsolationLevel IsolationLevel { get; set; }

        public TimeSpan Timeout { get; set; }
    }
}
