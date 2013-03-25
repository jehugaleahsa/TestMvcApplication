using System;

namespace ServiceInterfaces.Entities
{
    public class Customer
    {
        public Guid CustomerId { get; set; }

        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        public int Height { get; set; }
    }
}
