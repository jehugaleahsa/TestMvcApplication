using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ninject.Extensions.Interception;

namespace Policies.Tests
{
    [TestClass]
    public class ExceptionInterceptorTester
    {
        [TestMethod]
        public void ShouldWrapException()
        {
            ExceptionWrapper wrapper = new ExceptionWrapper();

            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Target.Returns(typeof(NoAttribute));
            invocation.Request.Method.Returns(typeof(NoAttribute).GetMethod("DoSomething"));
            const string message = "Error message";
            invocation.When(i => i.Proceed()).Do(x => { throw new Exception(message); });
            try
            {
                wrapper.Intercept(invocation);
            }
            catch (InvalidOperationException exception)
            {
                Assert.AreEqual(message, exception.Message);
            }
        }

        [TestMethod]
        public void ShouldUseDifferentErrorMessageWithAttribute()
        {
            ExceptionWrapper wrapper = new ExceptionWrapper();

            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Target.Returns(typeof(WithAttribute));
            invocation.Request.Method.Returns(typeof(WithAttribute).GetMethod("DoSomething"));
            const string message = "Error message";
            invocation.When(i => i.Proceed()).Do(x => { throw new Exception(message); });
            try
            {
                wrapper.Intercept(invocation);
            }
            catch (InvalidOperationException exception)
            {
                Assert.AreEqual("A different error message.", exception.Message);
            }
        }

        [TestMethod]
        public void ShouldDoNothingIfNoExceptionIsThrown()
        {
            ExceptionWrapper wrapper = new ExceptionWrapper();

            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Target.Returns(typeof(WithAttribute));
            invocation.Request.Method.Returns(typeof(WithAttribute).GetMethod("DoSomething"));
            wrapper.Intercept(invocation);
        }

        [TestMethod]
        [ExpectedException(typeof(OutOfMemoryException))]
        public void ShouldDoNothingIfDifferentExceptionType()
        {
            InvalidOperationExceptionWrapper wrapper = new InvalidOperationExceptionWrapper();

            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Target.Returns(typeof(WithAttribute));
            invocation.Request.Method.Returns(typeof(WithAttribute).GetMethod("DoSomething"));
            invocation.When(i => i.Proceed()).Do(x => { throw new OutOfMemoryException(); });
            wrapper.Intercept(invocation);
        }

        public class ExceptionWrapper : ExceptionInterceptor<Exception>
        {
            protected override Exception Wrap(Exception exception, string message)
            {
                return new InvalidOperationException(message, exception);
            }
        }

        public class NoAttribute
        {
            public void DoSomething()
            {
            }
        }

        public class WithAttribute
        {
            [ErrorMessage("A different error message.")]
            public void DoSomething()
            {
            }
        }

        public class InvalidOperationExceptionWrapper : ExceptionInterceptor<InvalidOperationException>
        {
            protected override Exception Wrap(InvalidOperationException exception, string message)
            {
                return exception;
            }
        }
    }
}
