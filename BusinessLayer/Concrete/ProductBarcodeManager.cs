using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    namespace BusinessLayer.Concrete
    {
        public class ProductBarcodeManager : IProductBarcodeService
        {
            private readonly IProductBarcodeDal _productBarcodeDal;

            public ProductBarcodeManager(IProductBarcodeDal productBarcodeDal)
            {
                _productBarcodeDal = productBarcodeDal;
            }

            public void TAdd(ProductBarcode productBarcode)
            {
                if (productBarcode == null)
                    throw new ArgumentNullException(nameof(productBarcode));

                _productBarcodeDal.Add(productBarcode);
            }

            public void TDelete(ProductBarcode productBarcode)
            {
                if (productBarcode == null)
                    throw new ArgumentNullException(nameof(productBarcode));

                _productBarcodeDal.Delete(productBarcode);
            }

            public void TUpdate(ProductBarcode productBarcode)
            {
                if (productBarcode == null)
                    throw new ArgumentNullException(nameof(productBarcode));

                _productBarcodeDal.Update(productBarcode);
            }

            public ProductBarcode TGetByID(int id)
            {
                if (id <= 0)
                    throw new ArgumentException("Uyumsuz  ID");

                return _productBarcodeDal.GetById(id);
            }

            public List<ProductBarcode> TGetList()
            {
                return _productBarcodeDal.GetAll();
            }

       
        }
    }

}
