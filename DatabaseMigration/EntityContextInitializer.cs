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
            context.Database.ExecuteSqlCommand("CREATE UNIQUE INDEX [UK_dbo.CustomerSetting_CustomerId_Key] ON [CustomerSetting] ([CustomerId], [Key])");

            seedCustomers(context);

            context.SaveChanges();
            base.Seed(context);
        }

        private static void seedCustomers(EntityContext context)
        {
            Customer aaron = context.Customers.Add(new Customer() { Name = "Aaron", Height = 1, BirthDate = new DateTime(2013, 1, 1) });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = aaron, Key = "Line1", Value = "221 Aaron St" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = aaron, Key = "Municipality", Value = "Aaronville" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = aaron, Key = "Region", Value = "AZ" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = aaron, Key = "PostalCode", Value = "12345" });

            Customer bob = context.Customers.Add(new Customer() { Name = "Bob", Height = 2, BirthDate = new DateTime(2013, 1, 2) });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = bob, Key = "Line1", Value = "221 Bob Ln" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = bob, Key = "Line2", Value = "Apt 123" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = bob, Key = "Municipality", Value = "Bobtown" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = bob, Key = "Region", Value = "MD" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = bob, Key = "PostalCode", Value = "23456" });

            Customer clint = context.Customers.Add(new Customer() { Name = "Clint", Height = 3, BirthDate = new DateTime(2013, 1, 3) });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = clint, Key = "Line1", Value = "221 Clint Ave" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = clint, Key = "Municipality", Value = "Clintsville" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = clint, Key = "Region", Value = "CA" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = clint, Key = "PostalCode", Value = "34567" });

            Customer doug = context.Customers.Add(new Customer() { Name = "Doug", Height = 4, BirthDate = new DateTime(2013, 1, 4) });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = doug, Key = "Line1", Value = "221 Doug St" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = doug, Key = "Line2", Value = "Apt 234" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = doug, Key = "Municipality", Value = "Dougington" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = doug, Key = "Region", Value = "DE" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = doug, Key = "PostalCode", Value = "45678" });

            Customer eric = context.Customers.Add(new Customer() { Name = "Eric", Height = 5, BirthDate = new DateTime(2013, 1, 5) });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = eric, Key = "Line1", Value = "221 Eric St" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = eric, Key = "Municipality", Value = "Erictown" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = eric, Key = "Region", Value = "PA" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = eric, Key = "PostalCode", Value = "56789" });

            Customer fred = context.Customers.Add(new Customer() { Name = "Fred", Height = 6, BirthDate = new DateTime(2013, 1, 6) });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = fred, Key = "Line1", Value = "221 Fred St" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = fred, Key = "Municipality", Value = "Fredville" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = fred, Key = "Region", Value = "FL" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = fred, Key = "PostalCode", Value = "54321" });

            Customer gary = context.Customers.Add(new Customer() { Name = "Gary", Height = 7, BirthDate = new DateTime(2013, 1, 7) });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = gary, Key = "Line1", Value = "221 Gary St" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = gary, Key = "Line2", Value = "Apt 345" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = gary, Key = "Municipality", Value = "Garington" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = gary, Key = "Region", Value = "GA" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = gary, Key = "PostalCode", Value = "65432" });

            Customer henry = context.Customers.Add(new Customer() { Name = "Henry", Height = 8, BirthDate = new DateTime(2013, 1, 8) });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = henry, Key = "Line1", Value = "221 Henry St" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = henry, Key = "Municipality", Value = "Henryburg" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = henry, Key = "Region", Value = "HI" });
            context.CustomerSettings.Add(new CustomerSetting() { Customer = henry, Key = "PostalCode", Value = "76543" });
        }
    }
}
