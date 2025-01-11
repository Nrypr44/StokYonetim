using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IStockDal:IGenericDal<Stock>
    {
        public List<Stock> GetListWithDetails();
        public Stock GetByIDWithProduct(int id);
        Stock GetByProductId(int productId);


    }
}
