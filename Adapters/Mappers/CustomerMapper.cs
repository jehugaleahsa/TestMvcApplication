using System;
using Adapters.Models;
using ServiceInterfaces.Entities;

namespace Adapters.Mappers
{
    public class CustomerMapper : ICustomerMapper
    {
        public Customer Convert(CustomerData viewModel)
        {
            PrimitiveMapper mapper = new PrimitiveMapper();
            Customer customer = new Customer();
            if (!String.IsNullOrWhiteSpace(viewModel.CustomerId))
            {
                customer.CustomerId = mapper.ToGuid(viewModel.CustomerId);
            }
            customer.Name = viewModel.Name;
            customer.BirthDate = mapper.ToDateTime(viewModel.BirthDate);
            customer.Height = viewModel.Height;
            return customer;
        }

        public CustomerData Convert(Customer customer)
        {
            PrimitiveMapper mapper = new PrimitiveMapper();
            CustomerData viewModel = new CustomerData();
            viewModel.CustomerId = mapper.ToString(customer.CustomerId);
            viewModel.Name = customer.Name;
            viewModel.BirthDate = mapper.ToString(customer.BirthDate);
            viewModel.Height = customer.Height;
            return viewModel;
        }
    }
}
