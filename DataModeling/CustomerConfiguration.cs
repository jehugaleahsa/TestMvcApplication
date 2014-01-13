using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using DataObjects;

namespace DataModeling
{
    internal class CustomerConfiguration : EntityTypeConfiguration<Customer>
    {
        public CustomerConfiguration()
        {
            HasKey(c => c.CustomerId);
            Property(c => c.CustomerId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            HasMany(c => c.Settings).WithRequired(s => s.Customer).WillCascadeOnDelete(true);
            Property(c => c.Name).HasMaxLength(250).IsRequired();
        }
    }
}
