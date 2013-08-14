using System;
using System.Collections.Generic;
using System.Linq;
using ServiceInterfaces.Entities;
using ServiceInterfaces.Repositories;

namespace DataModeling.Repositories
{
    public class AddressItemRepository : IAddressItemRepository
    {
        private readonly EntitySet entities;

        public AddressItemRepository(EntitySet entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            this.entities = entities;
        }

        public AddressItem GetAddressItem(Guid settingId)
        {
            return entities.AddressItems.Where(s => s.AddressItemId == settingId).SingleOrDefault();
        }

        public IEnumerable<AddressItem> GetAddressItems(Customer customer)
        {
            return entities.AddressItems.Where(s => s.Customer == customer).ToArray();
        }

        public void Add(AddressItem setting)
        {
            entities.AddressItems.Add(setting);
            entities.SaveChanges();
        }

        public void Remove(AddressItem setting)
        {
            entities.AddressItems.Remove(setting);
            entities.SaveChanges();
        }
    }
}
