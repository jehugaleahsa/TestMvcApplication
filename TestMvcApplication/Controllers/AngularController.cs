using System;
using System.Web.Mvc;
using MvcUtilities.FilterAttributes;

namespace TestMvcApplication.Controllers
{
    [Trace]
    public partial class AngularController : Controller
    {
        [OutputCache(CacheProfile = "page_cache")]
        public virtual ActionResult Index()
        {
            return View(Views.Index);
        }
    }
}
