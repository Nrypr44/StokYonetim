using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.EntityFramework
{
    public class EfStockDal:GenericDal<Stock>,IStockDal
    {
        private readonly MyDepoContext _context;

        public EfStockDal(MyDepoContext context) : base(context)
        {
            _context = context;
        }
        public List<Stock> GetListWithDetails()
        {
            return _context.Set<Stock>()
                           .Include(s => s.Product)  
                           .Include(s => s.Warehouse) 
                           .ToList();
        }

        public Stock GetByIDWithProduct(int id)
        {
            
                return _context.Stocks
                              .Include(s => s.Product)
                              .FirstOrDefault(s => s.StockID == id);
            
        }

        public Stock GetByProductId(int productId)
        {
            return _context.Stocks.FirstOrDefault(s => s.ProductID == productId);
        }

    }
}
