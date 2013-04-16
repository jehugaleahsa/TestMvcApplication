using System;
using System.Web.Mvc;
using MvcUtilities.Models;
using TestMvcApplication.Context;

namespace TestMvcApplication.Controllers
{
    public partial class ErrorController : Controller
    {
        private readonly IUrlHelper urlHelper;

        public ErrorController(IUrlHelper urlHelper)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException("manager");
            }
            this.urlHelper = urlHelper;
        }

        public virtual ActionResult Index(Error error = null)
        {
            if (error == null)
            {
                error = new Error();
            }
            if (String.IsNullOrWhiteSpace(error.ErrorMessage))
            {
                error.ErrorMessage = "An error occurred while processing your request.";
            }
            if (String.IsNullOrWhiteSpace(error.ReturnUrl) || !urlHelper.IsSafe(error.ReturnUrl))
            {
                error.ReturnUrl = urlHelper.Action(MVC.Classic.Index());
            }
            return View(Views.Index, error);
        }

        public virtual ActionResult NotFound()
        {
            return View(Views.NotFound);
        }
    }
}
