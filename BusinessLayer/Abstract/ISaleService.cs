using BusinessLayer.Concrete;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface ISaleService:IGenericService<Sale>
    {
        public List<WarehouseSalesAnalysis> GetWarehouseSalesAnalysis();
        public List<ProductSalesAnalysis> GetProductSalesAnalysis();
        List<ProductSalesAnalysis> GetTopSellingProducts(); 
        public List<CategorySalesAnalysis> GetCategorySalesAnalysis();




    }
}
