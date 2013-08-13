using Adapters.Models;
using ServiceInterfaces.Entities;

namespace Adapters.Mappers
{
    public interface ISettingMapper
    {
        CustomerSetting Convert(SettingData data);

        SettingData Convert(CustomerSetting setting);
    }
}
