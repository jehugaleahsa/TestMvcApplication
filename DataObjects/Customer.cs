using System;
using System.Collections.Generic;

namespace DataObjects
{
    public class Customer
    {
        public Guid CustomerId { get; set; }

        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        public int Height { get; set; }

        public ICollection<AddressItem> Settings { get; private set; }
    }
}
