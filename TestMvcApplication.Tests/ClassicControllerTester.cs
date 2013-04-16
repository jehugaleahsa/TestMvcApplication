using System.Linq;
using System.Web.Mvc;
using Adapters;
using Adapters.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using TestMvcApplication.Controllers;
using System;

namespace TestMvcApplication.Tests
{
    [TestClass]
    public class ClassicControllerTester
    {
        [TestMethod]
        public void ShouldBuildIndexViewWithCustomersOrderByName()
        {
            ICustomerAdapter adapter = Substitute.For<ICustomerAdapter>();
            CustomerData[] customers = new CustomerData[] 
            { 
                new CustomerData() { Name = "Gary" }, 
                new CustomerData() { Name = "Louis" }, 
                new CustomerData() { Name = "Bob" } 
            };
            adapter.GetCustomers().Returns(customers);

            ClassicController controller = new ClassicController(adapter);
            ActionResult result = controller.Index();

            adapter.Received().GetCustomers();

            CustomerData[] model = ActionResultHelper.AssertViewWithModel<CustomerData[]>(result, controller.Views.Index);
            string[] names = model.Select(customer => customer.Name).ToArray();
            string[] expected = new string[] { "Bob", "Gary", "Louis" };
            CollectionAssert.AreEqual(expected, names, "The customers were not ordered by name.");
        }

        [TestMethod]
        public void ShouldBuildCreateViewWithNoModel()
        {
            ICustomerAdapter adapter = Substitute.For<ICustomerAdapter>();
            ClassicController controller = new ClassicController(adapter);
            ActionResult result = controller.Create();

            ActionResultHelper.AssertView(result, controller.Views.Create);
        }

        [TestMethod]
        public void ShouldCreateCustomerAndReturnToIndex()
        {
            ICustomerAdapter adapter = Substitute.For<ICustomerAdapter>();
            ClassicController controller = new ClassicController(adapter);

            CustomerData data = new CustomerData();
            ActionResult result = controller.Create(data);

            adapter.Received().AddCustomer(data);

            ActionResultHelper.AssertRedirected(result, controller.Name, controller.ActionNames.Index);
        }

        [TestMethod]
        public void ShouldRedisplayViewIfCreateValidationFails()
        {
            ICustomerAdapter adapter = Substitute.For<ICustomerAdapter>();
            ClassicController controller = new ClassicController(adapter);
            controller.ModelState.AddModelError("txtName", "You must provide a customer name.");

            CustomerData data = new CustomerData();
            ActionResult result = controller.Create(data);

            CustomerData model = ActionResultHelper.AssertViewWithModel<CustomerData>(result, controller.Views.Create);
            Assert.AreSame(data, model, "The customer data was not set as the model.");
        }

        [TestMethod]
        public void ShouldRetrieveCustomerForEdit()
        {
            ICustomerAdapter adapter = Substitute.For<ICustomerAdapter>();
            CustomerData data = new CustomerData();
            adapter.GetCustomer(Arg.Any<string>()).Returns(data);

            ClassicController controller = new ClassicController(adapter);

            string customerId = Guid.NewGuid().ToString("N");
            ActionResult result = controller.Edit(customerId);

            CustomerData model = ActionResultHelper.AssertViewWithModel<CustomerData>(result, controller.Views.Edit);

            adapter.Received().GetCustomer(customerId);

            Assert.AreSame(data, model, "The customer data was not passed to the view.");
        }

        [TestMethod]
        public void ShouldUpdateCustomerAndRedirectToIndex()
        {
            ICustomerAdapter adapter = Substitute.For<ICustomerAdapter>();
            ClassicController controller = new ClassicController(adapter);

            CustomerData data = new CustomerData();
            ActionResult result = controller.Edit(data);

            adapter.Received().UpdateCustomer(data);

            ActionResultHelper.AssertRedirected(result, controller.Name, controller.ActionNames.Index);
        }

        [TestMethod]
        public void ShouldRedisplayViewWhenEditValidationFails()
        {
            ICustomerAdapter adapter = Substitute.For<ICustomerAdapter>();
            ClassicController controller = new ClassicController(adapter);
            controller.ModelState.AddModelError("txtName", "You must provide a name for the customer.");

            CustomerData data = new CustomerData();
            ActionResult result = controller.Edit(data);

            CustomerData model = ActionResultHelper.AssertViewWithModel<CustomerData>(result, controller.Views.Edit);
            Assert.AreSame(data, model, "The model was not passed to the view.");
        }

        [TestMethod]
        public void ShouldRetrieveCustomerForDelete()
        {
            ICustomerAdapter adapter = Substitute.For<ICustomerAdapter>();
            CustomerData data = new CustomerData();
            adapter.GetCustomer(Arg.Any<string>()).Returns(data);

            ClassicController controller = new ClassicController(adapter);

            string customerId = Guid.NewGuid().ToString("N");
            ActionResult result = controller.Delete(customerId);

            CustomerData model = ActionResultHelper.AssertViewWithModel<CustomerData>(result, controller.Views.Delete);

            adapter.Received().GetCustomer(customerId);

            Assert.AreSame(data, model, "The customer data was not passed to the view.");
        }

        [TestMethod]
        public void ShouldDeleteCustomerAndRedirectToIndex()
        {
            ICustomerAdapter adapter = Substitute.For<ICustomerAdapter>();
            ClassicController controller = new ClassicController(adapter);

            CustomerData data = new CustomerData() { CustomerId = Guid.NewGuid().ToString("N") };
            ActionResult result = controller.Delete(data);

            adapter.Received().RemoveCustomer(data.CustomerId);

            ActionResultHelper.AssertRedirected(result, controller.Name, controller.ActionNames.Index);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowExceptionIfAdapterIsNull()
        {
            ICustomerAdapter adapter = null;
            new ClassicController(adapter);
        }
    }
}
