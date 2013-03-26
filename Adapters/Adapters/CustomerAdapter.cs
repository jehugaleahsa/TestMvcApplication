using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public virtual CustomerData GetCustomer(string customerId)
        {
            Customer customer = getCustomer(parseGuid(customerId));
            return toCustomerData(customer);
        }

        [ErrorMessage("An error occurred while retrieving the customers.")]
        public virtual IEnumerable<CustomerData> GetCustomers()
        {
            return customerRepository.GetCustomers().Select(toCustomerData).ToList();
        }

        [ErrorMessage("An error occurred while adding the customer.")]
        public virtual CustomerData AddCustomer(CustomerData customerData)
        {
            Customer customer = toCustomer(customerData);
            customerRepository.Add(customer);
            return toCustomerData(customer);
        }

        [ErrorMessage("An error occurred while updating the customer.")]
        public virtual void UpdateCustomer(CustomerData customerData)
        {
            Customer original = getCustomer(parseGuid(customerData.CustomerId));
            Customer modified = toCustomer(customerData);
            customerRepository.Update(original, modified);
        }

        [ErrorMessage("An error occurred while removing the customer.")]
        public virtual void RemoveCustomer(string customerId)
        {
            Customer customer = getCustomer(parseGuid(customerId));
            customerRepository.Remove(customer);
        }

        private Customer getCustomer(Guid customerId)
        {
            Customer customer = customerRepository.GetCustomer(customerId);
            if (customer == null)
            {
                throw new AdapterException(HttpStatusCode.NotFound, "A customer with the given ID was not found.");
            }
            return customer;
        }

        private static Customer toCustomer(CustomerData customerData)
        {
            Customer customer = new Customer();
            if (!String.IsNullOrWhiteSpace(customerData.CustomerId))
            {
                customer.CustomerId = parseGuid(customerData.CustomerId);
            }
            customer.Name = customerData.Name;
            customer.BirthDate = parseDateTime(customerData.BirthDate);
            customer.Height = customerData.Height;
            return customer;
        }

        private static CustomerData toCustomerData(Customer customer)
        {
            CustomerData result = new CustomerData();
            result.CustomerId = customer.CustomerId.ToString("N");
            result.Name = customer.Name;
            result.BirthDate = customer.BirthDate.ToString("d");
            result.Height = customer.Height;
            return result;
        }

        private static Guid parseGuid(string value)
        {
            try
            {
                return Guid.ParseExact(value, "N");
            }
            catch (Exception exception)
            {
                throw new AdapterException(HttpStatusCode.BadRequest, "Encountered an invalid ID.", exception);
            }
        }

        private static DateTime parseDateTime(string value)
        {
            try
            {
                return DateTime.Parse(value);
            }
            catch (Exception exception)
            {
                throw new AdapterException(HttpStatusCode.BadRequest, "Encountered an invalid date.", exception);
            }
        }
    }
}
