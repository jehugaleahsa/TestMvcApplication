using System;
using System.Reflection;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject.Extensions.Interception;
using NSubstitute;

namespace Policies.Tests
{
    [TestClass]
    public class TransactionInterceptorTester
    {
        [TestMethod]
        public void ShouldCommitTransactionIfExceptionNotThrown()
        {
            Thrower instance = new Thrower() { ShouldThrowException = false };
            TransactionStatus status = TransactionStatus.InDoubt;
            instance.Completed = (obj, e) =>
                {
                    status = e.Transaction.TransactionInformation.Status;
                };

            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Target.Returns(instance);
            MethodInfo method = typeof(Thrower).GetMethod("DoSomething");
            invocation.Request.Method.Returns(method);
            invocation.When(i => i.Proceed()).Do(i => instance.DoSomething());

            TransactionInterceptor interceptor = new TransactionInterceptor();
            interceptor.Intercept(invocation);

            Assert.AreEqual(TransactionStatus.Committed, status, "The transaction was not committed.");
        }

        [TestMethod]
        public void ShouldAbortTransactionIfExceptionThrown()
        {
            Thrower instance = new Thrower() { ShouldThrowException = true };
            TransactionStatus status = TransactionStatus.InDoubt;
            instance.Completed = (obj, e) =>
            {
                status = e.Transaction.TransactionInformation.Status;
            };

            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Target.Returns(instance);
            MethodInfo method = typeof(Thrower).GetMethod("DoSomething");
            invocation.Request.Method.Returns(method);
            invocation.When(i => i.Proceed()).Do(i => instance.DoSomething());

            try
            {
                TransactionInterceptor interceptor = new TransactionInterceptor();
                interceptor.Intercept(invocation);
                Assert.Fail("An exception should have been thrown.");
            }
            catch (Exception)
            {
                Assert.AreEqual(TransactionStatus.Aborted, status, "The transaction was not abandoned.");
            }
        }

        [TestMethod]
        public void ShouldNotCreateTransactionIfMissingAttribute()
        {
            NoAttribute instance = new NoAttribute();

            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Target.Returns(instance);
            MethodInfo method = typeof(NoAttribute).GetMethod("DoSomething");
            invocation.Request.Method.Returns(method);
            invocation.When(i => i.Proceed()).Do(i => instance.DoSomething());

            TransactionInterceptor interceptor = new TransactionInterceptor();
            interceptor.Intercept(invocation);

            Assert.IsFalse(instance.HadTransaction, "There should be no active transaction.");
        }

        public class NoAttribute
        {
            public void DoSomething()
            {
                HadTransaction = Transaction.Current != null;
            }

            public bool HadTransaction { get; set; }
        }

        public class Thrower
        {
            public bool ShouldThrowException { get; set; }

            public TransactionCompletedEventHandler Completed { get; set; }

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
