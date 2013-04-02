using Adapters.Models;
using ServiceInterfaces.Entities;

namespace Adapters.Mappers
{
    public interface ICustomerMapper
    {
        Customer Convert(CustomerData data);
        
        CustomerData Convert(Customer customer);
    }
}
