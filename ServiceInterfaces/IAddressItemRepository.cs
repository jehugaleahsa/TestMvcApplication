using System;
using System.Collections.Generic;
using DataObjects;

namespace ServiceInterfaces
{
    public interface IAddressItemRepository
    {
        AddressItem GetAddressItem(Guid itemId);

        IEnumerable<AddressItem> GetAddressItems(Customer customer);

        void Add(AddressItem item);

        void Remove(AddressItem item);
    }
}
