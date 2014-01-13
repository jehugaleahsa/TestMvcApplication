using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QueryableInterceptors
{
    public abstract class InterceptingQueryProvider : IQueryProvider
    {
        private readonly IQueryProvider provider;

        protected InterceptingQueryProvider(IQueryProvider provider)
        {
            this.provider = provider;
        }

        public virtual IEnumerator<TElement> ExecuteQuery<TElement>(Expression expression)
        {
            IQueryable<TElement> query = provider.CreateQuery<TElement>(expression);
            IEnumerator<TElement> enumerator = query.GetEnumerator();
            return enumerator;
        }

        public virtual IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            IQueryable<TElement> queryable = provider.CreateQuery<TElement>(expression);
            return new InterceptingQuery<TElement>(queryable, this);
        }

        public virtual IQueryable CreateQuery(Expression expression)
        {
            IQueryable queryable = provider.CreateQuery(expression);
            Type elementType = queryable.ElementType;
            Type queryType = typeof(InterceptingQuery<>).MakeGenericType(elementType);
            return (IQueryable)Activator.CreateInstance(queryType, queryable, this);
        }

        public virtual TResult Execute<TResult>(Expression expression)
        {
            return provider.Execute<TResult>(expression);
        }

        public virtual object Execute(Expression expression)
        {
            return provider.Execute(expression);
        }
    }
}
