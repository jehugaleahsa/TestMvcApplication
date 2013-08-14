using Adapters.Models;
using ServiceInterfaces.Entities;

namespace Adapters.Mappers
{
    public interface IAddressItemMapper
    {
        AddressItem Convert(AddressItemData data);

        AddressItemData Convert(AddressItem setting);
    }
}
