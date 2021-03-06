﻿using System;
using System.Web.Mvc;

namespace MvcUtilities.FilterAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public sealed class RedirectOnErrorAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of a RedirectOnErrorAttribute.
        /// </summary>
        public RedirectOnErrorAttribute()
        {
        }

        /// <summary>
        /// Gets or sets the controller the user will return to after viewing the error.
        /// </summary>
        public string ReturnController { get; set; }

        /// <summary>
        /// Gets or sets the action the user will return to after viewing the error.
        /// </summary>
        public string ReturnAction { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Redirects the user to an error screen if an exception is thrown.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            if (filterContext.Exception != null && !filterContext.ExceptionHandled)
            {
                ErrorData viewModel = new ErrorData();
                setErrorMessage(filterContext, viewModel);
                UrlHelper helper = new UrlHelper(filterContext.RequestContext);
                setReturnUrl(filterContext, viewModel, helper);
                setActionResult(filterContext, viewModel, helper);
                filterContext.ExceptionHandled = true;
            }
            base.OnActionExecuted(filterContext);
        }

        private void setErrorMessage(ActionExecutedContext filterContext, ErrorData viewModel)
        {
            if (String.IsNullOrWhiteSpace(ErrorMessage))
            {
                CodedException exception = filterContext.Exception as CodedException;
                if (exception == null)
                {
                    viewModel.ErrorMessage = "An error occurred while processing your request.";
                }
                else
                {
                    viewModel.ErrorMessage = filterContext.Exception.Message;
                }
            }
            else
            {
                viewModel.ErrorMessage = ErrorMessage;
            }
        }

        private void setReturnUrl(ActionExecutedContext filterContext, ErrorData viewModel, UrlHelper helper)
        {
            string returnController = ReturnController;
            if (String.IsNullOrWhiteSpace(returnController))
            {
                returnController = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            }
            string returnAction = ReturnAction;
            if (String.IsNullOrWhiteSpace(returnAction))
            {
                returnAction = "Index";
            }
            viewModel.ReturnUrl = helper.Action(returnAction, returnController);
        }

        private static void setActionResult(ActionExecutedContext filterContext, ErrorData viewModel, UrlHelper helper)
        {
            if (helper.RouteCollection["Error"] == null)
            {
                string url = helper.Action("Index", "Error", viewModel);
                filterContext.Result = new RedirectResult(url);
            }
            else
            {
                string url = helper.RouteUrl("Error", viewModel);
                filterContext.Result = new RedirectResult(url);
            }
        }
    }
}