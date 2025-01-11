using BusinessLayer;
using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using InventoryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly BarcodeGeneratorService _barcodeGeneratorService;

        public ProductController(IProductService productService, BarcodeGeneratorService barcodeGeneratorService)
        {
            _productService = productService;
            _barcodeGeneratorService = barcodeGeneratorService;
        }

        public IActionResult Index()
        {
            var values = _productService.TGetListWithBarcodes()
                  .Where(p => p.IsActive) // Sadece aktif ürünleri getir
                  .ToList();
            return View(values); // Index.cshtml'e gönder            return View(values);
        }


      
        [HttpGet]
        public IActionResult AddProduct()
        {
            return View(); // Ürün ekleme sayfasını göster
        }


        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            ModelState.Remove("Stocks");
            ModelState.Remove("WarehouseProducts");
            ModelState.Remove("Barcodes"); // Barcodes alanını ModelState doğrulamasından çıkar
            ModelState.Remove("Sales");

            if (ModelState.IsValid)
            {
                product.IsActive = true;

                if (string.IsNullOrEmpty(product.SKU))
                {
                    product.SKU = GenerateSKU();
                }

                // Barkodu SKU'ya göre otomatik oluştur
                var barcodeBytes = _barcodeGeneratorService.GenerateBarcode(product.SKU);
                var barcodeBase64 = Convert.ToBase64String(barcodeBytes);

                product.Barcodes = new List<ProductBarcode>
                {
                    new ProductBarcode
                    {
                     Barcode = barcodeBase64
                    }
                };


                _productService.TAdd(product);

                return RedirectToAction("Index", "Product");
            }
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is not valid:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
                return View(product); // Eğer model geçerli değilse, hatalarla birlikte formu geri göster
            }


            return View(product);
        }
        [HttpGet]
        public IActionResult GetBarcode(string sku)
        {
            var barcodeBytes = _barcodeGeneratorService.GenerateBarcode(sku);
            return File(barcodeBytes, "image/png");
        }

        private string GenerateSKU()
        {
            // SKU'yu rastgele ya da belirli bir kurala göre oluşturabilirsiniz
            return "SKU" + new Random().Next(100000, 999999).ToString();
        }


       

        [HttpPost]
        public IActionResult SearchProductByBarcode([FromBody] BarcodeRequest request)
        {
            var product = _productService.TGetByBarcode(request.Barcode);  // Barkodla ürünü al
            if (product == null)
            {
                return NotFound("Ürün bulunamadı!");
            }

            // Ürün bilgilerini JSON formatında geri döndür
            return Json(new
            {
                product.ProductID,
                product.ProductName,
                product.Price,
                product.SKU,
                product.Description,
                product.Category
            });
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var product = _productService.TGetByID(id);
            if (product == null)
            {
                return NotFound();  // Ürün bulunamazsa 404 döndür
            }

            try
            {
                // Ürünü pasif hale getir
                product.IsActive = false;  // Silme yerine pasif hale getir
                _productService.TUpdate(product);  // Ürünü güncelle

                TempData["SuccessMessage"] = "Ürün başarıyla pasif hale getirildi.";
            }
            catch (Exception ex)
            {
                // Eğer bir hata oluşursa, örneğin satışlarla ilişkiliyse, mesaj göster
                TempData["ErrorMessage"] = "Bu ürün satışlarla ilişkili olduğu için pasif hale getirilemez.";
            }

            return RedirectToAction("Index", "Product");  // Listeye geri dön
        }




        [HttpGet]
        public IActionResult UpdateProduct(int id)
        {
            var product = _productService.TGetByIDWithBarcodes(id); // Barkodlarla birlikte ürünü getir
            if (product != null)
            {
                return View(product); // Güncelleme sayfasını göster
            }
            return RedirectToAction("Index", "Product"); // Ürün bulunamazsa listeye geri dön
        }

        [HttpPost]
        public IActionResult UpdateProduct(Product product, List<string> barcodes)
        {
            ModelState.Remove("Stocks");
            ModelState.Remove("WarehouseProducts");
            ModelState.Remove("Barcodes"); // Barcodes alanını ModelState doğrulamasından çıkar
            ModelState.Remove("Sales");
            if (ModelState.IsValid) // Form doğrulaması başarılıysa
            {
                Console.WriteLine("Debug: UpdateProduct called");
                Console.WriteLine($"ProductName: {product.ProductName}");
                Console.WriteLine($"Price: {product.Price}");
                Console.WriteLine($"Description: {product.Description}");
                Console.WriteLine($"SKU: {product.SKU ?? "No SKU provided"}");
                Console.WriteLine($"Category: {product.Category ?? "No Category provided"}");

                var existingProduct = _productService.TGetByIDWithBarcodes(product.ProductID);

                if (existingProduct != null)
                {
                    // Ürün bilgilerini güncelle
                    existingProduct.ProductName = product.ProductName;
                    existingProduct.Description = product.Description;
                    existingProduct.Category = product.Category;
                    existingProduct.Price = product.Price;

                    // Barkodları güncelle
                    if (barcodes != null && barcodes.Any())
                    {
                        existingProduct.Barcodes.Clear(); // Önceki barkodları temizle

                        foreach (var barcode in barcodes)
                        {
                            existingProduct.Barcodes.Add(new ProductBarcode
                            {
                                Barcode = barcode
                            });
                        }
                    }

                    _productService.TUpdate(existingProduct); // Güncelle
                    Console.WriteLine("Product successfully updated.");

                    return RedirectToAction("Index", "Product"); // Listeye geri dön
                }
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is not valid:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
                return View(product); // Eğer model geçerli değilse, hatalarla birlikte formu geri göster
            }

            return View(product); // Model geçerli değilse tekrar göster
        }




    }
}