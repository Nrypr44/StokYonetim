using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Text;
using Syncfusion.Pdf;
using Syncfusion.HtmlConverter;
using System.IO;
using iTextSharp.text.pdf;
using Microsoft.OpenPublishing.Build.HtmlConverters.HtmlToPdf;
namespace InventoryManagement.Controllers
{
    public class SalesAnalysisController : Controller
    {
        private readonly ISaleService _saleService;
        private readonly IStockService _stockService;
        private readonly IProductService _productService;
        private readonly IWarehouseService _warehouseService;
        private readonly IServiceProvider _serviceProvider;
        public SalesAnalysisController(ISaleService saleService,IStockService stockService, IProductService productService, IWarehouseService warehouseService, IServiceProvider serviceProvider)
        {
            _saleService = saleService;
            _productService = productService;
            _warehouseService = warehouseService;
            _stockService = stockService;
            _serviceProvider = serviceProvider;
        }
       


        // Depo Bazlı Satış Analizini Göster
        public IActionResult WarehouseSalesAnalysis(string format = null)
        {
            // Depo satış verilerini al
            var warehouseSales = _saleService.GetWarehouseSalesAnalysis();

            // Depo adlarını almak için WarehouseService'i kullan
            var warehouseNames = _warehouseService.TGetList() // Tüm depoları al
                                       .ToDictionary(w => w.WarehouseID, w => w.WarehouseName); // ID ile adları ilişkilendir

            // Depo satış verisi ile depo adlarını ilişkilendir
            foreach (var sale in warehouseSales)
            {
                sale.WarehouseName = warehouseNames.ContainsKey(sale.WarehouseID) ? warehouseNames[sale.WarehouseID] : "Unknown Warehouse";
            }
            if (format == "PDF")
            {
                return new ViewAsPdf("WarehouseSalesAnalysis", warehouseSales)
                {
                    FileName = "WarehouseSalesAnalysis.pdf"
                };
            }
            return View(warehouseSales);

           
        }

        public IActionResult ProductSalesAnalysis()
        {
            // Satış verilerini al

                var productSales = _saleService.GetProductSalesAnalysis(); // Ürün satış verisi
                var categorySales = _saleService.GetCategorySalesAnalysis(); // Kategori satış verisi
            var productNames = _productService.TGetList() // Ürünleri al
                                    .ToDictionary(p => p.ProductID, p => p.ProductName); // ID ile adları ilişkilendir

            // Satış verisi ile ürün adlarını ilişkilendir
            foreach (var sale in productSales)
            {
                sale.ProductName = productNames.ContainsKey(sale.ProductID) ? productNames[sale.ProductID] : "Unknown Product";
            }

            var viewModel = Tuple.Create(productSales, categorySales); // İki listeyi birleştir
                return View(viewModel); // View'a gönder


        }



        // a) Depo Bazlı Stok ve Satış Durumu - Pasta Grafiği
        public IActionResult WarehouseStockAnalysis()
        {
            var warehouseStockData = _stockService.GetWarehouseStockDistribution();

            ViewBag.WarehouseStockData = warehouseStockData.Select(w => new
            {
                WarehouseName = w.WarehouseName,
                TotalStock = w.TotalStock
            }).ToList();

            return View();
        }


        public IActionResult TopSellingProducts()
        {
            // En çok satan 10 ürünü getir
            var topSellingProducts = _saleService.GetTopSellingProducts();

            // Ürün adlarını al ve eşleştir
            var productNames = _productService.TGetList()
                                    .ToDictionary(p => p.ProductID, p => p.ProductName);

            foreach (var sale in topSellingProducts)
            {
                sale.ProductName = productNames.ContainsKey(sale.ProductID) ? productNames[sale.ProductID] : "Unknown Product";
            }

            return View(topSellingProducts);
        }

        public IActionResult CategorySalesAnalysis()
        {
            var categorySales = _saleService.GetCategorySalesAnalysis();

            if (categorySales == null || !categorySales.Any())
            {
                ViewBag.Message = "No category sales data available.";
            }

            return View(categorySales);
        }



    }
}
