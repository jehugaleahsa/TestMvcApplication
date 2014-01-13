using System;
using System.Linq;

namespace QueryableInterceptors
{
    public static class QueryableExtensions
    {
        public static IQueryable<TElement> Handle<TElement, TException>(this IQueryable<TElement> source, Func<TException, Exception> wrapper)
            where TException : Exception
        {
            return WrappedQueryProvider<TException>.Check(() => handle(source, wrapper), wrapper);
        }

        private static IQueryable<TElement> handle<TElement, TException>(IQueryable<TElement> source, Func<TException, Exception> wrapper)
            where TException : Exception
        {
            var provider = new WrappedQueryProvider<TException>(source.Provider, wrapper);
            return provider.CreateQuery<TElement>(source.Expression);
        }
    }
}
