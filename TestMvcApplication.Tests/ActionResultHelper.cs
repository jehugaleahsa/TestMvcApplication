using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMvcApplication.Tests
{
    public class ActionResultHelper
    {
        public static void AssertView(ActionResult result, string expectedViewName)
        {
            getViewResult(result, expectedViewName);
        }

        public static TModel AssertViewWithModel<TModel>(ActionResult result, string expectedViewName)
        {
            ViewResult view = getViewResult(result, expectedViewName);
            Assert.IsInstanceOfType(view.ViewData.Model, typeof(TModel), "The view model was not set as expected.");
            TModel model = (TModel)view.ViewData.Model;
            return model;
        }

        private static ViewResult getViewResult(ActionResult result, string expectedViewName)
        {
            Assert.IsInstanceOfType(result, typeof(ViewResult), "A view was not returned.");
            ViewResult view = (ViewResult)result;
            Assert.AreEqual(expectedViewName, view.ViewName, "The wrong view was loaded.");
            return view;
        }

        public static void AssertRedirected(ActionResult result, string controllerName, string actionName)
        {
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult), "The user was not redirected.");
            RedirectToRouteResult redirect = (RedirectToRouteResult)result;
            Assert.AreEqual(controllerName, redirect.RouteValues["controller"], "The user was redirected to the wrong controller.");
            Assert.AreEqual(actionName, redirect.RouteValues["action"], "The user was redirected to the wrong action.");
        }
    }
}
