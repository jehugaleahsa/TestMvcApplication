using System;
using System.Collections.Generic;
using ServiceInterfaces.Entities;

namespace ServiceInterfaces.Repositories
{
    public interface ICustomerRepository
    {
        void Add(Customer customer);

        IEnumerable<Customer> GetCustomers();

        Customer GetCustomer(Guid customerId);

        void Remove(Customer customer);
    }
}
