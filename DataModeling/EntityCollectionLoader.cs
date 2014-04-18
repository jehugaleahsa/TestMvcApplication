using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ServiceInterfaces;

namespace DataModeling
{
    public class EntityCollectionLoader<TEntity> : IEntityLoader<TEntity>
        where TEntity : class
    {
        private readonly DbContext context;
        private readonly IEnumerable<TEntity> entities;

        internal EntityCollectionLoader(DbContext context, IEnumerable<TEntity> entities)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            if (entities.Any(e => e == null))
            {
                throw new ArgumentException("Encountered a null entity.", "entities");
            }
            this.context = context;
            this.entities = entities;
        }

        public void Reload()
        {
            Expression<Func<TEntity, bool>> expression = getFilterExpression();
            if (expression == null)
            {
                return;
            }
            context.Set<TEntity>()
                .Where(expression)
                .Load();
        }

        public void Load<TRelation>(Expression<Func<TEntity, TRelation>> accessor)
            where TRelation : class
        {
            Expression<Func<TEntity, bool>> expression = getFilterExpression();
            if (expression == null)
            {
                return;
            }
            context.Set<TEntity>()
                .Where(expression)
                .Include(accessor)
                .Load();
        }

        public void Load<TRelation>(Expression<Func<TEntity, ICollection<TRelation>>> accessor)
            where TRelation : class
        {
            Expression<Func<TEntity, bool>> expression = getFilterExpression();
            if (expression == null)
            {
                return;
            }
            context.Set<TEntity>()
                .Where(expression)
                .Include(accessor)
                .Load();
        }

        public IQueryable<TRelation> LoadQuery<TRelation>(Expression<Func<TEntity, TRelation>> accessor)
            where TRelation : class
        {
            IQueryable<TEntity> query = context.Set<TEntity>();
            Expression<Func<TEntity, bool>> expression = getFilterExpression();
            if (expression != null)
            {
                query = query.Where(expression);
            }
            return query.Select(accessor);
        }

        public IQueryable<TRelation> LoadQuery<TRelation>(Expression<Func<TEntity, ICollection<TRelation>>> accessor)
            where TRelation : class
        {
            IQueryable<TEntity> query = context.Set<TEntity>();
            Expression<Func<TEntity, bool>> expression = getFilterExpression();
            if (expression != null)
            {
                query = query.Where(expression);
            }
            Expression<Func<TEntity, IEnumerable<TRelation>>> wrapped = Expression.Lambda<Func<TEntity, IEnumerable<TRelation>>>(accessor.Body, accessor.Parameters.ToArray());
            return query.SelectMany(wrapped);
        }

        private Expression<Func<TEntity, bool>> getFilterExpression()
        {
            if (!entities.Any())
            {
                return null;
            }

            var keys = getKeyNames();

            if (!keys.Any())
            {
                // the entity has no keys, so throw an exception
                throw new InvalidOperationException("Loading is not permitted for entities without keys.");
            }
            else if (!keys.Skip(1).Any())
            {
                // the is a single key, so use a Contains filter
                var e = Expression.Parameter(typeof(TEntity), "e");
                Expression inFilter = getContainsFilter(e);
                Expression<Func<TEntity, bool>> expression = Expression.Lambda<Func<TEntity, bool>>(inFilter, e);
                return expression;
            }
            else
            {
                // there is a multi-part key, try to build a filter with conjunctions and disjunctions
                var e = Expression.Parameter(typeof(TEntity), "e");
                Expression orFilter = getOrFilter(e);
                Expression<Func<TEntity, bool>> expression = Expression.Lambda<Func<TEntity, bool>>(orFilter, e);
                return expression;
            }
        }

        private IEnumerable<string> getKeyNames()
        {
            IObjectContextAdapter adapter = context;
            var objectContext = adapter.ObjectContext;
            var stateManager = objectContext.ObjectStateManager;

            var keys = stateManager.GetObjectStateEntry(entities.First()).EntityKey.EntityKeyValues.Select(k => k.Key);
            return keys;
        }

        private Expression getContainsFilter(ParameterExpression parameter)
        {
            IObjectContextAdapter adapter = context;
            var objectContext = adapter.ObjectContext;
            var stateManager = objectContext.ObjectStateManager;

            var entries = entities.Select(e => stateManager.GetObjectStateEntry(e)).ToArray();
            string keyName = entries.Select(e => e.EntityKey.EntityKeyValues.Select(k => k.Key).Single()).First();
            PropertyInfo keyProperty = typeof(TEntity).GetProperty(keyName);
            var containsMethod = getContainsMethod(keyProperty);
            Expression array = getKeyArrayExpressions(entries, keyProperty);
            Expression member = Expression.MakeMemberAccess(parameter, keyProperty);
            Expression contains = Expression.Call(containsMethod, array, member);
            return contains;
        }

        private static MethodInfo getContainsMethod(PropertyInfo keyProperty)
        {
            var containsMethods = from method in typeof(Enumerable).GetMethods()
                                  where method.Name == "Contains"
                                  where method.IsGenericMethodDefinition
                                  let typeParameters = method.GetGenericArguments()
                                  where typeParameters.Length == 1
                                  let typeParameter = typeParameters.Single()
                                  let parameters = method.GetParameters()
                                  where parameters.Length == 2
                                  where parameters.ElementAt(0).ParameterType == typeof(IEnumerable<>).MakeGenericType(typeParameter)
                                  where parameters.ElementAt(1).ParameterType == typeParameter
                                  select method;
            var containsMethod = containsMethods.Single().MakeGenericMethod(keyProperty.PropertyType);
            return containsMethod;
        }

        private Expression getKeyArrayExpressions(IEnumerable<ObjectStateEntry> entries, PropertyInfo keyProperty)
        {
            var values = from entry in entries
                         let keyPairs = entry.EntityKey.EntityKeyValues
                         let value = keyPairs.Select(p => p.Value).Single()
                         select Expression.Constant(value);
            Expression array = Expression.NewArrayInit(keyProperty.PropertyType, values.ToArray());
            return array;
        }

        private Expression getOrFilter(ParameterExpression parameter)
        {
            IObjectContextAdapter adapter = context;
            var objectContext = adapter.ObjectContext;
            var stateManager = objectContext.ObjectStateManager;

            Expression or = null;
            foreach (TEntity entity in entities)
            {
                var entry = stateManager.GetObjectStateEntry(entity);
                Expression and = getKeyComparisonExpression(entry, parameter);
                or = or == null ? and : Expression.OrElse(or, and);
            }
            return or;
        }

        private static Expression getKeyComparisonExpression(ObjectStateEntry entry, Expression parameter)
        {
            var key = entry.EntityKey;

            Expression and = null;
            foreach (var keyPart in key.EntityKeyValues)
            {
                Expression equal = getEqualExpression(parameter, keyPart);
                and = and == null ? equal : Expression.AndAlso(and, equal);
            }
            return and;
        }

        private static Expression getEqualExpression(Expression parameter, EntityKeyMember keyPart)
        {
            PropertyInfo property = typeof(TEntity).GetProperty(keyPart.Key);
            Expression member = Expression.MakeMemberAccess(parameter, property);
            Expression value = Expression.Constant(keyPart.Value);
            Expression equal = Expression.Equal(member, value);
            return equal;
        }
    }
}
