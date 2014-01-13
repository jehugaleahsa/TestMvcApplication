using System;
using System.Web.Mvc;
using MvcUtilities;
using MvcUtilities.FilterAttributes;
using TestMvcApplication.Context;

namespace TestMvcApplication.Controllers
{
    [Trace]
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

        public virtual ActionResult Index(ErrorData error = null)
        {
            if (error == null)
            {
                error = new ErrorData();
            }
            if (String.IsNullOrWhiteSpace(error.ErrorMessage))
            {
                error.ErrorMessage = "An error occurred while processing your request.";
            }
            if (error.ReturnUrl == null || !urlHelper.IsSafe(error.ReturnUrl))
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
