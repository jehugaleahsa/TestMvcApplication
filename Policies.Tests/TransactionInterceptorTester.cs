using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject.Extensions.Interception;
using NSubstitute;
using System.Reflection;
using System.Transactions;

namespace Policies.Tests
{
    [TestClass]
    public class TransactionInterceptorTester
    {
        [TestMethod]
        public void ShouldCommitTransactionIfExceptionNotThrown()
        {
            Thrower instance = new Thrower() { ShouldThrowException = false };
            bool isCompleted = false;
            instance.Completed = (obj, e) =>
                {
                    Assert.AreEqual(
                        TransactionStatus.Committed, 
                        e.Transaction.TransactionInformation.Status,
                        "The transaction was not committed.");
                    isCompleted = true;
                };
            TransactionInterceptor interceptor = new TransactionInterceptor();
            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.When(i => i.Proceed()).Do(i => instance.DoSomething());
            invocation.Request.Target.Returns(instance);
            MethodInfo method = typeof(Thrower).GetMethod("DoSomething");
            invocation.Request.Method.Returns(method);
            interceptor.Intercept(invocation);
            Assert.IsTrue(isCompleted, "The transaction was never created.");
        }

        [TestMethod]
        public void ShouldAbortTransactionIfExceptionThrown()
        {
            Thrower instance = new Thrower() { ShouldThrowException = true };
            bool isCompleted = false;
            instance.Completed = (obj, e) =>
            {
                Assert.AreEqual(
                    TransactionStatus.Aborted,
                    e.Transaction.TransactionInformation.Status,
                    "The transaction was not abandoned.");
                isCompleted = true;
            };
            TransactionInterceptor interceptor = new TransactionInterceptor();
            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.When(i => i.Proceed()).Do(i => instance.DoSomething());
            invocation.Request.Target.Returns(instance);
            MethodInfo method = typeof(Thrower).GetMethod("DoSomething");
            invocation.Request.Method.Returns(method);
            try
            {
                interceptor.Intercept(invocation);
                Assert.Fail("An exception should have been thrown.");
            }
            catch (Exception)
            {
            }
            Assert.IsTrue(isCompleted, "The transaction was never created.");
        }

        [TestMethod]
        public void ShouldNotCreateTransactionIfMissingAttribute()
        {
            NoAttribute instance = new NoAttribute();
            TransactionInterceptor interceptor = new TransactionInterceptor();
            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.When(i => i.Proceed()).Do(i => instance.DoSomething());
            invocation.Request.Target.Returns(instance);
            MethodInfo method = typeof(NoAttribute).GetMethod("DoSomething");
            invocation.Request.Method.Returns(method);
            interceptor.Intercept(invocation);
        }

        public class NoAttribute
        {
            public void DoSomething()
            {
                Assert.IsNull(Transaction.Current, "There should be no active transaction.");
            }
        }

        public class Thrower
        {
            public bool ShouldThrowException
            {
                get;
                set;
            }

            public TransactionCompletedEventHandler Completed
            {
                get;
                set;
            }

            [Transaction]
            public void DoSomething()
            {
                Transaction.Current.TransactionCompleted += Completed;
                if (ShouldThrowException)
                {
                    throw new Exception();
                }
            }
        }
    }
}
