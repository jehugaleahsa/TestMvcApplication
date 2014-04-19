using System;
using System.Collections.Generic;
using System.Linq;
using DataObjects;
using Policies;
using ServiceInterfaces;
using QueryableInterceptors;

namespace DataModeling
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

        [Log]
        [ErrorMessage("Failed to get the customer.")]
        public Customer GetCustomer(Guid customerId)
        {
            return entities.Customers.SingleOrDefault(customer => customer.CustomerId == customerId);
        }

        [Log]
        [ErrorMessage("Failed to get the customers.")]
        public IEnumerable<Customer> GetCustomers()
        {
            return entities.Customers.ToArray();
        }

        [Log]
        [ErrorMessage("Failed to add the customer.")]
        public void Add(Customer customer)
        {
            entities.Customers.Add(customer);
            entities.SaveChanges();
        }

        [Log]
        [ErrorMessage("Failed to update the customer.")]
        public void Update(Customer original, Customer modified)
        {
            if (original == null)
            {
                throw new ArgumentNullException("original");
            }
            if (modified == null)
            {
                throw new ArgumentNullException("modified");
            }
            original.Name = modified.Name;
            original.BirthDate = modified.BirthDate;
            original.Height = modified.Height;
            entities.SaveChanges();
        }

        [Log]
        [ErrorMessage("Failed to remove the customer.")]
        public void Remove(Customer customer)
        {
            entities.Customers.Remove(customer);
            entities.SaveChanges();
        }
    }
}
