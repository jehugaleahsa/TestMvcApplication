using System;
using ServiceInterfaces.Entities;
using Adapters.Models;

namespace Adapters.Mappers
{
    public class CustomerMapper
    {
        public Customer Convert(CustomerData data)
        {
            PrimitiveMapper mapper = new PrimitiveMapper();
            Customer customer = new Customer();
            if (!String.IsNullOrWhiteSpace(data.CustomerId))
            {
                customer.CustomerId = mapper.ToGuid(data.CustomerId);
            }
            customer.Name = data.Name;
            customer.BirthDate = mapper.ToDateTime(data.BirthDate);
            customer.Height = data.Height;
            return customer;
        }

        public CustomerData Convert(Customer customer)
        {
            PrimitiveMapper mapper = new PrimitiveMapper();
            CustomerData result = new CustomerData();
            result.CustomerId = mapper.ToString(customer.CustomerId);
            result.Name = customer.Name;
            result.BirthDate = mapper.ToString(customer.BirthDate);
            result.Height = customer.Height;
            return result;
        }
    }
}
