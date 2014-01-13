using System;
using System.Linq;
using System.Web.Mvc;
using Adapters;
using MvcUtilities.FilterAttributes;
using ViewModels;

namespace TestMvcApplication.Controllers
{
    [Trace]
    [RedirectOnError(ReturnController=ClassicController.NameConst, ReturnAction=ClassicController.ActionNameConstants.Index)]
    public partial class ClassicController : Controller
    {
        private readonly ICustomerAdapter adapter;

        public ClassicController(ICustomerAdapter adapter)
        {
            if (adapter == null)
            {
                throw new ArgumentNullException("adapter");
            }
            this.adapter = adapter;
        }

        public virtual ActionResult Index()
        {
            CustomerData[] customers = adapter.GetCustomers().OrderBy(customer => customer.Name).ToArray();
            return View(Views.Index, customers);
        }

        public virtual ActionResult Create()
        {
            return View(Views.Create);
        }

        [HttpPost]
        public virtual ActionResult Create(CustomerData data)
        {
            if (!ModelState.IsValid)
            {
                return View(Views.Create, data);
            }
            adapter.AddCustomer(data);
            return RedirectToAction(MVC.Classic.Index());
        }

        public virtual ActionResult Edit(string customer_id)
        {
            CustomerData customer = adapter.GetCustomer(customer_id);
            return View(Views.Edit, customer);
        }

        [HttpPost]
        public virtual ActionResult Edit(CustomerData data)
        {
            if (!ModelState.IsValid)
            {
                return View(Views.Edit, data);
            }
            adapter.UpdateCustomer(data);
            return RedirectToAction(MVC.Classic.Index());
        }

        public virtual ActionResult Delete(string customer_id)
        {
            CustomerData customer = adapter.GetCustomer(customer_id);
            return View(Views.Delete, customer);
        }

        [HttpPost]
        public virtual ActionResult Delete(CustomerData customerData)
        {
            adapter.RemoveCustomer(customerData.CustomerId);
            return RedirectToAction(MVC.Classic.Index());
        }
    }
}
