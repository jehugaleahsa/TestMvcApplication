using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;
using ServiceInterfaces;

namespace DataModeling
{
    public class EntityLoader<TEntity> : IEntityLoader<TEntity>
        where TEntity : class
    {
        private readonly DbContext context;
        private readonly TEntity entity;

        internal EntityLoader(DbContext context, TEntity entity)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
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
            this.entity = entity;
        }

        public void Reload()
        {
            DbEntityEntry<TEntity> entry = context.Entry(entity);
            entry.Reload();
        }

        public void Load<TRelation>(Expression<Func<TEntity, TRelation>> accessor)
            where TRelation : class
        {
            DbEntityEntry<TEntity> entry = context.Entry(entity);
            DbReferenceEntry reference = entry.Reference(accessor);
            if (!reference.IsLoaded)
            {
                reference.Load();
            }
        }

        public void Load<TRelation>(Expression<Func<TEntity, ICollection<TRelation>>> accessor)
            where TRelation : class
        {
            DbEntityEntry<TEntity> entry = context.Entry(entity);
            DbCollectionEntry collection = entry.Collection(accessor);
            if (!collection.IsLoaded)
            {
                collection.Load();
            }
        }

        public IQueryable<TRelation> LoadQuery<TRelation>(Expression<Func<TEntity, TRelation>> accessor)
            where TRelation : class
        {
            DbEntityEntry<TEntity> entry = context.Entry(entity);
            DbReferenceEntry<TEntity, TRelation> reference = entry.Reference(accessor);
            return reference.Query();
        }

        public IQueryable<TRelation> LoadQuery<TRelation>(Expression<Func<TEntity, ICollection<TRelation>>> accessor)
            where TRelation : class
        {
            DbEntityEntry<TEntity> entry = context.Entry(entity);
            DbCollectionEntry<TEntity, TRelation> collection = entry.Collection(accessor);
            return collection.Query();
        }
    }
}
