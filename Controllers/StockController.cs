using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Concrete;

namespace InventoryManagement.Controllers
{
    public class StockController : Controller
    {
        private readonly IStockService _stockService;
        private readonly IProductService _productService;
        private readonly IWarehouseService _warehouseService;

        public StockController(IStockService stockService, IProductService productService, IWarehouseService warehouseService)
        {
            _stockService = stockService;
            _productService = productService;
            _warehouseService = warehouseService;
        }

     


        public IActionResult Index()
        {
            // Stokları ilgili ürün ve depo detayları ile birlikte getiriyoruz
            var stocks = _stockService.TGetListWithDetails();
            return View(stocks); // Index.cshtml'e gönderiyoruz
        }

        [HttpGet]
        public IActionResult AddStock()
        {
            var products = _productService.TGetList(); // Ürünleri al
            var warehouses = _warehouseService.TGetList(); // Depoları al

            // View'da kullanılacak ürünler ve depoları gönderiyoruz
            ViewBag.Products = new SelectList(products, "ProductID", "ProductName");
            ViewBag.Warehouses = new SelectList(warehouses, "WarehouseID", "WarehouseName");

            return View();
        }

        [HttpPost]
        public IActionResult AddStock(Stock stock, int productId, int warehouseId)
        {
            ModelState.Remove("Product"); // Eğer stok eklerken ModelState'de bir hata varsa, ürünü temizliyoruz
            ModelState.Remove("Warehouse");
            ModelState.Remove("Sales");

            if (ModelState.IsValid)
            {
                var product = _productService.TGetByID(productId); // Ürünü alıyoruz
                var warehouse = _warehouseService.TGetByID(warehouseId); // Depoyu alıyoruz

                if (product != null && warehouse != null)
                {
                    stock.Product = product; // Ürünü stok ile ilişkilendiriyoruz
                    stock.Warehouse = warehouse; // Depoyu stok ile ilişkilendiriyoruz

                    _stockService.TAdd(stock); // Stok ekleme işlemini gerçekleştiriyoruz


                    return RedirectToAction("Index", "Stock");
                }
                else
                {
                    // Eğer ürün veya depo bulunamadıysa hata mesajı ekliyoruz
                    ModelState.AddModelError(string.Empty, "Invalid product or warehouse.");
                }
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is not valid:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
                return View(stock); // Eğer model geçerli değilse, hatalarla birlikte formu geri göster
            }

            // Hatalı durumdaki view'a tekrar yönlendiriyoruz
            return View(stock);
        }
        [HttpGet]
        public IActionResult UpdateStock(int id)
        {
            // Stok bilgilerini al
            var stock = _stockService.TGetByID(id);

            // Depoları al
            var warehouses = _warehouseService.TGetList(); // Warehouse servisini kullanarak depoları al
            var products = _productService.TGetList(); // Ürünleri al

            // Depoları radio button formatına dönüştür


            // Ürünleri SelectList formatında gönder
            ViewBag.Warehouses = new SelectList(warehouses, "WarehouseID", "WarehouseName");

            // Model olarak mevcut stok bilgilerini gönder
            return View(stock); // Index view'ını stok listesi modeli ile döndür
        }


        [HttpPost]
        public IActionResult UpdateStock(int id, Stock updatedStock)
        {
            ModelState.Remove("Product");
            ModelState.Remove("Warehouse");

            var existingStock = _stockService.TGetByID(id);
            if (existingStock == null)
            {
                return NotFound();
            }

            if (existingStock.WarehouseID != updatedStock.WarehouseID)
            {
                var warehouse = _warehouseService.TGetByID(updatedStock.WarehouseID);
                if (warehouse != null)
                {
                    existingStock.Warehouse = warehouse;
                }
            }

            existingStock.Quantity = updatedStock.Quantity;
            existingStock.Date = updatedStock.Date;
            existingStock.StockMovementType = updatedStock.StockMovementType;

            _stockService.TUpdate(existingStock);


            return RedirectToAction("Index", "Stock");
        }

        [HttpPost]
        public IActionResult DeleteStock(int id)
        {
            var stock = _stockService.TGetByID(id);
            if (stock == null)
            {
                return NotFound();
            }

            _stockService.TDelete(stock);

            return RedirectToAction("Index", "Stock");
        }


    }
}