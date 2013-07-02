using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Adapters;
using Adapters.Models;

namespace TestMvcApplication.Controllers
{
    public class CustomersController : ApiController
    {
        private readonly ICustomerAdapter adapter;

        public CustomersController(ICustomerAdapter adapter)
        {
            if (adapter == null)
            {
                throw new ArgumentNullException("adapter");
            }
            this.adapter = adapter;
        }

        public CustomerData Get(string id)
        {
            CustomerData data = adapter.GetCustomer(id);
            return data;
        }

        public CustomerData[] GetAll()
        {
            CustomerData[] data = adapter.GetCustomers().ToArray();
            return data;
        }

        public HttpResponseMessage Post(CustomerData data)
        {
            CustomerData result = adapter.AddCustomer(data);
            return Request.CreateResponse(HttpStatusCode.Created, result);
        }

        public HttpResponseMessage Put(CustomerData data)
        {
            adapter.UpdateCustomer(data);
            return Request.CreateResponse(HttpStatusCode.OK, "The customer was updated.");
        }

        public HttpResponseMessage Delete(string id)
        {
            adapter.RemoveCustomer(id);
            return Request.CreateResponse(HttpStatusCode.OK, "The customer was deleted.");
        }
    }
}
