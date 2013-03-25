using System;
using System.Collections.Generic;
using System.Linq;
using DataModeling.DataModel;
using Policies;
using ServiceInterfaces.Entities;
using ServiceInterfaces.Repositories;

namespace DataModeling.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly EntitySet entities;

        public CustomerRepository(EntitySet entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            this.entities = entities;
        }

        [ErrorMessage("Failed to get the customer.")]
        public Customer GetCustomer(Guid customerId)
        {
            return entities.Customers.SingleOrDefault(customer => customer.CustomerId == customerId);
        }

        [ErrorMessage("Failed to get the customers.")]
        public IEnumerable<Customer> GetCustomers()
        {
            return entities.Customers.ToList();
        }

        public void Add(Customer customer)
        {
            entities.Customers.Add(customer);
            entities.SaveChanges();
        }

        public void Update(Customer original, Customer modified)
        {
            original.Name = modified.Name;
            original.BirthDate = modified.BirthDate;
            original.Height = modified.Height;
            entities.SaveChanges();
        }

        public void Remove(Customer customer)
        {
            entities.Customers.Remove(customer);
            entities.SaveChanges();
        }
    }
}
