using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Adapters.Mappers;
using Adapters.Models;
using Policies;
using ServiceInterfaces.Entities;
using ServiceInterfaces.Repositories;

namespace Adapters
{
    public class SettingAdapter : ISettingAdapter
    {
        private readonly ICustomerRepository customerRepository;
        private readonly ICustomerSettingRepository settingRepository;

        public SettingAdapter(
            ICustomerRepository customerRepository, 
            ICustomerSettingRepository settingRepository)
        {
            if (customerRepository == null)
            {
                throw new ArgumentNullException("customerRepository");
            }
            if (settingRepository == null)
            {
                throw new ArgumentNullException("settingRepository");
            }
            this.customerRepository = customerRepository;
            this.settingRepository = settingRepository;
            SettingMapper = new SettingMapper();
        }

        public SettingMapper SettingMapper { get; set; }

        [Log]
        [ErrorMessage("An error occurred while adding the setting.")]
        public SettingData AddSetting(SettingData data)
        {
            CustomerSetting setting = SettingMapper.Convert(data);
            settingRepository.Add(setting);
            return SettingMapper.Convert(setting);
        }

        [Log]
        [ErrorMessage("An error occurred while retrieving the customer's settings.")]
        public IEnumerable<SettingData> GetSettings(string customerId)
        {
            PrimitiveMapper mapper = new PrimitiveMapper();
            Customer customer = customerRepository.GetCustomer(mapper.ToGuid(customerId));
            if (customer == null)
            {
                throw new AdapterException(HttpStatusCode.NotFound, "A customer with the given ID was not found.");
            }
            return settingRepository.GetSettings(customer).Select(c => SettingMapper.Convert(c)).ToArray();
        }

        [Log]
        [ErrorMessage("An error occurred while removing the customer.")]
        public void RemoveSetting(string settingId)
        {
            PrimitiveMapper mapper = new PrimitiveMapper();
            CustomerSetting setting = settingRepository.GetSetting(mapper.ToGuid(settingId));
            if (setting == null)
            {
                throw new AdapterException(HttpStatusCode.NotFound, "A setting with the given ID was not found.");
            }
            settingRepository.Remove(setting);
        }
    }
}
