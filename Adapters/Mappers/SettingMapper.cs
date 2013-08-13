using System;
using Adapters.Models;
using ServiceInterfaces.Entities;

namespace Adapters.Mappers
{
    public class SettingMapper : ISettingMapper
    {
        public CustomerSetting Convert(SettingData data)
        {
            PrimitiveMapper mapper = new PrimitiveMapper();
            CustomerSetting setting = new CustomerSetting();
            if (!String.IsNullOrWhiteSpace(data.SettingId))
            {
                setting.CustomerSettingId = mapper.ToGuid(data.SettingId);
            }
            setting.CustomerId = mapper.ToGuid(data.CustomerId);
            setting.Key = data.Key;
            setting.Value = data.Key;
            return setting;
        }

        public SettingData Convert(CustomerSetting setting)
        {
            PrimitiveMapper mapper = new PrimitiveMapper();
            SettingData data = new SettingData();
            data.SettingId = mapper.ToString(setting.CustomerSettingId);
            data.CustomerId = mapper.ToString(setting.CustomerId);
            data.Key = setting.Key;
            data.Value = setting.Value;
            return data;
        }
    }
}
