using System;
using System.Web.Mvc;
using TestMvcApplication.Models;

namespace TestMvcApplication.Controllers
{
    public partial class ErrorController : Controller
    {
        private readonly ContextManager manager;

        public ErrorController(ContextManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            this.manager = manager;
        }

        public virtual ActionResult Index(Error error = null)
        {
            if (error == null)
            {
                error = new Error()
                {
                    ErrorMessage = "An error occurred while processing your request.",
                    ReturnUrl = manager.Action(MVC.Classic.Index()),
                };
            }
            return View(Views.Index, error);
        }

        public virtual ActionResult NotFound()
        {
            return View(Views.NotFound);
        }
    }
}
