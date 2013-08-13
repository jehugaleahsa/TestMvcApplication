using System;
using System.Web;
using System.Web.Http;
using Adapters;
using DataModeling.DataModel;
using DataModeling.Repositories;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using MvcUtilities.FilterAttributes;
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
using Ninject.Syntax;

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
            IKernel kernel = new StandardKernel(new NinjectSettings() { InjectAttribute = typeof(Policies.InjectAttribute) });
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            RegisterServices(kernel);

            // Enables dependency lookup for Web API
            GlobalConfiguration.Configuration.DependencyResolver = new NinjectWebApiDependencyResolver(kernel);

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

            Bind<TraceAttribute>().ToSelf();

            var customerRepositoryBinding = Bind<ICustomerRepository>().To<CustomerRepository>();
            setupRepositoryInterceptors(customerRepositoryBinding);

            var settingRepositoryBinding = Bind<ICustomerSettingRepository>().To<CustomerSettingRepository>();
            setupRepositoryInterceptors(customerRepositoryBinding);

            var customerAdapterBinding = Bind<ICustomerAdapter>().To<CustomerAdapter>();
            setupAdapterInterceptors(customerAdapterBinding);

            var settingAdapterBinding = Bind<ISettingAdapter>().To<SettingAdapter>();
            setupAdapterInterceptors(settingAdapterBinding);

            Bind<ErrorController>().ToSelf();
            Bind<ClassicController>().ToSelf();
            Bind<KnockoutController>().ToSelf();
            Bind<CustomersController>().ToSelf();
        }

        private static void setupRepositoryInterceptors<TRepository>(IBindingWhenInNamedWithOrOnSyntax<TRepository> binding)
        {
            binding.Intercept().With<DataExceptionInterceptor>().InOrder(1);
            binding.Intercept().With<LogInterceptor>().InOrder(2);
        }

        private static void setupAdapterInterceptors<TAdapter>(IBindingWhenInNamedWithOrOnSyntax<TAdapter> binding)
        {
            binding.Intercept().With<AdapterExceptionInterceptor>().InOrder(1);
            binding.Intercept().With<TransactionInterceptor>().InOrder(2);
            binding.Intercept().With<LogInterceptor>().InOrder(3);
        }

        private static EntitySet getEntitySet(IContext context)
        {
            IConfigurationManager manager = context.Kernel.Get<IConfigurationManager>();
            return new EntitySet(manager.ConnectionString);
        }
    }
}
