using System;
using System.Collections.Generic;
using Adapters.Models;
using Policies;

namespace Adapters.Adapters
{
    public interface ICustomerAdapter
    {
        CustomerData AddCustomer(CustomerData customerData);

        CustomerData GetCustomer(string customerId);

        IEnumerable<CustomerData> GetCustomers();

        void RemoveCustomer(string customerId);

        void UpdateCustomer(CustomerData customerData);
    }
}
