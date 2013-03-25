using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.EntityClient;
using System.Data.Objects;
using ServiceInterfaces.Entities;

namespace DataModeling.DataModel
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
            get 
            { 
                EntityConnection connection = (EntityConnection)context.ObjectContext.Connection;
                return connection.StoreConnection;
            }
        }

        public IDbSet<Customer> Customers
        {
            get { return context.Set<Customer>(); }
        }

        public IEnumerable<TEntity> GetEntities<TEntity>(DbDataReader reader)
        {
            return context.ObjectContext.Translate<TEntity>(reader);
        }

        public int SaveChanges()
        {
            return context.SaveChanges();
        }

        private class EntityContext : DbContext
        {
            public EntityContext(string connectionString)
                : base(connectionString)
            {
                this.Configuration.LazyLoadingEnabled = false;
                this.Configuration.ProxyCreationEnabled = false;
                this.Configuration.ValidateOnSaveEnabled = false;
            }

            public ObjectContext ObjectContext
            {
                get
                {
                    IObjectContextAdapter adapter = this as IObjectContextAdapter;
                    return adapter.ObjectContext;
                }
            }
        }
    }
}
