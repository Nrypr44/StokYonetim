using Microsoft.AspNetCore.Mvc;
using BusinessLayer;
using EntityLayer.Concrete;
using BusinessLayer.Abstract;

namespace InventoryManagement.Controllers
{
    // API'yi belirli bir URL ile erişilebilir yapıyoruz
    [Route("api/[controller]")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        private readonly IProductService _productService;

        // ProductApiController'a ProductService'i ekliyoruz
        public ProductApiController(IProductService productService)
        {
            _productService = productService;
        }

        // API üzerinden SKU'ya göre ürün verisini almak için metod
        [HttpGet("{sku}")]
        public IActionResult GetProductBySku(string sku)
        {
            var product = _productService.GetBySKU(sku); // SKU ile ürünü alıyoruz
            if (product == null)
            {
                return NotFound("Product not found"); // Eğer ürün bulunamazsa hata döndür
            }
            return Ok(product); // Ürünü döndürüyoruz
        }
    }
}
