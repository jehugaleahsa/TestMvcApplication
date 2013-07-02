using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Adapters;
using Adapters.Models;

namespace TestMvcApplication.Controllers
{
    public class CustomerController : ApiController
    {
        private readonly ICustomerAdapter adapter;

        public CustomerController(ICustomerAdapter adapter)
        {
            if (adapter == null)
            {
                throw new ArgumentNullException("adapter");
            }
            this.adapter = adapter;
        }

        public virtual CustomerData Get(string customerId)
        {
            CustomerData data = adapter.GetCustomer(customerId);
            return data;
        }

        public virtual CustomerData[] GetCustomers()
        {
            CustomerData[] data = adapter.GetCustomers().ToArray();
            return data;
        }

        public virtual HttpResponseMessage Post(CustomerData data)
        {
            CustomerData result = adapter.AddCustomer(data);
            return Request.CreateResponse(HttpStatusCode.Created, result);
        }

        public virtual HttpResponseMessage Put(CustomerData data)
        {
            adapter.UpdateCustomer(data);
            return Request.CreateResponse(HttpStatusCode.OK, "The customer was updated.");
        }

        public virtual HttpResponseMessage Delete(string customerId)
        {
            adapter.RemoveCustomer(customerId);
            return Request.CreateResponse(HttpStatusCode.OK, "The customer was deleted.");
        }
    }
}
