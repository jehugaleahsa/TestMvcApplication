﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using Adapters.Adapters;
using Adapters.Models;
using TestMvcApplication.ActionResults;
using TestMvcApplication.FilterAttributes;

namespace TestMvcApplication.Controllers
{
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

        [HttpStatusCodeErrorHandler]
        public virtual ActionResult Load()
        {
            IEnumerable<CustomerData> data = adapter.GetCustomers();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HttpStatusCodeErrorHandler]
        public virtual ActionResult Create(CustomerData data)
        {
            CustomerData result = adapter.AddCustomer(data);
            return new JsonWithHttpStatusCodeResult(HttpStatusCode.Created) { Data = result };
        }

        [HttpPut]
        [HttpStatusCodeErrorHandler]
        public virtual ActionResult Edit(CustomerData data)
        {
            adapter.UpdateCustomer(data);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent, "The customer was updated.");
        }

        [HttpDelete]
        [HttpStatusCodeErrorHandler]
        public virtual ActionResult Delete(string customer_id)
        {
            adapter.RemoveCustomer(customer_id);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent, "The customer was deleted.");
        }
    }
}
