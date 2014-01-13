using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace QueryableInterceptors
{
    internal class InterceptingQuery<TElement> : IQueryable<TElement>
    {
        private readonly IQueryable queryable;
        private readonly InterceptingQueryProvider provider;

        public InterceptingQuery(IQueryable queryable, InterceptingQueryProvider provider)
        {
            this.queryable = queryable;
            this.provider = provider;
        }

        public IQueryable<TElement> Include(string path)
        {
            return new InterceptingQuery<TElement>(queryable.Include(path), provider);
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            Expression expression = queryable.Expression;
            return provider.ExecuteQuery<TElement>(expression);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Type ElementType
        {
            get { return typeof(TElement); }
        }

        public Expression Expression
        {
            get { return queryable.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return provider; }
        }
    }
}
