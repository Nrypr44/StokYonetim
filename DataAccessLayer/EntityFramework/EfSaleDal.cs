using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Concrete.EntityFramework
{
    public class EfSaleDal : GenericDal<Sale>, ISaleDal
    {
        private readonly MyDepoContext _context;

        public EfSaleDal(MyDepoContext context) : base(context)
        {
            _context = context;
        }
        public IEnumerable<Sale> GetSalesByProductId(int productId)
        {
            return _context.Sales.Where(s => s.ProductID == productId).ToList();
        }
        public List<Sale> GetSales()
        {
            var sales = _context.Sales
                                .Include(s => s.Product)
                                .Include(s => s.Warehouse)
                                .Include(s => s.Customer)
                                .ToList();
            return sales;
        }
    }
}
