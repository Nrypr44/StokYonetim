using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly IWarehouseService _warehouseService;

        // Constructor Dependency Injection
        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        public IActionResult Index()
        {
            var warehouses = _warehouseService.TGetList();
            return View(warehouses);
        }

        public IActionResult AddWarehouse()
        {
            return View();
        }

        // POST: Warehouse/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddWarehouse(Warehouse warehouse)
        {
            ModelState.Remove("Stocks");
            ModelState.Remove("WarehouseProducts");
            ModelState.Remove("Sales");
            if (ModelState.IsValid)
            {
                try
                {
                    _warehouseService.TAdd(warehouse);  // WarehouseManager'da ekleme işlemi yapılır
                    return RedirectToAction(nameof(Index)); // Başarılı ekleme sonrası listeye yönlendirilir
                }
                catch (Exception ex)
                {
                    // Hata durumu, kullanıcının bilgilendirilmesi için
                    ViewBag.ErrorMessage = $"Error: {ex.Message}";
                }
            }

            return View(warehouse);
        }

        public IActionResult EditWarehouse(int id)
        {
            var warehouse = _warehouseService.TGetByID(id);  // WarehouseManager'dan id'ye göre buluyoruz
            if (warehouse == null)
            {
                return NotFound();  // Eğer Warehouse bulunamazsa 404 döndürüyoruz
            }
            return View(warehouse);  // Düzenleme formunu göstermek için
        }

        // POST: Warehouse/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditWarehouse(int id, Warehouse warehouse)
        {
            ModelState.Remove("Stocks");
            ModelState.Remove("WarehouseProducts");
            ModelState.Remove("Sales");


            if (id != warehouse.WarehouseID)
            {
                return BadRequest("ID eşleşmiyor.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _warehouseService.TUpdate(warehouse);
                    return RedirectToAction("Index", "Warehouse");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"Error: {ex.Message}";
                }
            }

            return View(warehouse);
        }

        // POST: Warehouse/Delete/5
        // POST: Warehouse/DeleteWarehouse/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteWarehouse(int id)
        {
            var warehouse = _warehouseService.TGetByID(id);
            if (warehouse == null)
            {
                return NotFound();  // Eğer depo bulunamazsa, 404 döndürülür.
            }

            try
            {
                _warehouseService.TDelete(warehouse);  // Depoyu silme işlemi
                TempData["SuccessMessage"] = "Depo başarıyla silindi.";
                return RedirectToAction(nameof(Index));  // Başarı durumunda liste sayfasına yönlendir.
            }
            catch (Exception ex)
            {
                // Hata durumu: işlemi gerçekleştirememe (silme sırasında)
                ViewBag.ErrorMessage = $"Depo silinirken bir hata oluştu: {ex.Message}";

                // Eğer hata nedeniyle silinemiyorsa, kullanıcıya uyarı mesajını göster.
                TempData["ErrorMessage"] = "Depo silinirken bir hata oluştu. Lütfen ilişkili verileri kontrol edin.";
                return RedirectToAction(nameof(Index));  // Aynı liste sayfasına yönlendir.
            }
        }




    }
}
