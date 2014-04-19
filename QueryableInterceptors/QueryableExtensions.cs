using System;
using System.Linq;

namespace QueryableInterceptors
{
    public static class QueryableExtensions
    {
        public static ExceptionHandler<TElement> WrapExceptions<TElement>(this IQueryable<TElement> source)
        {
            return new ExceptionHandler<TElement>(source);
        }
    }

    public sealed class ExceptionHandler<TElement>
    {
        private readonly IQueryable<TElement> queryable;

        internal ExceptionHandler(IQueryable<TElement> queryable)
        {
            this.queryable = queryable;
        }

        public ExceptionHandler<TElement, TException> OfType<TException>()
            where TException : Exception
        {
            return new ExceptionHandler<TElement, TException>(queryable);
        }
    }

    public sealed class ExceptionHandler<TElement, TFromException>
        where TFromException : Exception
    {
        private readonly IQueryable<TElement> queryable;

        internal ExceptionHandler(IQueryable<TElement> queryable)
        {
            this.queryable = queryable;
        }

        public IQueryable<TElement> With<TToException>()
            where TToException : Exception
        {
            Func<TFromException, Exception> wrapper = e => (TToException)Activator.CreateInstance(typeof(TToException), e.Message, e);
            return WrappedQueryProvider<TFromException>.Check(() => handle(queryable, wrapper), wrapper);
        }

        public IQueryable<TElement> With<TToException>(string message)
            where TToException : Exception
        {
            Func<TFromException, Exception> wrapper = e => (TToException)Activator.CreateInstance(typeof(TToException), message, e);
            return WrappedQueryProvider<TFromException>.Check(() => handle(queryable, wrapper), wrapper);
        }

        public IQueryable<TElement> With<TToException>(Func<TFromException, TToException> wrapper)
            where TToException : Exception
        {
            return WrappedQueryProvider<TFromException>.Check(() => handle(queryable, wrapper), wrapper);
        }

        private static IQueryable<TElement> handle(IQueryable<TElement> source, Func<TFromException, Exception> wrapper)
        {
            var provider = new WrappedQueryProvider<TFromException>(source.Provider, wrapper);
            return provider.CreateQuery<TElement>(source.Expression);
        }
    }
}
