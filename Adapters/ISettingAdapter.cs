using System.Collections.Generic;
using Adapters.Models;

namespace Adapters
{
    public interface ISettingAdapter
    {
        SettingData AddSetting(SettingData data);

        IEnumerable<SettingData> GetSettings(string customerId);

        void RemoveSetting(string settingId);
    }
}
