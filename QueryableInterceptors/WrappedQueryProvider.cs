using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QueryableInterceptors
{
    internal class WrappedQueryProvider<TException> : InterceptingQueryProvider
        where TException : Exception
    {
        private readonly Func<TException, Exception> wrapper;

        internal WrappedQueryProvider(IQueryProvider provider, Func<TException, Exception> wrapper)
            : base(provider)
        {
            this.wrapper = wrapper;
        }

        public override IEnumerator<TElement> ExecuteQuery<TElement>(Expression expression)
        {
            return Check(() => wrapEnumerator<TElement>(expression), wrapper);
        }

        private IEnumerator<TElement> wrapEnumerator<TElement>(Expression expression)
        {
            IEnumerator<TElement> enumerator = base.ExecuteQuery<TElement>(expression);
            return new WrappedEnumerator<TElement>(enumerator, wrapper);
        }

        public override TResult Execute<TResult>(Expression expression)
        {
            return Check(() => base.Execute<TResult>(expression), wrapper);
        }

        public override object Execute(Expression expression)
        {
            return Check(() => base.Execute(expression), wrapper);
        }

        internal static TResult Check<TResult>(Func<TResult> action, Func<TException, Exception> wrapper)
        {
            try
            {
                return action();
            }
            catch (TException exception)
            {
                throw wrapper(exception);
            }
        }

        private class WrappedEnumerator<TElement> : IEnumerator<TElement>
        {
            private readonly IEnumerator<TElement> enumerator;
            private readonly Func<TException, Exception> wrapper;

            public WrappedEnumerator(IEnumerator<TElement> enumerator, Func<TException, Exception> wrapper)
            {
                this.enumerator = enumerator;
                this.wrapper = wrapper;
            }

            public TElement Current
            {
                get { return enumerator.Current; }
            }

            public void Dispose()
            {
                enumerator.Dispose();
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                return WrappedQueryProvider<TException>.Check(enumerator.MoveNext, wrapper);
            }

            public void Reset()
            {
                enumerator.Reset();
            }
        }
    }
}
