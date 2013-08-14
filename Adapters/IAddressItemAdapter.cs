using System.Collections.Generic;
using Adapters.Models;

namespace Adapters
{
    public interface IAddressItemAdapter
    {
        AddressItemData AddAddressItem(AddressItemData data);

        IEnumerable<AddressItemData> GetAddressItems(string customerId);

        void RemoveAddressItem(string settingId);
    }
}
