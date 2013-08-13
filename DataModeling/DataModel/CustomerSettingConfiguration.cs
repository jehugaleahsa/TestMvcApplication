using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using ServiceInterfaces.Entities;

namespace DataModeling.DataModel
{
    internal class CustomerSettingConfiguration : EntityTypeConfiguration<CustomerSetting>
    {
        public CustomerSettingConfiguration()
        {
            Property(c => c.CustomerSettingId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(c => c.Key).HasMaxLength(250).IsRequired();
            Property(c => c.Value).HasMaxLength(1000).IsRequired();
        }
    }
}
