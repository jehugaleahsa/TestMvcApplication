using System;
using System.Collections.Generic;
using ServiceInterfaces.Entities;

namespace ServiceInterfaces.Repositories
{
    public interface IAddressItemRepository
    {
        AddressItem GetAddressItem(Guid guid);

        IEnumerable<AddressItem> GetAddressItems(Customer customer);

        void Add(AddressItem item);

        void Remove(AddressItem item);
    }
}
