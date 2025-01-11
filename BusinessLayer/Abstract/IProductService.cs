using BusinessLayer.Concrete;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IProductService: IGenericService<Product>
    {
        public List<Product> TGetListWithBarcodes();
        public Product TGetByIDWithBarcodes(int id);
        public Product GetBySKU(string sku);

        public Product TGetByBarcode(string barcode);



    }
}
