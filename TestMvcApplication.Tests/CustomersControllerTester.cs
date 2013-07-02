using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Adapters;
using TestMvcApplication.Controllers;
using NSubstitute;
using Adapters.Models;
using System.Net.Http;
using System.Net;
using System.Web.Http.Hosting;
using System.Web.Http;

namespace TestMvcApplication.Tests
{
    [TestClass]
    public class CustomersControllerTester
    {
        #region Ctor

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowExceptionIfAdapterNull()
        {
            ICustomerAdapter adapter = null;
            new CustomersController(adapter);
        }

        #endregion

        #region Get

        [TestMethod]
        public void ShouldGetCustomerById()
        {
            ICustomerAdapter adapter = Substitute.For<ICustomerAdapter>();
            CustomerData data = new CustomerData();
            adapter.GetCustomer(Arg.Any<string>()).Returns(data);
            CustomersController controller = new CustomersController(adapter);

            string customerId = Guid.NewGuid().ToString("N");
            CustomerData result = controller.Get(customerId);

            Assert.AreSame(data, result, "The wrong customer was returned.");

            adapter.Received().GetCustomer(customerId);
        }

        #endregion

        #region GetAll

        [TestMethod]
        public void ShouldGetAllCustomers()
        {
            ICustomerAdapter adapter = Substitute.For<ICustomerAdapter>();
            IEnumerable<CustomerData> data = new List<CustomerData>() { new CustomerData() };
            adapter.GetCustomers().Returns(data);
            CustomersController controller = new CustomersController(adapter);

            CustomerData[] results = controller.GetAll();

            Assert.AreEqual(1, results.Length, "There should have been one customer.");
            Assert.AreSame(data.First(), results[0], "The wrong customer was returned.");

            adapter.Received().GetCustomers();
        }

        #endregion

        #region Post

        [TestMethod]
        public void ShouldCreateAndReturnCustomer()
        {
            ICustomerAdapter adapter = Substitute.For<ICustomerAdapter>();
            CustomerData added = new CustomerData();
            adapter.AddCustomer(Arg.Any<CustomerData>()).Returns(added);
            CustomersController controller = new CustomersController(adapter);
            controller.Request = new HttpRequestMessage(HttpMethod.Post, "~/api/customers");
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            CustomerData data = new CustomerData();
            HttpResponseMessage message = controller.Post(data);

            Assert.AreEqual(HttpStatusCode.Created, message.StatusCode, "The wrong status code was returned.");
            Assert.IsInstanceOfType(message.Content, typeof(ObjectContent), "The content was not the serialized result.");
            ObjectContent content = (ObjectContent)message.Content;
            Assert.AreSame(added, content.Value, "The wrong object was returned.");

            adapter.Received().AddCustomer(data);
        }

        #endregion

        #region Put

        [TestMethod]
        public void ShouldUpdateCustomer()
        {
            ICustomerAdapter adapter = Substitute.For<ICustomerAdapter>();
            adapter.UpdateCustomer(Arg.Any<CustomerData>());
            CustomersController controller = new CustomersController(adapter);
            controller.Request = new HttpRequestMessage(HttpMethod.Put, "~/api/customers");
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            CustomerData data = new CustomerData();
            HttpResponseMessage message = controller.Put(data);

            Assert.AreEqual(HttpStatusCode.OK, message.StatusCode, "The wrong status code was returned.");
            Assert.IsInstanceOfType(message.Content, typeof(ObjectContent), "The content was not the serialized result.");
            ObjectContent content = (ObjectContent)message.Content;
            Assert.AreSame("The customer was updated.", content.Value, "The wrong object was returned.");

            adapter.Received().UpdateCustomer(data);
        }

        #endregion

        #region Delete

        [TestMethod]
        public void ShouldDeleteCustomer()
        {
            ICustomerAdapter adapter = Substitute.For<ICustomerAdapter>();
            adapter.RemoveCustomer(Arg.Any<string>());
            CustomersController controller = new CustomersController(adapter);
            controller.Request = new HttpRequestMessage(HttpMethod.Delete, "~/api/customers");
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            string customerId = Guid.NewGuid().ToString("N");
            HttpResponseMessage message = controller.Delete(customerId);

            Assert.AreEqual(HttpStatusCode.OK, message.StatusCode, "The wrong status code was returned.");
            Assert.IsInstanceOfType(message.Content, typeof(ObjectContent), "The content was not the serialized result.");
            ObjectContent content = (ObjectContent)message.Content;
            Assert.AreSame("The customer was deleted.", content.Value, "The wrong object was returned.");

            adapter.Received().RemoveCustomer(customerId);
        }

        #endregion
    }
}
