using System;
using System.Web.Mvc;
using TestMvcApplication.Models;

namespace TestMvcApplication.Controllers
{
    public partial class ErrorController : Controller
    {
        public virtual ActionResult Index(Error error)
        {
            return View(Views.Index, error);
        }
    }
}
