using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject.Extensions.Interception;
using NSubstitute;

namespace Policies.Tests
{
    [TestClass]
    public class AttributeInterceptorTester
    {
        [TestMethod]
        public void ShouldFindAttributeOnInterface()
        {
            CustomInterceptor interceptor = new CustomInterceptor();
            MethodInfo method = typeof(IInterfaceWithAttribute).GetMethod("DoSomething");
            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Method.Returns(method);
            ImplementsInterface instance = new ImplementsInterface();
            invocation.Request.Target.Returns(instance);
            CustomAttribute[] attributes = interceptor.GetAllAttributes(invocation);

            Assert.AreEqual(1, attributes.Length, "The wrong number of attributes were found.");
        }

        [TestMethod]
        public void ShouldFindAttributeOnAbstractClass()
        {
            CustomInterceptor interceptor = new CustomInterceptor();
            MethodInfo method = typeof(AbstractClassWithAttribute).GetMethod("DoSomething");
            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Method.Returns(method);
            ImplementsAbstractClass instance = new ImplementsAbstractClass();
            invocation.Request.Target.Returns(instance);
            CustomAttribute[] attributes = interceptor.GetAllAttributes(invocation);

            Assert.AreEqual(1, attributes.Length, "The wrong number of attributes were found.");
        }

        [TestMethod]
        public void ShouldFindAttributesInInterfaceAndClass()
        {
            CustomInterceptor interceptor = new CustomInterceptor();
            MethodInfo method = typeof(IInterfaceWithAttribute).GetMethod("DoSomething");
            IInvocation invocation = Substitute.For<IInvocation>();
            invocation.Request.Method.Returns(method);
            AttributeInheritedAndReproduced instance = new AttributeInheritedAndReproduced();
            invocation.Request.Target.Returns(instance);
            CustomAttribute[] attributes = interceptor.GetAllAttributes(invocation);

            Assert.AreEqual(2, attributes.Length, "The wrong number of attributes were found.");
        }

        [AttributeUsage(AttributeTargets.Method)]
        internal sealed class CustomAttribute : Attribute
        {
        }

        internal class CustomInterceptor : AttributeInterceptor<CustomAttribute>
        {
            public override void Intercept(IInvocation invocation)
            {
                if (invocation == null)
                {
                    throw new ArgumentNullException("invocation");
                }
                invocation.Proceed();
            }

            public CustomAttribute[] GetAllAttributes(IInvocation invocation)
            {
                return GetAttributes(invocation).ToArray();
            }
        }

        internal interface IInterfaceWithAttribute
        {
            [Custom]
            void DoSomething();
        }

        internal class ImplementsInterface : IInterfaceWithAttribute
        {
            public void DoSomething()
            {                
            }
        }

        internal class AttributeInheritedAndReproduced : IInterfaceWithAttribute
        {
            [Custom]
            public void DoSomething()
            {
            }
        }

        internal abstract class AbstractClassWithAttribute
        {
            [Custom]
            public abstract void DoSomething();
        }

        internal class ImplementsAbstractClass : AbstractClassWithAttribute
        {
            public override void DoSomething()
            {
            }
        }
    }
}
