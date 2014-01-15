using System.Data;
using System.Net;
using Adapters;
using Ninject.Extensions.Interception.Infrastructure.Language;
using Ninject.Syntax;
using Policies;
using ServiceInterfaces;

namespace TestMvcApplication
{
    internal static class InterceptorHelpers
    {
        public static void WithRepositoryInterceptors<TRepository>(this IBindingWhenInNamedWithOrOnSyntax<TRepository> binding)
        {
            binding.Intercept().With(new ExceptionWrapper<DataException>((e, m) => new ServiceException(m, e))).InOrder(1);
            binding.Intercept().With<LogInterceptor>().InOrder(2);
        }

        public static void WithAdapterInterceptors<TAdapter>(this IBindingWhenInNamedWithOrOnSyntax<TAdapter> binding)
        {
            binding
                .Intercept()
                .With(new ExceptionWrapper<ServiceException>((e, m) => new AdapterException(HttpStatusCode.InternalServerError, m, e)))
                .InOrder(1);
            binding.Intercept().With<TransactionInterceptor>().InOrder(2);
            binding.Intercept().With<LogInterceptor>().InOrder(3);
        }
    }

}