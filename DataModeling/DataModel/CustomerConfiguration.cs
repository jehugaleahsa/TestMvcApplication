﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using ServiceInterfaces.Entities;

namespace DataModeling.DataModel
{
    internal class CustomerConfiguration : EntityTypeConfiguration<Customer>
    {
        public CustomerConfiguration()
        {
            Property(c => c.CustomerId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            HasMany(c => c.Settings).WithRequired(s => s.Customer).WillCascadeOnDelete(true);
            Property(c => c.Name).HasMaxLength(250).IsRequired();
        }
    }
}
