using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Adapters;
using MvcUtilities.FilterAttributes;
using ViewModels;

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

        [WebApiExceptionHandler]
        public HttpResponseMessage Get(HttpRequestMessage request, string id)
        {
            CustomerData data = adapter.GetCustomer(id);
            return request.CreateResponse(HttpStatusCode.OK, data);
        }

        [WebApiExceptionHandler]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            CustomerData[] data = adapter.GetCustomers().ToArray();
            return request.CreateResponse(HttpStatusCode.OK, data);
        }

        [ValidateModel]
        [WebApiExceptionHandler]
        public HttpResponseMessage Post(HttpRequestMessage request, CustomerData data)
        {
            CustomerData result = adapter.AddCustomer(data);
            return request.CreateResponse(HttpStatusCode.Created, result);
        }

        [ValidateModel]
        [WebApiExceptionHandler]
        public HttpResponseMessage Put(HttpRequestMessage request, CustomerData data)
        {
            adapter.UpdateCustomer(data);
            return request.CreateResponse(HttpStatusCode.OK, "The customer was updated.");
        }

        [WebApiExceptionHandler]
        public HttpResponseMessage Delete(HttpRequestMessage request, string id)
        {
            adapter.RemoveCustomer(id);
            return request.CreateResponse(HttpStatusCode.OK, "The customer was deleted.");
        }
    }
}
