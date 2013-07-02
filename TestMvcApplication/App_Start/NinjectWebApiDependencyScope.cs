using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Ninject;
using Ninject.Syntax;

namespace TestMvcApplication
{
    public class NinjectWebApiDependencyScope : IDependencyScope
    {
        private IResolutionRoot resolver;

        public NinjectWebApiDependencyScope(IResolutionRoot resolver)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException("resolver");
            }
            this.resolver = resolver;
        }

        ~NinjectWebApiDependencyScope()
        {
            Dispose(false);
        }

        public object GetService(Type serviceType)
        {
            if (resolver == null)
            {
                throw new ObjectDisposedException("this", "The scope has been disposed.");
            }
            return resolver.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (resolver == null)
            {
                throw new ObjectDisposedException("this", "The scope has been disposed.");
            }
            return resolver.GetAll(serviceType);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                IDisposable disposable = resolver as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
                resolver = null;
            }
        }
    }
}