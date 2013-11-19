using System;
using System.Collections.Generic;
using System.Linq;
using ServiceInterfaces.Entities;
using ServiceInterfaces.Repositories;
using Policies;

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

        [Log]
        [ErrorMessage("Failed to get the address item.")]
        public AddressItem GetAddressItem(Guid settingId)
        {
            return entities.AddressItems.Where(s => s.AddressItemId == settingId).SingleOrDefault();
        }

        [Log]
        [ErrorMessage("Failed to get the address items.")]
        public IEnumerable<AddressItem> GetAddressItems(Customer customer)
        {
            return entities.AddressItems.Where(s => s.Customer == customer).ToArray();
        }

        [Log]
        [ErrorMessage("Failed to add the address item.")]
        public void Add(AddressItem setting)
        {
            entities.AddressItems.Add(setting);
            entities.SaveChanges();
        }

        [Log]
        [ErrorMessage("Failed to remove the address item.")]
        public void Remove(AddressItem setting)
        {
            entities.AddressItems.Remove(setting);
            entities.SaveChanges();
        }
    }
}
