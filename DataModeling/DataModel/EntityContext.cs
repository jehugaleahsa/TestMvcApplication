using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Objects;
using System.Linq;
using ServiceInterfaces.Entities;

namespace DataModeling.DataModel
{
    internal class EntityContext : DbContext
    {
        static EntityContext()
        {
            Database.SetInitializer<EntityContext>(null);
        }

        public EntityContext(string connectionString)
            : base(connectionString)
        {
        }

        public ObjectContext ObjectContext
        {
            get { return ((IObjectContextAdapter)this).ObjectContext; }
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<AddressItem> AddressItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            addEntityConfigurations(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void addEntityConfigurations(DbModelBuilder modelBuilder)
        {
            var types = from type in GetType().Assembly.GetTypes()
                        where type.BaseType.IsGenericType
                        where type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>)
                        select type;
            foreach (Type type in types)
            {
                dynamic configuration = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configuration);
            }
        }
    }
}
