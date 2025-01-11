using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using InventoryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using System.Linq;

public class SaleController : Controller
{
    private readonly IStockService _stockService;
    private readonly ISaleService _saleService;
    private readonly IProductService _productService;
    private readonly IWarehouseService _warehouseService;
    private readonly ICustomerService _customerService;

    public SaleController(
        IStockService stockService,
        ISaleService saleService,
        IProductService productService,
        IWarehouseService warehouseService,
        ICustomerService customerService)
    {
        _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
        _saleService = saleService ?? throw new ArgumentNullException(nameof(saleService));
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _warehouseService = warehouseService ?? throw new ArgumentNullException(nameof(warehouseService));
        _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
    }

    // Satışları listeleme
    public IActionResult Index()
    {
        var sales = _saleService.TGetList();
        return View(sales);
    }

    // Yeni satış eklemek için AddSale GET metodu
    [HttpGet]
    public IActionResult AddSale()
    {

        ViewBag.Products = new SelectList(_productService.TGetList(), "ProductID", "ProductName");
        ViewBag.Warehouses = new SelectList(_warehouseService.TGetList(), "WarehouseID", "WarehouseName");
        ViewBag.Customers = new SelectList(_customerService.TGetList(), "CustomerID", "CustomerName");
        return View();
    }

    // Yeni satış eklemek için AddSale POST metodu
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddSale(int productId, int warehouseId, int saleQuantity, int? customerId, string customerName, string customerEmail, string customerAddress)
    {
        if (!customerId.HasValue && (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(customerEmail) || string.IsNullOrEmpty(customerAddress)))
        {
            TempData["Error"] = "Müşteri bilgileri eksik!";
            return RedirectToAction("Index", "Sale");
        }

        // Eğer yeni müşteri ekleniyorsa
        if (!customerId.HasValue)
        {
            var newCustomer = new Customer
            {
                CustomerName = customerName,
                Email = customerEmail,
                Address = customerAddress
            };

            _customerService.TAdd(newCustomer);
            customerId = newCustomer.CustomerID;
        }

        // Stok kontrolü
        // Stok bilgisini al
        var stock = _stockService.TGetList().FirstOrDefault(s => s.ProductID == productId && s.WarehouseID == warehouseId);
        if (stock == null || stock.Quantity < saleQuantity)
        {
            TempData["Error"] = "Yetersiz stok!";
            return RedirectToAction("Index","Sale");
        }

        // Stok miktarını güncelle
        stock.Quantity -= saleQuantity;
        _stockService.TUpdate(stock);


        // Satış kaydı oluştur
        var sale = new Sale
        {
            ProductID = productId,
            WarehouseID = warehouseId,
            Quantity = saleQuantity,
            SaleDate = DateTime.Now,
            CustomerID = customerId.Value,

        };

        try
        {
            _saleService.TAdd(sale);
            TempData["Success"] = "Satış işlemi başarılı!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Satış işlemi sırasında bir hata oluştu: {ex.Message}";
        }

        return RedirectToAction("Index", "Sale");
    }

    [HttpPost]
    public IActionResult DeleteSale(int saleId)
    {
        // Satışı bul
        var sale = _saleService.TGetList().FirstOrDefault(s => s.SaleID == saleId);

        if (sale == null)
        {
            TempData["Error"] = "Satış bulunamadı!";
            return RedirectToAction("Index");
        }

        // Stok bilgisini al
        var stock = _stockService.TGetList().FirstOrDefault(s => s.ProductID == sale.ProductID && s.WarehouseID == sale.WarehouseID);
        if (stock != null)
        {
            // Stok miktarını geri ekle
            stock.Quantity += sale.Quantity;
            _stockService.TUpdate(stock);

            try
            {
                // Satış kaydını sil
                _saleService.TDelete(sale);
                TempData["Success"] = "Satış başarıyla silindi!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Satış silinirken bir hata oluştu: {ex.Message}";
            }
        }
        else
        {
            TempData["Error"] = "Stok güncellenemedi!";
        }

        return RedirectToAction("Index","Sale");
    }


    // Dropdown listeleri doldurmak için yardımcı metod
    private void PopulateViewBags()
    {
        ViewBag.Products = new SelectList(_productService.TGetList(), "ProductID", "ProductName");
        ViewBag.Warehouses = new SelectList(_warehouseService.TGetList(), "WarehouseID", "WarehouseName");
        ViewBag.Customers = new SelectList(_customerService.TGetList(), "CustomerID", "CustomerName");
    }

}
