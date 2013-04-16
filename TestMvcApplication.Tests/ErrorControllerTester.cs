using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcUtilities.Models;
using NSubstitute;
using TestMvcApplication.Context;
using TestMvcApplication.Controllers;

namespace TestMvcApplication.Tests
{
    [TestClass]
    public class ErrorControllerTester
    {
        #region Ctor

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowExceptionIfUrlHelperIsNull()
        {
            IUrlHelper helper = null;
            new ErrorController(helper);
        }

        #endregion

        #region Index

        [TestMethod]
        public void ShouldDisplayDefaultErrorMessageIfErrorNotProvided()
        {
            IUrlHelper helper = Substitute.For<IUrlHelper>();
            const string returnUrl = "/return/url";
            helper.Action(Arg.Any<ActionResult>()).Returns(returnUrl);
            ErrorController controller = new ErrorController(helper);

            ActionResult result = controller.Index();

            Error model = ActionResultHelper.AssertViewWithModel<Error>(result, controller.Views.Index);

            Assert.AreEqual("An error occurred while processing your request.", model.ErrorMessage, "The wrong error message was set.");
            Assert.AreEqual(returnUrl, model.ReturnUrl, "The wrong return URL was set.");
        }

        [TestMethod]
        public void ShouldUseGivenErrorMessageAndReturnUrlIfProvided()
        {
            IUrlHelper helper = Substitute.For<IUrlHelper>();
            helper.IsSafe(Arg.Any<string>()).Returns(true);
            ErrorController controller = new ErrorController(helper);

            Error error = new Error()
            {
                ErrorMessage = "Custom error message",
                ReturnUrl = "/return/url",
            };
            ActionResult result = controller.Index(error);

            Error model = ActionResultHelper.AssertViewWithModel<Error>(result, controller.Views.Index);

            Assert.AreEqual(error.ErrorMessage, model.ErrorMessage, "The wrong error message was set.");
            Assert.AreEqual(error.ReturnUrl, model.ReturnUrl, "The wrong return URL was set.");
        }

        [TestMethod]
        public void ShouldChangeReturnUrlIfNotSafe()
        {
            IUrlHelper helper = Substitute.For<IUrlHelper>();
            helper.IsSafe(Arg.Any<string>()).Returns(false);
            string returnUrl = "/return/url";
            helper.Action(Arg.Any<ActionResult>()).Returns(returnUrl);
            ErrorController controller = new ErrorController(helper);

            Error error = new Error()
            {
                ErrorMessage = "Custom error message",
                ReturnUrl = @"http://www.google.com",
            };
            ActionResult result = controller.Index(error);

            Error model = ActionResultHelper.AssertViewWithModel<Error>(result, controller.Views.Index);

            Assert.AreEqual(error.ErrorMessage, model.ErrorMessage, "The wrong error message was set.");
            Assert.AreEqual(returnUrl, model.ReturnUrl, "The wrong return URL was set.");
        }

        #endregion

        #region NotFound

        [TestMethod]
        public void ShouldDisplayNotFoundView()
        {
            IUrlHelper helper = Substitute.For<IUrlHelper>();
            ErrorController controller = new ErrorController(helper);
            ActionResult result = controller.NotFound();
            ActionResultHelper.AssertView(result, controller.Views.NotFound);
        }

        #endregion
    }
}
