using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.Areas.member.Controllers
{
    [Area("member")]
    [Route("member/[controller]/[action]/{id?}")]

    public class UserSaleController : Controller
    {
        private readonly IStockService _stockService;
        private readonly ISaleService _saleService;
        private readonly IProductService _productService;
        private readonly IWarehouseService _warehouseService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserSaleController(
            IStockService stockService,
            ISaleService saleService,
            IProductService productService,
            IWarehouseService warehouseService,
            ICustomerService customerService,
            UserManager<ApplicationUser> userManager
            )
        {
            _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
            _saleService = saleService ?? throw new ArgumentNullException(nameof(saleService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _warehouseService = warehouseService ?? throw new ArgumentNullException(nameof(warehouseService));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<IActionResult> Index()
        {
            // Kullanıcıyı alıyoruz
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["Error"] = "Kullanıcı bilgileri alınamadı.";
                return RedirectToAction("Index", "Home");
            }

            // Kullanıcının yaptığı satışları filtreliyoruz
            var sales = _saleService.TGetList()
                .Where(s => s.Customer.Email == user.Email)  // Kullanıcının emailine göre filtrele
                .ToList();

            return View(sales);
        }


        // Yeni satış eklemek için AddSale GET metodu
        [HttpGet]
        public async Task<IActionResult> AddSale()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Kullanıcı bilgileri alınamadı.";
                return RedirectToAction("Index");
            }

            // Dropdown verileri ve kullanıcı bilgileri
            ViewBag.Products = new SelectList(_productService.TGetList(), "ProductID", "ProductName");
            ViewBag.Warehouses = new SelectList(_warehouseService.TGetList(), "WarehouseID", "WarehouseName");
            ViewBag.UserName = user.UserName;
            ViewBag.UserEmail = user.Email;

            return View();
        }

        // Yeni satış eklemek için AddSale POST metodu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSale(int productId, int warehouseId, int saleQuantity, string customerAddress)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Kullanıcı bilgileri alınamadı.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(customerAddress))
            {
                TempData["Error"] = "Adres alanı boş bırakılamaz.";
                return RedirectToAction("AddSale");
            }

            // Kullanıcının müşteri olarak eklenmesi
            var customer = _customerService.TGetList()
                .FirstOrDefault(c => c.Email == user.Email);

            if (customer == null)
            {
                customer = new Customer
                {
                    CustomerName = user.UserName,
                    Email = user.Email,
                    Address = customerAddress
                };
                _customerService.TAdd(customer);
            }
            else
            {
                customer.Address = customerAddress;
                _customerService.TUpdate(customer);
            }

            // Stok kontrolü
            var stock = _stockService.TGetList()
                .FirstOrDefault(s => s.ProductID == productId && s.WarehouseID == warehouseId);

            if (stock == null || stock.Quantity < saleQuantity)
            {
                TempData["Error"] = "Yetersiz stok!";
                return RedirectToAction("Index");
            }

            // Stok güncellemesi
            stock.Quantity -= saleQuantity;
            _stockService.TUpdate(stock);

            // Satış kaydı oluşturuluyor
            var sale = new Sale
            {
                ProductID = productId,
                WarehouseID = warehouseId,
                Quantity = saleQuantity,
                SaleDate = DateTime.Now,
                CustomerID = customer.CustomerID,
            };

            try
            {
                _saleService.TAdd(sale);
                TempData["Success"] = "Satış işlemi başarılı!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Satış işlemi sırasında hata oluştu: {ex.Message}";
            }
            if (!ModelState.IsValid)
            {
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var value = ModelState[modelStateKey];
                    foreach (var error in value.Errors)
                    {
                        Console.WriteLine($"{modelStateKey}: {error.ErrorMessage}");
                    }
                }
                TempData["Error"] = "Form doğrulama hatası! Eksik veya hatalı alanlar mevcut.";
                return RedirectToAction("AddSale");
            }

            return RedirectToAction("Index","UserSale");
        }

        public IActionResult StokList()
        {
            var stocks = _stockService.TGetListWithDetails();
            return View(stocks); // Index.cshtml'e gönderiyoruz
        }


    }
}