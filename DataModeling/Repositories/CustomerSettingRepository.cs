using System;
using System.Collections.Generic;
using System.Linq;
using DataModeling.DataModel;
using ServiceInterfaces.Entities;
using ServiceInterfaces.Repositories;

namespace DataModeling.Repositories
{
    public class CustomerSettingRepository : ICustomerSettingRepository
    {
        private readonly EntitySet entities;

        public CustomerSettingRepository(EntitySet entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            this.entities = entities;
        }

        public CustomerSetting GetSetting(Guid settingId)
        {
            return entities.Settings.Where(s => s.CustomerSettingId == settingId).SingleOrDefault();
        }

        public IEnumerable<CustomerSetting> GetSettings(Customer customer)
        {
            return entities.Settings.Where(s => s.Customer == customer).ToArray();
        }

        public void Add(CustomerSetting setting)
        {
            entities.Settings.Add(setting);
            entities.SaveChanges();
        }

        public void Remove(CustomerSetting setting)
        {
            entities.Settings.Remove(setting);
            entities.SaveChanges();
        }
    }
}
