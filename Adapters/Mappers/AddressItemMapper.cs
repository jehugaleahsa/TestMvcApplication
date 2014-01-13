using System;
using ViewModels;
using DataObjects;

namespace Adapters.Mappers
{
    public interface IAddressItemMapper
    {
        AddressItem Convert(AddressItemData data);

        AddressItemData Convert(AddressItem item);
    }

    public class AddressItemMapper : IAddressItemMapper
    {
        public AddressItem Convert(AddressItemData data)
        {
            if (data == null)
            {
                return null;
            }
            PrimitiveMapper mapper = new PrimitiveMapper();
            AddressItem item = new AddressItem();
            if (!String.IsNullOrWhiteSpace(data.AddressItemId))
            {
                item.AddressItemId = mapper.ToGuid(data.AddressItemId);
            }
            item.CustomerId = mapper.ToGuid(data.CustomerId);
            item.Key = data.Key;
            item.Value = data.Key;
            return item;
        }

        public AddressItemData Convert(AddressItem item)
        {
            if (item == null)
            {
                return null;
            }
            PrimitiveMapper mapper = new PrimitiveMapper();
            AddressItemData data = new AddressItemData();
            data.AddressItemId = mapper.ToString(item.AddressItemId);
            data.CustomerId = mapper.ToString(item.CustomerId);
            data.Key = item.Key;
            data.Value = item.Value;
            return data;
        }
    }
}
