using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Stock
    {
        public int StockID { get; set; }            
        public int ProductID { get; set; }          
        public int WarehouseID { get; set; }        
        public int Quantity { get; set; }
        public string StockMovementType { get; set; }
        public DateTime Date { get; set; }

        
        public Product Product { get; set; }
        public Warehouse Warehouse { get; set; }

        public ICollection<Sale> Sales { get; set; }

    }
}
