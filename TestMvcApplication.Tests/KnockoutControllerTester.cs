using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Adapters;
using Adapters.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using TestMvcApplication.Controllers;

namespace TestMvcApplication.Tests
{
    [TestClass]
    public class KnockoutControllerTester
    {
        #region Ctor

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowExceptionIfAdapterNull()
        {
            ICustomerAdapter adapter = null;
            new KnockoutController(adapter);
        }

        #endregion

        #region Index

        [TestMethod]
        public void ShouldDisplayIndex()
        {
            ICustomerAdapter adapter = Substitute.For<ICustomerAdapter>();
            KnockoutController controller = new KnockoutController(adapter);

            ActionResult result = controller.Index();

            ActionResultHelper.AssertView(result, controller.Views.Index);
        }

        #endregion

        #region Load

        [TestMethod]
        public void ShouldLoadCustomers()
        {
            ICustomerAdapter adapter = Substitute.For<ICustomerAdapter>();
            IEnumerable<CustomerData> data = new List<CustomerData>() { new CustomerData() };
            adapter.GetCustomers().Returns(data);
            KnockoutController controller = new KnockoutController(adapter);

            ActionResult result = controller.Load();

            CustomerData[] model = ActionResultHelper.AssertJson<CustomerData[]>(result);
            Assert.AreEqual(1, model.Length, "There should have been one customer.");
            Assert.AreSame(data.First(), model[0], "The wrong customer was returned.");

            adapter.Received().GetCustomers();
        }

        #endregion

        #region Create

        [TestMethod]
        public void ShouldCreateAndReturnCustomer()
        {
            ICustomerAdapter adapter = Substitute.For<ICustomerAdapter>();
            CustomerData added = new CustomerData();
            adapter.AddCustomer(Arg.Any<CustomerData>()).Returns(added);
            KnockoutController controller = new KnockoutController(adapter);

            CustomerData data = new CustomerData();
            ActionResult result = controller.Create(data);

            CustomerData model = ActionResultHelper.AssertJsonWithHttpStatusCode<CustomerData>(result, HttpStatusCode.Created);
            Assert.AreSame(added, model, "The added customer was not returned.");

            adapter.Received().AddCustomer(data);
        }
        
        #endregion

        #region Edit

        [TestMethod]
        public void ShouldUpdateCustomer()
        {
            ICustomerAdapter adapter = Substitute.For<ICustomerAdapter>();
            KnockoutController controller = new KnockoutController(adapter);

            CustomerData data = new CustomerData();
            ActionResult result = controller.Edit(data);

            ActionResultHelper.AssertHttpStatusCode(result, HttpStatusCode.OK);

            adapter.Received().UpdateCustomer(data);
        }

        #endregion

        #region Delete

        [TestMethod]
        public void ShouldDeleteCustomer()
        {
            ICustomerAdapter adapter = Substitute.For<ICustomerAdapter>();
            KnockoutController controller = new KnockoutController(adapter);

            string customerId = Guid.NewGuid().ToString("N");
            ActionResult result = controller.Delete(customerId);

            ActionResultHelper.AssertHttpStatusCode(result, HttpStatusCode.OK);

            adapter.Received().RemoveCustomer(customerId);
        }

        #endregion
    }
}
