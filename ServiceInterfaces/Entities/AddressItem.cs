using System;

namespace ServiceInterfaces.Entities
{
    public class AddressItem
    {
        public Guid AddressItemId { get; set; }

        public Guid CustomerId { get; set; }

        public string Key { get; set; }

        public int Order { get; set; }

        public string Value { get; set; }

        public Customer Customer { get; set; }
    }
}
