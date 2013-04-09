using Adapters.Mappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Adapters.Models;
using ServiceInterfaces.Entities;
using System;

namespace Adapters.Tests
{
    [TestClass]
    public class CustomerMapperTester
    {
        [TestMethod]
        public void ShouldConvertViewModelToDTOIgnoringMissingCustomerId()
        {
            CustomerData viewModel = new CustomerData()
            {
                CustomerId = null,
                Name = "Bob",
                Height = 123,
                BirthDate = "12/31/2012"
            };

            CustomerMapper mapper = new CustomerMapper();
            Customer customer = mapper.Convert(viewModel);

            Assert.AreEqual(DateTime.Parse(viewModel.BirthDate), customer.BirthDate, "The birth date was not mapped.");
            Assert.AreEqual(viewModel.Height, customer.Height, "The height was not mapped.");
            Assert.AreEqual(viewModel.Name, customer.Name, "The name was not mapped.");
        }

        [TestMethod]
        public void ShouldConvertViewModelToDTO()
        {
            CustomerData viewModel = new CustomerData()
            {
                CustomerId = Guid.NewGuid().ToString("N"),
                Name = "Bob",
                Height = 123,
                BirthDate = "12/31/2012"
            };

            CustomerMapper mapper = new CustomerMapper();
            Customer customer = mapper.Convert(viewModel);

            Assert.AreEqual(DateTime.Parse(viewModel.BirthDate), customer.BirthDate, "The birth date was not mapped.");
            Assert.AreEqual(Guid.Parse(viewModel.CustomerId), customer.CustomerId, "The customer ID was not mapped.");
            Assert.AreEqual(viewModel.Height, customer.Height, "The height was not mapped.");
            Assert.AreEqual(viewModel.Name, customer.Name, "The name was not mapped.");
        }

        [TestMethod]
        public void ShouldConvertDTOToViewModel()
        {
            Customer customer = new Customer()
            {
                CustomerId = Guid.NewGuid(),
                Name = "Bob",
                Height = 123,
                BirthDate = new DateTime(2012, 12, 31)
            };

            CustomerMapper mapper = new CustomerMapper();
            CustomerData viewModel = mapper.Convert(customer);

            Assert.AreEqual(customer.BirthDate, DateTime.Parse(viewModel.BirthDate), "The birth date was not mapped.");
            Assert.AreEqual(customer.CustomerId, Guid.Parse(viewModel.CustomerId), "The customer ID was not mapped.");
            Assert.AreEqual(customer.Height, viewModel.Height, "The height was not mapped.");
            Assert.AreEqual(customer.Name, viewModel.Name, "The name was not mapped.");
        }
    }
}
