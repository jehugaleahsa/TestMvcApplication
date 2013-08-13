﻿using System;
using System.Collections.Generic;

namespace ServiceInterfaces.Entities
{
    public class Customer
    {
        public Guid CustomerId { get; set; }

        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        public int Height { get; set; }

        public ICollection<CustomerSetting> Settings { get; private set; }
    }
}
