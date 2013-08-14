using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
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
            get { return ((IObjectContextAdapter)this).ObjectContext; }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            
            // Customer
            modelBuilder.Entity<Customer>().Property(c => c.CustomerId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Customer>().HasMany(c => c.Settings).WithRequired(s => s.Customer).WillCascadeOnDelete(true);
            modelBuilder.Entity<Customer>().Property(c => c.Name).HasMaxLength(250).IsRequired();

            // CustomerSetting
            modelBuilder.Entity<CustomerSetting>().Property(c => c.CustomerSettingId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<CustomerSetting>().Property(c => c.Key).HasMaxLength(250).IsRequired();
            modelBuilder.Entity<CustomerSetting>().Property(c => c.Value).HasMaxLength(1000).IsRequired();

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<CustomerSetting> CustomerSettings { get; set; }
    }
}
