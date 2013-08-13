using System.Data.Entity;
using DataModeling.DataModel;
using ServiceInterfaces.Entities;
using System;

namespace DatabaseMigration
{
    internal class EntityContextInitializer : DropCreateDatabaseAlways<EntityContext>
    {
        public EntityContextInitializer()
        {
        }

        protected override void Seed(EntityContext context)
        {
            seedCustomers(context);
            context.SaveChanges();
            base.Seed(context);
        }

        private static void seedCustomers(EntityContext context)
        {
            context.Customers.Add(new Customer() { Name = "Aaron", Height = 1, BirthDate = new DateTime(2013, 1, 1) });
            context.Customers.Add(new Customer() { Name = "Bob", Height = 2, BirthDate = new DateTime(2013, 1, 2) });
            context.Customers.Add(new Customer() { Name = "Clint", Height = 3, BirthDate = new DateTime(2013, 1, 3) });
            context.Customers.Add(new Customer() { Name = "Doug", Height = 4, BirthDate = new DateTime(2013, 1, 4) });
            context.Customers.Add(new Customer() { Name = "Eric", Height = 5, BirthDate = new DateTime(2013, 1, 5) });
            context.Customers.Add(new Customer() { Name = "Fred", Height = 6, BirthDate = new DateTime(2013, 1, 6) });
            context.Customers.Add(new Customer() { Name = "Gary", Height = 7, BirthDate = new DateTime(2013, 1, 7) });
            context.Customers.Add(new Customer() { Name = "Henry", Height = 8, BirthDate = new DateTime(2013, 1, 8) });
        }
    }
}
