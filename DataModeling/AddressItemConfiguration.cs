using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using DataObjects;

namespace DataModeling
{
    internal class AddressItemConfiguration : EntityTypeConfiguration<AddressItem>
    {
        public AddressItemConfiguration()
        {
            HasKey(i => i.AddressItemId);
            Property(i => i.AddressItemId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(i => i.Key).HasMaxLength(250).IsRequired();
            Property(i => i.Value).HasMaxLength(1000).IsRequired();
        }
    }
}
