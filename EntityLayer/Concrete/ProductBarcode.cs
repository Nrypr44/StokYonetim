using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class ProductBarcode
    {
        [Key]
        public int BarcodeID { get; set; }
        public int ProductID { get; set; }
        public string Barcode { get; set; }

        public virtual Product Product { get; set; }
    }
}
