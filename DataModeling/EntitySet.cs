using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using DataObjects;
using ServiceInterfaces;

namespace DataModeling
{
    public class EntitySet : IDisposable
    {
        private readonly EntityContext context;

        public EntitySet(string connectionString)
        {
            context = new EntityContext(connectionString);
        }

        ~EntitySet()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }

        public DbConnection Connection
        {
            get { return context.ObjectContext.Connection; }
        }

        public IEnumerable<TEntity> GetEntities<TEntity>(DbDataReader reader)
        {
            return context.ObjectContext.Translate<TEntity>(reader);
        }

        public int SaveChanges()
        {
            return context.SaveChanges();
        }

        public IDbSet<Customer> Customers
        {
            get { return context.Customers; }
        }

        public IDbSet<AddressItem> AddressItems 
        { 
            get { return context.AddressItems; } 
        }

        public IEntityLoader<TEntity> GetLoader<TEntity>(TEntity entity)
            where TEntity : class
        {
            return new EntityLoader<TEntity>(context, entity);
        }

        public IEntityLoader<TEntity> GetLoaderForCollection<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            return new EntityCollectionLoader<TEntity>(context, entities);
        }
    }
}
