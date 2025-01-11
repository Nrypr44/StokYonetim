using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Warehouse
    {
        public int WarehouseID { get; set; } 
        public string WarehouseName { get; set; }
        public string Location { get; set; }
        public string ManagerName { get; set; }
        public string ManagerEmail { get; set; }
        public int Capacity { get; set; }

        
        public ICollection<Stock> Stocks { get; set; }
        public ICollection<Sale> Sales { get; set; }

    }
}