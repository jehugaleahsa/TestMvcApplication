using System;
using System.Collections.Generic;
using ServiceInterfaces.Entities;

namespace ServiceInterfaces.Repositories
{
    public interface ICustomerRepository
    {
        Customer GetCustomer(Guid customerId);

        IEnumerable<Customer> GetCustomers();

        void Add(Customer customer);

        void Update(Customer original, Customer modified);

        void Remove(Customer customer);
    }
}
