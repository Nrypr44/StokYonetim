using EntityLayer.Concrete;

namespace DataAccessLayer.Abstract
{
    public interface ISaleDal : IGenericDal<Sale>
    {
        
        public List<Sale> GetSales();
        public IEnumerable<Sale> GetSalesByProductId(int productId);


    }
}
