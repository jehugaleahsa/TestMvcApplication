using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Objects;
using System.Linq;
using DataObjects;

namespace DataModeling
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
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.ValidateOnSaveEnabled = false;
        }

        public ObjectContext ObjectContext
        {
            get { return ((IObjectContextAdapter)this).ObjectContext; }
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<AddressItem> AddressItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }

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
