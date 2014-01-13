using System;
using System.Collections.Generic;
using System.Linq;
using DataObjects;
using Policies;
using ServiceInterfaces;

namespace DataModeling
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
        public AddressItem GetAddressItem(Guid itemId)
        {
            return entities.AddressItems.Where(s => s.AddressItemId == itemId).SingleOrDefault();
        }

        [Log]
        [ErrorMessage("Failed to get the address items.")]
        public IEnumerable<AddressItem> GetAddressItems(Customer customer)
        {
            return entities.AddressItems.Where(s => s.Customer == customer).ToArray();
        }

        [Log]
        [ErrorMessage("Failed to add the address item.")]
        public void Add(AddressItem item)
        {
            entities.AddressItems.Add(item);
            entities.SaveChanges();
        }

        [Log]
        [ErrorMessage("Failed to remove the address item.")]
        public void Remove(AddressItem item)
        {
            entities.AddressItems.Remove(item);
            entities.SaveChanges();
        }
    }
}
