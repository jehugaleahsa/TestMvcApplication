using System;
using System.Web;
using Adapters;
using DataModeling.DataModel;
using DataModeling.Repositories;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Interception.Infrastructure.Language;
using Ninject.Modules;
using Ninject.Web.Common;
using Policies;
using ServiceInterfaces;
using ServiceInterfaces.Repositories;
using TestMvcApplication.Context;
using TestMvcApplication.Controllers;
using TestMvcApplication.Interceptors;

[assembly: WebActivator.PreApplicationStartMethod(typeof(TestMvcApplication.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(TestMvcApplication.NinjectWebCommon), "Stop")]

namespace TestMvcApplication
{
    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            IKernel kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Load<TestModule>();
        }        
    }

    public class TestModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ContextManager>().ToSelf().InRequestScope();
            Bind<IUrlHelper>().To<ContextManager>();
            Bind<IPrincipalManager>().To<ContextManager>();
            Bind<IConfigurationManager>().To<ContextManager>();
            Bind<ILogger>().To<ContextManager>();

            Bind<EntitySet>().ToMethod(getEntitySet).InRequestScope();

            var customerRepositoryBinding = Bind<ICustomerRepository>().To<CustomerRepository>();
            customerRepositoryBinding.Intercept().With<DataExceptionInterceptor>().InOrder(1);
            customerRepositoryBinding.Intercept().With<LogInterceptor>().InOrder(2);

            var adapterBinding = Bind<ICustomerAdapter>().To<CustomerAdapter>();
            adapterBinding.Intercept().With<AdapterExceptionInterceptor>().InOrder(1);
            adapterBinding.Intercept().With<TransactionInterceptor>().InOrder(2);
            adapterBinding.Intercept().With<LogInterceptor>().InOrder(3);

            Bind<ErrorController>().ToSelf();
            Bind<ClassicController>().ToSelf();
            Bind<KnockoutController>().ToSelf();
        }

        private static EntitySet getEntitySet(IContext context)
        {
            IConfigurationManager manager = context.Kernel.Get<IConfigurationManager>();
            return new EntitySet(manager.ConnectionString);
        }
    }
}
