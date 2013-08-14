using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.EntityClient;
using DataModeling.DataModel;
using ServiceInterfaces.Entities;

namespace DataModeling
{
    public class EntitySet : IDisposable
    {
        private readonly EntityContext context;

        public EntitySet(string connectionString)
        {
            context = new EntityContext(connectionString);
            context.Configuration.LazyLoadingEnabled = false;
            context.Configuration.ProxyCreationEnabled = false;
            context.Configuration.ValidateOnSaveEnabled = false;
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
            get 
            { 
                EntityConnection connection = (EntityConnection)context.ObjectContext.Connection;
                return connection.StoreConnection;
            }
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
    }
}
