using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject.Extensions.Interception;
using NSubstitute;

namespace Policies.Tests
{
    [TestClass]
    public class ExceptionInterceptorTester
    {
        [TestMethod]
        public void ShouldWrapExceptionAndUseItsMessageByDefault()
        {
            ExceptionWrapper<Exception> wrapper = new ExceptionWrapper<Exception>((e, m) => new InvalidOperationException(m, e));

            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Target.Returns(typeof(NoAttribute));
            invocation.Request.Method.Returns(typeof(NoAttribute).GetMethod("DoSomething"));
            const string message = "Error message";
            invocation.When(i => i.Proceed()).Do(x => { throw new Exception(message); });
            try
            {
                wrapper.Intercept(invocation);
                Assert.Fail("The exception was swallowed.");
            }
            catch (InvalidOperationException exception)
            {
                Assert.AreEqual(message, exception.Message, "Did not use wrapped exception's message.");
            }
        }

        [TestMethod]
        public void ShouldUseDifferentErrorMessageWithAttribute()
        {
            ExceptionWrapper<Exception> wrapper = new ExceptionWrapper<Exception>((e, m) => new InvalidOperationException(m, e));

            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Target.Returns(typeof(WithAttribute));
            invocation.Request.Method.Returns(typeof(WithAttribute).GetMethod("DoSomething"));
            const string message = "Error message";
            invocation.When(i => i.Proceed()).Do(x => { throw new Exception(message); });
            try
            {
                wrapper.Intercept(invocation);
                Assert.Fail("The exception was swallowed.");
            }
            catch (InvalidOperationException exception)
            {
                Assert.AreEqual(WithAttribute.ErrorMessage, exception.Message);
            }
        }

        [TestMethod]
        public void ShouldDoNothingIfNoExceptionIsThrown()
        {
            ExceptionWrapper<Exception> wrapper = new ExceptionWrapper<Exception>((e, m) => e);

            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Target.Returns(typeof(WithAttribute));
            invocation.Request.Method.Returns(typeof(WithAttribute).GetMethod("DoSomething"));
            wrapper.Intercept(invocation);
        }

        [TestMethod]
        [ExpectedException(typeof(OutOfMemoryException))]
        public void ShouldDoNothingIfDifferentExceptionType()
        {
            ExceptionWrapper<InvalidOperationException> wrapper = new ExceptionWrapper<InvalidOperationException>((e, m) => e);
            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Target.Returns(typeof(WithAttribute));
            invocation.Request.Method.Returns(typeof(WithAttribute).GetMethod("DoSomething"));
            invocation.When(i => i.Proceed()).Do(x => { throw new OutOfMemoryException(); });
            wrapper.Intercept(invocation);
        }

        [TestMethod]
        public void ShouldUseClosestMatchWhenMultipleMessagesProvided()
        {
            ExceptionWrapper<Exception> wrapper = new ExceptionWrapper<Exception>((e, m) => new InvalidOperationException(m, e));

            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Target.Returns(typeof(WithMultipleAttributes));
            invocation.Request.Method.Returns(typeof(WithMultipleAttributes).GetMethod("DoSomething"));
            const string parameterName = "x";
            invocation.When(i => i.Proceed()).Do(x => { throw new ArgumentNullException(parameterName); });
            try
            {
                wrapper.Intercept(invocation);
                Assert.Fail("The exception was swallowed.");
            }
            catch (InvalidOperationException exception)
            {
                Assert.AreEqual(WithMultipleAttributes.DerivedErrorMessage, exception.Message);
            }
        }

        [TestMethod]
        public void ShouldIgnoreMessagesForMoreDerivedTypes()
        {
            ExceptionWrapper<Exception> wrapper = new ExceptionWrapper<Exception>((e, m) => new InvalidOperationException(m, e));

            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Target.Returns(typeof(WithMultipleAttributes));
            invocation.Request.Method.Returns(typeof(WithMultipleAttributes).GetMethod("DoSomething"));
            const string message = "Error message";
            invocation.When(i => i.Proceed()).Do(x => { throw new ArgumentException(message); });
            try
            {
                wrapper.Intercept(invocation);
                Assert.Fail("The exception was swallowed.");
            }
            catch (InvalidOperationException exception)
            {
                Assert.AreEqual(WithMultipleAttributes.BaseErrorMessage, exception.Message);
            }
        }

        [TestMethod]
        public void ShouldUseExceptionMessageWhenAllMessageForDerivedTypes()
        {
            ExceptionWrapper<Exception> wrapper = new ExceptionWrapper<Exception>((e, m) => new InvalidOperationException(m, e));

            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Target.Returns(typeof(WithMultipleAttributes));
            invocation.Request.Method.Returns(typeof(WithMultipleAttributes).GetMethod("DoSomething"));
            const string message = "Error message";
            invocation.When(i => i.Proceed()).Do(x => { throw new Exception(message); });
            try
            {
                wrapper.Intercept(invocation);
                Assert.Fail("The exception was swallowed.");
            }
            catch (InvalidOperationException exception)
            {
                Assert.AreEqual(message, exception.Message);
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
            public const string ErrorMessage = "A different error message.";

            [ErrorMessage(ErrorMessage)]
            public void DoSomething()
            {
            }
        }

        public class WithMultipleAttributes
        {
            public const string BaseErrorMessage = "This is the generic message.";
            public const string DerivedErrorMessage = "This is a more specific message.";

            [ErrorMessage(BaseErrorMessage, ExceptionType=typeof(ArgumentException))]
            [ErrorMessage(DerivedErrorMessage, ExceptionType=typeof(ArgumentNullException))]
            public void DoSomething()
            {
            }
        }
    }
}
