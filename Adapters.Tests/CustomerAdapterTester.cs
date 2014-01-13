using System;
using System.Collections.Generic;
using System.Linq;
using Adapters.Mappers;
using ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using DataObjects;
using ServiceInterfaces;

namespace Adapters.Tests
{
    [TestClass]
    public class CustomerAdapterTester
    {
        #region Ctor

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowExceptionIfNullPassedToConstructor()
        {
            new CustomerAdapter(null);
        }

        [TestMethod]
        public void ShouldSetDefaultMappers()
        {
            ICustomerRepository repository = Substitute.For<ICustomerRepository>();

            CustomerAdapter adapter = new CustomerAdapter(repository);

            Assert.IsInstanceOfType(adapter.CustomerMapper, typeof(CustomerMapper), "The wrong customer mapper was set by default.");
        }

        #endregion

        #region GetCustomer

        [TestMethod]
        [ExpectedException(typeof(AdapterException))]
        public void ShouldThrowExceptionIfCustomerNotFound()
        {
            ICustomerRepository repository = Substitute.For<ICustomerRepository>();
            setReturnedCustomer(repository, null);

            CustomerAdapter adapter = new CustomerAdapter(repository);
            adapter.GetCustomer(Guid.Empty.ToString("N"));
        }

        [TestMethod]
        public void ShouldMapCustomerToViewModel()
        {
            ICustomerRepository repository = Substitute.For<ICustomerRepository>();
            Customer dto = new Customer();
            setReturnedCustomer(repository, dto);

            ICustomerMapper mapper = Substitute.For<ICustomerMapper>();
            CustomerData viewModel = new CustomerData();
            mapper.Convert(dto).Returns(viewModel);

            CustomerAdapter adapter = new CustomerAdapter(repository) { CustomerMapper = mapper };
            PrimitiveMapper primitiveMapper = new PrimitiveMapper();
            CustomerData data = adapter.GetCustomer(primitiveMapper.ToString(Guid.Empty));

            repository.Received().GetCustomer(dto.CustomerId);
            mapper.Received().Convert(dto);

            Assert.IsNotNull(data, "The returned view model was null.");
        }

        #endregion

        #region GetCustomers

        [TestMethod]
        public void ShouldGetCustomersFromRepository()
        {
            ICustomerRepository repository = Substitute.For<ICustomerRepository>();
            Customer[] customers = new Customer[] { new Customer() };
            repository.GetCustomers().Returns(customers);

            ICustomerMapper mapper = Substitute.For<ICustomerMapper>();
            mapper.Convert(Arg.Any<Customer>()).Returns(new CustomerData());

            CustomerAdapter adapter = new CustomerAdapter(repository) { CustomerMapper = mapper };
            IEnumerable<CustomerData> results = adapter.GetCustomers();

            repository.Received().GetCustomers();
            mapper.Received(1).Convert(customers[0]);

            Assert.IsNotNull(results, "Null was returned by the adapter.");
            Assert.AreEqual(1, results.Count(), "The wrong number of view models were returned.");
        }

        #endregion

        #region AddCustomer

        [TestMethod]
        public void ShouldAddConvertedCustomerToRepositoryAndReturnUpdated()
        {
            ICustomerRepository repository = Substitute.For<ICustomerRepository>();
            ICustomerMapper mapper = Substitute.For<ICustomerMapper>();
            Customer dto = new Customer();
            CustomerData viewModelInput = new CustomerData();
            CustomerData viewModelOutput = new CustomerData();
            mapper.Convert(viewModelInput).Returns(dto);
            mapper.Convert(dto).Returns(viewModelOutput);

            CustomerAdapter adapter = new CustomerAdapter(repository) { CustomerMapper = mapper };
            
            CustomerData result = adapter.AddCustomer(viewModelInput);

            Assert.AreSame(viewModelOutput, result, "The updated view model was not returned.");

            mapper.Received().Convert(viewModelInput);
            repository.Received().Add(dto);
            mapper.Received().Convert(dto);
        }

        #endregion

        #region UpdateCustomer

        [TestMethod]
        public void ShouldFindOriginalAndPassModifiedToRepository()
        {
            ICustomerRepository repository = Substitute.For<ICustomerRepository>();
            Customer original = new Customer();
            setReturnedCustomer(repository, original);

            ICustomerMapper mapper = Substitute.For<ICustomerMapper>();
            Customer modified = new Customer(); 
            CustomerData viewModel = new CustomerData() { CustomerId = Guid.Empty.ToString("N") };
            mapper.Convert(Arg.Any<CustomerData>()).Returns(modified);

            CustomerAdapter adapter = new CustomerAdapter(repository) { CustomerMapper = mapper };
            adapter.UpdateCustomer(viewModel);

            repository.Received().GetCustomer(Arg.Any<Guid>());
            mapper.Received().Convert(viewModel);
            repository.Received().Update(original, modified);
        }

        #endregion

        #region RemoveCustomer

        [TestMethod]
        public void ShouldFindCustomerAndRemoveIt()
        {
            ICustomerRepository repository = Substitute.For<ICustomerRepository>();
            Customer dto = new Customer();
            setReturnedCustomer(repository, dto);

            CustomerAdapter adapter = new CustomerAdapter(repository);
            adapter.RemoveCustomer(Guid.Empty.ToString("N"));

            repository.Received().GetCustomer(Guid.Empty);
            repository.Received().Remove(dto);
        }

        #endregion

        private static void setReturnedCustomer(ICustomerRepository repository, Customer customer)
        {
            repository.GetCustomer(Arg.Any<Guid>()).Returns(customer);
        }
    }
}
