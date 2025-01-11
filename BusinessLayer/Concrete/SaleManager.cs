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
    public class SaleManager : ISaleService
    {
        private readonly ISaleDal _saleDal;
        private readonly IStockService _stockService; 

        public SaleManager(ISaleDal saleDal, IStockService stockService)
        {
            _saleDal = saleDal;
            _stockService = stockService; 
        }
        public void TAdd(Sale sale)
        {
                _saleDal.Add(sale);
        }


        public void TUpdate(Sale sale)
        {
            _saleDal.Update(sale);
        }

        public void TDelete(Sale sale)
        {
            _saleDal.Delete(sale);
        }

        public Sale TGetByID(int id)
        {
            return _saleDal.GetById(id);
        }

        public List<Sale> TGetList()
        {
            return _saleDal.GetSales();
        }

        public List<WarehouseSalesAnalysis> GetWarehouseSalesAnalysis()
        {
            var result = _saleDal.GetSales()
                .GroupBy(s => s.WarehouseID)
                .Select(g => new WarehouseSalesAnalysis
                {
                    WarehouseID = g.Key,
                    TotalSales = g.Sum(s => s.Quantity),
                    TotalStock = _stockService.TGetList()
                                .Where(stock => stock.WarehouseID == g.Key)
                                .Sum(stock => stock.Quantity)
                })
                .ToList();

            return result;
        }

       



        public List<ProductSalesAnalysis> GetProductSalesAnalysis()
        {
            var salesData = _saleDal.GetSales()
                .Where(s => s.Product != null) 
                .GroupBy(s => s.ProductID)
                .Select(g => new ProductSalesAnalysis
                {
                    ProductID = g.Key,
                    TotalSales = g.Sum(s => s.Quantity)
                })
                .ToList();

            if (!salesData.Any())
            {
                Console.WriteLine("Ürün satış analizi için satış bilgisi bulunamamıştır.");
            }

            return salesData;
        }


        public List<ProductSalesAnalysis> GetTopSellingProducts()
        {
            var topSellingProducts = _saleDal.GetSales()
                .Where(s => s.Product != null)
                .GroupBy(s => s.ProductID)
                .Select(g => new ProductSalesAnalysis
                {
                    ProductID = g.Key,
                    TotalSales = g.Sum(s => s.Quantity)
                })
                .OrderByDescending(p => p.TotalSales)
                .Take(10)
                .ToList();

            return topSellingProducts;
        }

        public List<CategorySalesAnalysis> GetCategorySalesAnalysis()
        {
            var categorySales = _saleDal.GetSales()
                .Where(s => !string.IsNullOrEmpty(s.Product?.Category)) 
                .GroupBy(s => s.Product.Category) 
                .Select(g => new CategorySalesAnalysis
                {
                    CategoryID = 0, 
                    CategoryName = g.Key ?? "Bilinmeyen", 
                    TotalSales = g.Sum(s => s.Quantity) 
                })
                .ToList();

            return categorySales;
        }


    }
}
