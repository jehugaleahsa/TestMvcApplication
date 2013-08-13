using System;
using System.Collections.Generic;
using ServiceInterfaces.Entities;

namespace ServiceInterfaces.Repositories
{
    public interface ICustomerSettingRepository
    {
        CustomerSetting GetSetting(Guid guid);

        IEnumerable<CustomerSetting> GetSettings(Customer customer);

        void Add(CustomerSetting setting);

        void Remove(CustomerSetting setting);
    }
}
