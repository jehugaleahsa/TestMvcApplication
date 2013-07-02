using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Adapters;
using Adapters.Models;
using MvcUtilities.ActionResults;
using MvcUtilities.FilterAttributes;

namespace TestMvcApplication.Controllers
{
    [Trace]
    public partial class KnockoutController : Controller
    {
        private readonly ICustomerAdapter adapter;

        public KnockoutController(ICustomerAdapter adapter)
        {
            if (adapter == null)
            {
                throw new ArgumentNullException("adapter");
            }
            this.adapter = adapter;
        }

        [OutputCache(CacheProfile="page_cache")]
        public virtual ActionResult Index()
        {
            return View(Views.Index);
        }

        [CodedExceptionHandler]
        public virtual ActionResult Load()
        {
            CustomerData[] data = adapter.GetCustomers().ToArray();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CodedExceptionHandler]
        public virtual ActionResult Create(CustomerData data)
        {
            CustomerData result = adapter.AddCustomer(data);
            return new JsonWithHttpStatusCodeResult(HttpStatusCode.Created) { Data = result };
        }

        [HttpPut]
        [CodedExceptionHandler]
        public virtual ActionResult Edit(CustomerData data)
        {
            adapter.UpdateCustomer(data);
            return new HttpStatusCodeResult(HttpStatusCode.OK, "The customer was updated.");
        }

        [HttpDelete]
        [CodedExceptionHandler]
        public virtual ActionResult Delete(string customer_id)
        {
            adapter.RemoveCustomer(customer_id);
            return new HttpStatusCodeResult(HttpStatusCode.OK, "The customer was deleted.");
        }
    }
}
