using Adapters;
using DataModeling;
using MvcUtilities.FilterAttributes;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using Ninject.Web.Common;
using ServiceInterfaces;
using TestMvcApplication.Context;
using TestMvcApplication.Controllers;

namespace TestMvcApplication
{
    public class TestNinjectModule : NinjectModule
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

            Bind<ICustomerRepository>().To<CustomerRepository>().WithRepositoryInterceptors();
            Bind<IAddressItemRepository>().To<AddressItemRepository>().WithRepositoryInterceptors();

            Bind<ICustomerAdapter>().To<CustomerAdapter>().WithAdapterInterceptors();
            Bind<IAddressItemAdapter>().To<AddressItemAdapter>().WithAdapterInterceptors();

            Bind<ErrorController>().ToSelf();
            Bind<ClassicController>().ToSelf();
            Bind<KnockoutController>().ToSelf();
            Bind<CustomersController>().ToSelf();
        }

        private static EntitySet getEntitySet(IContext context)
        {
            IConfigurationManager manager = context.Kernel.Get<IConfigurationManager>();
            return new EntitySet(manager.ConnectionString);
        }
    }
}