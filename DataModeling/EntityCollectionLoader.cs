using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
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
        private readonly EntityType entityType;

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

            IObjectContextAdapter adapter = context;
            var objectContext = adapter.ObjectContext;
            var workspace = objectContext.MetadataWorkspace;
            EntityType entityType;
            if (!workspace.TryGetItem<EntityType>(typeof(TEntity).FullName, DataSpace.OSpace, out entityType))
            {
                const string format = "The entity type {0} is not part of the model for the current context."
                    + " Verify the entity is configured with the context and not a complex type.";
                string message = String.Format(null, format, typeof(TEntity).Name);
                throw new InvalidOperationException(message);
            }

            this.context = context;
            this.entities = entities;
            this.entityType = entityType;
        }

        public void Reload()
        {
            Expression<Func<TEntity, bool>> expression = getFilterExpression(null);
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
            Func<TEntity, bool> isLoaded = e =>
            {
                DbEntityEntry<TEntity> entry = context.Entry(e);
                DbReferenceEntry<TEntity, TRelation> reference = entry.Reference(accessor);
                return reference.IsLoaded;
            };
            Expression<Func<TEntity, bool>> expression = getFilterExpression(isLoaded);
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
            Func<TEntity, bool> isLoaded = e =>
            {
                DbEntityEntry<TEntity> entry = context.Entry(e);
                DbCollectionEntry<TEntity, TRelation> collection = entry.Collection(accessor);
                return collection.IsLoaded;
            };
            Expression<Func<TEntity, bool>> expression = getFilterExpression(isLoaded);
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
            Expression<Func<TEntity, bool>> expression = getFilterExpression(null);
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
            Expression<Func<TEntity, bool>> expression = getFilterExpression(null);
            if (expression != null)
            {
                query = query.Where(expression);
            }
            Expression<Func<TEntity, IEnumerable<TRelation>>> wrapped = Expression.Lambda<Func<TEntity, IEnumerable<TRelation>>>(accessor.Body, accessor.Parameters.ToArray());
            return query.SelectMany(wrapped);
        }

        private Expression<Func<TEntity, bool>> getFilterExpression(Func<TEntity, bool> isLoaded)
        {
            if (!entities.Any())
            {
                return null;
            }
            if (isLoaded == null)
            {
                isLoaded = e => false;
            }

            var keys = getKeyNames();

            if (!keys.Any())
            {
                // the entity has no keys, it's probably a complex type, which is illegal
                // attempt to build filter using every property
                var e = Expression.Parameter(typeof(TEntity), "e");
                var properties = getAllProperties();
                Expression orFilter = getCompoundOrFilter(e, properties, isLoaded);
                Expression<Func<TEntity, bool>> expression = Expression.Lambda<Func<TEntity, bool>>(orFilter, e);
                return expression;
                
            }
            else if (!keys.Skip(1).Any())
            {
                // the is a single key, so use a Contains filter
                var e = Expression.Parameter(typeof(TEntity), "e");
                var keyProperty = getKeyProperties().Single();
                Expression inFilter = getContainsFilter(e, keyProperty, isLoaded);
                Expression<Func<TEntity, bool>> expression = Expression.Lambda<Func<TEntity, bool>>(inFilter, e);
                return expression;
            }
            else
            {
                // there is a multi-part key, try to build a filter with conjunctions and disjunctions
                var e = Expression.Parameter(typeof(TEntity), "e");
                var keyProperties = getKeyProperties();
                Expression orFilter = getCompoundOrFilter(e, keyProperties, isLoaded);
                Expression<Func<TEntity, bool>> expression = Expression.Lambda<Func<TEntity, bool>>(orFilter, e);
                return expression;
            }
        }

        private IEnumerable<string> getKeyNames()
        {
            return entityType.KeyMembers.Select(k => k.Name);
        }

        private IEnumerable<PropertyInfo> getKeyProperties()
        {
            string[] propertyNames = entityType.KeyMembers.Select(m => m.Name).ToArray();
            return propertyNames.Select(n => typeof(TEntity).GetProperty(n)).ToArray();
        }

        private IEnumerable<PropertyInfo> getAllProperties()
        {
            string[] propertyNames = entityType.Properties.Select(m => m.Name).ToArray();
            return propertyNames.Select(n => typeof(TEntity).GetProperty(n)).ToArray();
        }

        private Expression getContainsFilter(ParameterExpression parameter, PropertyInfo keyProperty, Func<TEntity, bool> isLoaded)
        {
            var containsMethod = getContainsMethod(keyProperty);
            Expression array = getArrayOfKeysExpression(keyProperty, isLoaded);
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

        private Expression getArrayOfKeysExpression(PropertyInfo keyProperty, Func<TEntity, bool> isLoaded)
        {
            var values = from entity in entities
                         where !isLoaded(entity)
                         let value = keyProperty.GetValue(entity, null)
                         select Expression.Constant(value);
            values = values.ToArray();
            Expression array = Expression.NewArrayInit(keyProperty.PropertyType, values);
            return array;
        }

        private Expression getCompoundOrFilter(ParameterExpression parameter, IEnumerable<PropertyInfo> properties, Func<TEntity, bool> isLoaded)
        {
            Expression or = null;
            var missing = entities.Where(e => !isLoaded(e)).ToArray();
            if (!missing.Any())
            {
                return Expression.Constant(false);
            }
            foreach (TEntity entity in missing)
            {
                Expression and = getCompoundAndExpression(entity, properties, parameter);
                or = or == null ? and : Expression.OrElse(or, and);
            }
            return or;
        }

        private static Expression getCompoundAndExpression(TEntity entity, IEnumerable<PropertyInfo> properties, Expression parameter)
        {
            Expression and = null;
            foreach (var property in properties)
            {
                object value = property.GetValue(entity, null);
                Expression equal = getEqualExpression(parameter, property, value);
                and = and == null ? equal : Expression.AndAlso(and, equal);
            }
            return and;
        }

        private static Expression getEqualExpression(Expression parameter, PropertyInfo property, object value)
        {
            Expression member = Expression.MakeMemberAccess(parameter, property);
            Expression constant = Expression.Constant(value);
            Expression equal = Expression.Equal(member, constant);
            return equal;
        }
    }
}
