using System.Web.Http.Dependencies;
using Ninject;

namespace TestMvcApplication
{
    public sealed class NinjectWebApiDependencyResolver : NinjectWebApiDependencyScope, IDependencyResolver
    {
        private readonly IKernel kernel;

        public NinjectWebApiDependencyResolver(IKernel kernel)
            : base(kernel)
        {
            this.kernel = kernel;
        }

        public IDependencyScope BeginScope()
        {
            return new NinjectWebApiDependencyScope(kernel.BeginBlock());
        }
    }
}