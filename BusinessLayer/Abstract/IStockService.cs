using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IStockService: IGenericService<Stock>
    {
        List<Stock> TGetListWithDetails();
        Stock TGetByIDWithDetails(int id);
        public Stock GetStockByIDWithProduct(int id);

        public Stock GetStockByProductAndWarehouse(int productId, int warehouseId);

        List<(string WarehouseName, int TotalStock)> GetWarehouseStockDistribution();



    }
}
