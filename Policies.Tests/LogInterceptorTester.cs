using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Ninject.Extensions.Interception;
using NSubstitute;
using ServiceInterfaces;

namespace Policies.Tests
{
    [TestClass]
    public class LogInterceptorTester
    {
        [TestMethod]
        public void ShouldLogToNullWhenAttributeMissing()
        {
            NoAttribute instance = new NoAttribute();

            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Target.Returns(instance);
            MethodInfo method = typeof(NoAttribute).GetMethod("DoSomething");
            invocation.Request.Method.Returns(method);
            invocation.When(i => i.Proceed()).Do(i => instance.DoSomething());

            ILogger logger = Substitute.For<ILogger>();
            IKernel kernel = new StandardKernel();
            kernel.Bind<ILogger>().ToConstant(logger);
            invocation.Request.Kernel.Returns(kernel);

            LogInterceptor interceptor = new LogInterceptor();
            interceptor.Intercept(invocation);

            logger.Received().Trace("null", "Entering {0}.{1}", typeof(NoAttribute), "DoSomething");
            logger.Received().Trace("null", "Exiting {0}.{1}", typeof(NoAttribute), "DoSomething");
        }

        public class NoAttribute
        {
            public void DoSomething()
            {
            }
        }

        [TestMethod]
        public void ShouldLogToNamedLogWhenAttributeProvided()
        {
            WithAttribute instance = new WithAttribute();

            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Target.Returns(instance);
            MethodInfo method = typeof(WithAttribute).GetMethod("DoSomething");
            invocation.Request.Method.Returns(method);
            invocation.When(i => i.Proceed()).Do(i => instance.DoSomething());

            ILogger logger = Substitute.For<ILogger>();
            IKernel kernel = new StandardKernel();
            kernel.Bind<ILogger>().ToConstant(logger);
            invocation.Request.Kernel.Returns(kernel);

            LogInterceptor interceptor = new LogInterceptor();
            interceptor.Intercept(invocation);

            logger.Received().Trace(WithAttribute.LogName, "Entering {0}.{1}", typeof(WithAttribute), "DoSomething");
            logger.Received().Trace(WithAttribute.LogName, "Exiting {0}.{1}", typeof(WithAttribute), "DoSomething");
        }

        public class WithAttribute
        {
            public const string LogName = "Test";

            [Log(LogName = LogName)]
            public void DoSomething()
            {
            }
        }

        [TestMethod]
        public void ShouldLogExceptions()
        {
            Throws instance = new Throws();

            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Target.Returns(instance);
            MethodInfo method = typeof(Throws).GetMethod("DoSomething");
            invocation.Request.Method.Returns(method);
            invocation.When(i => i.Proceed()).Do(i => instance.DoSomething());

            ILogger logger = Substitute.For<ILogger>();
            IKernel kernel = new StandardKernel();
            kernel.Bind<ILogger>().ToConstant(logger);
            invocation.Request.Kernel.Returns(kernel);

            LogInterceptor interceptor = new LogInterceptor();
            try
            {
                interceptor.Intercept(invocation);
                Assert.Fail("The LogInterceptor swallowed the exception.");
            }
            catch (Exception exception)
            {
                logger.Received().Trace(Throws.LogName, "Entering {0}.{1}", typeof(Throws), "DoSomething");
                logger.Received().ErrorException(Throws.LogName, exception);
                logger.Received().Trace(Throws.LogName, "Exiting {0}.{1}", typeof(Throws), "DoSomething");
            }
        }

        public class Throws
        {
            public const string LogName = "Test";

            [Log(LogName = LogName)]
            public void DoSomething()
            {
                throw new Exception();
            }
        }
    }
}
