using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Adapters.Mappers;
using Adapters.Models;
using Policies;
using ServiceInterfaces.Entities;
using ServiceInterfaces.Repositories;

namespace Adapters.Adapters
{
    public class CustomerAdapter : ICustomerAdapter
    {
        private readonly ICustomerRepository customerRepository;

        public CustomerAdapter(ICustomerRepository customerRepository)
        {
            if (customerRepository == null)
            {
                throw new ArgumentNullException("customerRepository");
            }
            this.customerRepository = customerRepository;
        }

        [ErrorMessage("An error occurred while retrieving the customer.")]
        public CustomerData GetCustomer(string customerId)
        {
            CustomerMapper mapper = new CustomerMapper();
            Customer customer = getCustomer(customerId);
            return mapper.Convert(customer);
        }

        [ErrorMessage("An error occurred while retrieving the customers.")]
        public IEnumerable<CustomerData> GetCustomers()
        {
            CustomerMapper mapper = new CustomerMapper();
            return customerRepository.GetCustomers().Select(mapper.Convert).ToList();
        }

        [ErrorMessage("An error occurred while adding the customer.")]
        public CustomerData AddCustomer(CustomerData customerData)
        {
            CustomerMapper mapper = new CustomerMapper();
            Customer customer = mapper.Convert(customerData);
            customerRepository.Add(customer);
            return mapper.Convert(customer);
        }

        [ErrorMessage("An error occurred while updating the customer.")]
        public void UpdateCustomer(CustomerData customerData)
        {
            CustomerMapper mapper = new CustomerMapper();
            Customer original = getCustomer(customerData.CustomerId);
            Customer modified = mapper.Convert(customerData);
            customerRepository.Update(original, modified);
        }

        [ErrorMessage("An error occurred while removing the customer.")]
        public void RemoveCustomer(string customerId)
        {
            Customer customer = getCustomer(customerId);
            customerRepository.Remove(customer);
        }

        private Customer getCustomer(string customerId)
        {
            PrimitiveMapper mapper = new PrimitiveMapper();
            Guid customerGuid = mapper.ToGuid(customerId);
            Customer customer = customerRepository.GetCustomer(customerGuid);
            if (customer == null)
            {
                throw new AdapterException(HttpStatusCode.NotFound, "A customer with the given ID was not found.");
            }
            return customer;
        }
    }
}
