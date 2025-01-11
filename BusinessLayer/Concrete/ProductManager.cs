using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BusinessLayer.Concrete
{
    public class ProductManager : IProductService
    {
        private readonly IProductDal _productDal;

        public ProductManager(IProductDal productDal)
        {
            _productDal = productDal;
        }

        public void TAdd(Product product)
        {
            try
            {
                _productDal.Add(product);
               
               
                Console.WriteLine("Ürün başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ürün eklerken karşılaşılan hata : {ex.Message}");
            }
        }

        public void TDelete(Product product)
        {
            try
            {
                _productDal.Delete(product);
                Console.WriteLine($"Ürün ID'si {product.ProductID} olan ürün silindi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ürünü silerken karşılaşılan hata: {ex.Message}");
            }
        }


        public List<Product> TGetList()
        {
            return _productDal.GetAll();
        }

        public void TUpdate(Product product)
        {
            _productDal.Update(product);
        }

        public Product TGetByID(int id)
        {
            return _productDal.GetById(id);
        }

        public List<Product> TGetListWithBarcodes()
        {
                return _productDal.GetAllWithBarcodes(); 
            
        }

        public Product TGetByIDWithBarcodes(int id)
        {
            return _productDal.GetQueryable()
                .Include(p => p.Barcodes)  
                .FirstOrDefault(p => p.ProductID == id); 
        }

        public Product TGetByBarcode(string barcode)
        {
            return _productDal.GetQueryable()
                .Include(p => p.Barcodes)
                .FirstOrDefault(p => p.Barcodes.Any(b => b.Barcode == barcode));  
        }

        public Product GetBySKU(string sku)
        {
            return _productDal.GetQueryable()
                .Include(p => p.Barcodes)
                .FirstOrDefault(p => p.SKU == sku);
        }

     

    }
}
