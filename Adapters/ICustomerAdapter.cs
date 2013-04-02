using System.Collections.Generic;
using Adapters.Models;

namespace Adapters
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
