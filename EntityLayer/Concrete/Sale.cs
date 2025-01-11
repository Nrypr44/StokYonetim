using System;

namespace EntityLayer.Concrete
{
    public class Sale
    {
        public int SaleID { get; set; }          
        public int ProductID { get; set; }       
        public int WarehouseID { get; set; }

                                                 
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }
        public int CustomerID { get; set; }      

        
        public Product Product { get; set; }
        public Warehouse Warehouse { get; set; }
        public Customer Customer { get; set; }    
    }
}
