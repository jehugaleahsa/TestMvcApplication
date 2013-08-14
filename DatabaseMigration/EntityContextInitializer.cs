using System;
using System.Data.Entity;
using DataModeling.DataModel;
using ServiceInterfaces.Entities;

namespace DatabaseMigration
{
    internal class EntityContextInitializer : DropCreateDatabaseAlways<EntityContext>
    {
        public EntityContextInitializer()
        {
        }

        protected override void Seed(EntityContext context)
        {
            context.Database.ExecuteSqlCommand("CREATE UNIQUE INDEX [UK_dbo.AddressItem_CustomerId_Key] ON [AddressItem] ([CustomerId], [Key])");

            seedCustomers(context);

            context.SaveChanges();
            base.Seed(context);
        }

        private static void seedCustomers(EntityContext context)
        {
            Customer aaron = context.Customers.Add(new Customer() { Name = "Aaron", Height = 1, BirthDate = new DateTime(2013, 1, 1) });
            context.AddressItems.Add(new AddressItem() { Customer = aaron, Key = "Line1", Value = "221 Aaron St" });
            context.AddressItems.Add(new AddressItem() { Customer = aaron, Key = "Municipality", Value = "Aaronville" });
            context.AddressItems.Add(new AddressItem() { Customer = aaron, Key = "Region", Value = "AZ" });
            context.AddressItems.Add(new AddressItem() { Customer = aaron, Key = "PostalCode", Value = "12345" });

            Customer bob = context.Customers.Add(new Customer() { Name = "Bob", Height = 2, BirthDate = new DateTime(2013, 1, 2) });
            context.AddressItems.Add(new AddressItem() { Customer = bob, Key = "Line1", Value = "221 Bob Ln" });
            context.AddressItems.Add(new AddressItem() { Customer = bob, Key = "Line2", Value = "Apt 123" });
            context.AddressItems.Add(new AddressItem() { Customer = bob, Key = "Municipality", Value = "Bobtown" });
            context.AddressItems.Add(new AddressItem() { Customer = bob, Key = "Region", Value = "MD" });
            context.AddressItems.Add(new AddressItem() { Customer = bob, Key = "PostalCode", Value = "23456" });

            Customer clint = context.Customers.Add(new Customer() { Name = "Clint", Height = 3, BirthDate = new DateTime(2013, 1, 3) });
            context.AddressItems.Add(new AddressItem() { Customer = clint, Key = "Line1", Value = "221 Clint Ave" });
            context.AddressItems.Add(new AddressItem() { Customer = clint, Key = "Municipality", Value = "Clintsville" });
            context.AddressItems.Add(new AddressItem() { Customer = clint, Key = "Region", Value = "CA" });
            context.AddressItems.Add(new AddressItem() { Customer = clint, Key = "PostalCode", Value = "34567" });

            Customer doug = context.Customers.Add(new Customer() { Name = "Doug", Height = 4, BirthDate = new DateTime(2013, 1, 4) });
            context.AddressItems.Add(new AddressItem() { Customer = doug, Key = "Line1", Value = "221 Doug St" });
            context.AddressItems.Add(new AddressItem() { Customer = doug, Key = "Line2", Value = "Apt 234" });
            context.AddressItems.Add(new AddressItem() { Customer = doug, Key = "Municipality", Value = "Dougington" });
            context.AddressItems.Add(new AddressItem() { Customer = doug, Key = "Region", Value = "DE" });
            context.AddressItems.Add(new AddressItem() { Customer = doug, Key = "PostalCode", Value = "45678" });

            Customer eric = context.Customers.Add(new Customer() { Name = "Eric", Height = 5, BirthDate = new DateTime(2013, 1, 5) });
            context.AddressItems.Add(new AddressItem() { Customer = eric, Key = "Line1", Value = "221 Eric St" });
            context.AddressItems.Add(new AddressItem() { Customer = eric, Key = "Municipality", Value = "Erictown" });
            context.AddressItems.Add(new AddressItem() { Customer = eric, Key = "Region", Value = "PA" });
            context.AddressItems.Add(new AddressItem() { Customer = eric, Key = "PostalCode", Value = "56789" });

            Customer fred = context.Customers.Add(new Customer() { Name = "Fred", Height = 6, BirthDate = new DateTime(2013, 1, 6) });
            context.AddressItems.Add(new AddressItem() { Customer = fred, Key = "Line1", Value = "221 Fred St" });
            context.AddressItems.Add(new AddressItem() { Customer = fred, Key = "Municipality", Value = "Fredville" });
            context.AddressItems.Add(new AddressItem() { Customer = fred, Key = "Region", Value = "FL" });
            context.AddressItems.Add(new AddressItem() { Customer = fred, Key = "PostalCode", Value = "54321" });

            Customer gary = context.Customers.Add(new Customer() { Name = "Gary", Height = 7, BirthDate = new DateTime(2013, 1, 7) });
            context.AddressItems.Add(new AddressItem() { Customer = gary, Key = "Line1", Value = "221 Gary St" });
            context.AddressItems.Add(new AddressItem() { Customer = gary, Key = "Line2", Value = "Apt 345" });
            context.AddressItems.Add(new AddressItem() { Customer = gary, Key = "Municipality", Value = "Garington" });
            context.AddressItems.Add(new AddressItem() { Customer = gary, Key = "Region", Value = "GA" });
            context.AddressItems.Add(new AddressItem() { Customer = gary, Key = "PostalCode", Value = "65432" });

            Customer henry = context.Customers.Add(new Customer() { Name = "Henry", Height = 8, BirthDate = new DateTime(2013, 1, 8) });
            context.AddressItems.Add(new AddressItem() { Customer = henry, Key = "Line1", Value = "221 Henry St" });
            context.AddressItems.Add(new AddressItem() { Customer = henry, Key = "Municipality", Value = "Henryburg" });
            context.AddressItems.Add(new AddressItem() { Customer = henry, Key = "Region", Value = "HI" });
            context.AddressItems.Add(new AddressItem() { Customer = henry, Key = "PostalCode", Value = "76543" });
        }
    }
}
