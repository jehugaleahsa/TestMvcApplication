using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using ServiceInterfaces.Entities;

namespace DataModeling.DataModel
{
    internal class EntityContext : DbContext
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

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new CustomerConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
