using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Adapters.Mappers;
using ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using DataObjects;
using ServiceInterfaces;

namespace Adapters.Tests
{
    [TestClass]
    public class AddressItemAdapterTester
    {
        #region GetAddressItems

        [TestMethod]
        public void ShouldThrowIfGettingAddressItemsForUnknownCustomer()
        {
            ICustomerRepository customerRepository = Substitute.For<ICustomerRepository>();
            Customer customer = null;
            customerRepository.GetCustomer(Arg.Any<Guid>()).Returns(customer);
            IAddressItemRepository itemRepository = Substitute.For<IAddressItemRepository>();

            try
            {
                IAddressItemAdapter adapter = new AddressItemAdapter(customerRepository, itemRepository);
                string customerId = Guid.Empty.ToString("N");
                adapter.GetAddressItems(customerId);
                Assert.Fail("A customer was returned unexpectantly.");
            }
            catch (AdapterException exception)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, exception.StatusCode, "The wrong status code was returned.");
                customerRepository.Received(1).GetCustomer(Arg.Any<Guid>());
            }
        }

        [TestMethod]
        public void ShouldConvertReturnedAddressItemsToViewModel()
        {
            ICustomerRepository customerRepository = Substitute.For<ICustomerRepository>();
            Customer customer = new Customer();
            customerRepository.GetCustomer(Arg.Any<Guid>()).Returns(customer);

            IAddressItemRepository itemRepository = Substitute.For<IAddressItemRepository>();
            AddressItem[] items = new AddressItem[] { new AddressItem() };
            itemRepository.GetAddressItems(customer).Returns(items);

            IAddressItemMapper mapper = Substitute.For<IAddressItemMapper>();
            mapper.Convert(Arg.Any<AddressItem>()).Returns(new AddressItemData());

            AddressItemAdapter adapter = new AddressItemAdapter(customerRepository, itemRepository);
            adapter.AddressItemMapper = mapper;
            string customerId = Guid.Empty.ToString("N");
            IEnumerable<AddressItemData> models = adapter.GetAddressItems(customerId);

            Assert.AreEqual(1, models.Count(), "The wrong number of view models were returned.");
            customerRepository.Received(1).GetCustomer(Arg.Any<Guid>());
            itemRepository.Received(1).GetAddressItems(customer);
            mapper.Received(1).Convert(Arg.Any<AddressItem>());
        }

        #endregion
    }
}
