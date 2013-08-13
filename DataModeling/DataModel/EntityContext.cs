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

        public DbSet<CustomerSetting> CustomerSettings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new CustomerConfiguration());
            modelBuilder.Configurations.Add(new CustomerSettingConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
