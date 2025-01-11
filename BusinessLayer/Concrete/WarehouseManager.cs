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
    public class WarehouseManager : IWarehouseService
    {
        private readonly IWarehouseDal _warehouseDal;

        public WarehouseManager(IWarehouseDal warehouseDal)
        {
            _warehouseDal = warehouseDal;
        }

        public void TAdd(Warehouse warehouse)
        {
            try
            {
                _warehouseDal.Add(warehouse);
                Console.WriteLine("Depo başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Depo eklerken karşılaşılan hata : {ex.Message}");
            }
        }

        public void TDelete(Warehouse warehouse)
        {
            try
            {
                _warehouseDal.Delete(warehouse);
                Console.WriteLine($"Depo ID'si {warehouse.WarehouseID} olan depo silindi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Depo silerken karşılaşılan hata : {ex.Message}");
            }
        }

        public Warehouse TGetByID(int id)
        {
            try
            {
                return _warehouseDal.GetById(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Depo ID'si olan ürün bulunurken karşılaşılan hata : {ex.Message}");
                return null;
            }
        }

        public List<Warehouse> TGetList()
        {
            try
            {
                return _warehouseDal.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Depoları listelerken hata : {ex.Message}");
                return new List<Warehouse>();
            }
        }

        public void TUpdate(Warehouse warehouse)
        {
            try
            {
                _warehouseDal.Update(warehouse);
                Console.WriteLine($"Depo ID'si {warehouse.WarehouseID} olan depo güncellendi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Depo güncellerken hata : {ex.Message}");
            }
        }
    }
}
