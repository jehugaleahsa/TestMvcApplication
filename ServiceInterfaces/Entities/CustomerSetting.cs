using System;

namespace ServiceInterfaces.Entities
{
    public class CustomerSetting
    {
        public Guid CustomerSettingId { get; set; }

        public Customer Customer { get; set; }

        public Guid CustomerId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
