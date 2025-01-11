using System.Collections.Generic;
using EntityLayer.Concrete;

namespace InventoryManagement.Models
{
    public class SaleViewModel
    {
        public List<Product> Products { get; set; }
        public List<Warehouse> Warehouses { get; set; }
        public List<Customer> Customers { get; set; }
        public string NewCustomerName { get; set; } // Yeni müşteri adı
        public string NewCustomerAddress { get; set; } // Yeni müşteri adresi
        public string NewCustomerEmail { get; set; } // Yeni müşteri emaili
    }
}
