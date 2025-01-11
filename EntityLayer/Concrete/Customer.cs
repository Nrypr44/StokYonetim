using System;
using System.Collections.Generic;

namespace EntityLayer.Concrete
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }

        public List<Sale> Sales { get; set; }
    }
}
