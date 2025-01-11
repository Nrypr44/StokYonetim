using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace BusinessLayer.Concrete
{
    public class StockManager : IStockService
    {
        private readonly IStockDal _stockDal;
        private readonly IWarehouseDal _warehouseDal;
        private readonly EmailSettings _emailSettings;

        public StockManager(IStockDal stockDal, IWarehouseDal warehouseDal, IOptions<EmailSettings> emailSettings)
        {
            _stockDal = stockDal;
            _warehouseDal = warehouseDal;
            _emailSettings = emailSettings.Value;

        }

        public void CheckStockLevel(int productId)
        {
            Console.WriteLine("Stok kontrol fonksiyonu çalıştı...");

            var stock = _stockDal.GetQueryable()
                                 .Include(s => s.Product)  
                                 .FirstOrDefault(s => s.ProductID == productId);

            if (stock == null)
            {
                Console.WriteLine("Stok verisi bulunamadi. Ürün ID: " + productId);
                return;
            }

            Console.WriteLine($"Stok ID: {stock.StockID}, Ürün: {stock.Product.ProductName}, Miktar: {stock.Quantity}");

            if (stock.Quantity < 10)
            {
                Console.WriteLine("Stok seviyesi kritik: 10'dan az.");

                var warehouse = _warehouseDal.GetById(stock.WarehouseID);
                if (warehouse == null)
                {
                    Console.WriteLine("Depo bilgisi bulunamadı.");
                    return;
                }

                Console.WriteLine($"Depo ID: {warehouse.WarehouseID}, Yönetici E-posta: {warehouse.ManagerEmail}");

                if (string.IsNullOrEmpty(warehouse.ManagerEmail))
                {
                    Console.WriteLine("Yönetici e-posta adresi boş.");
                    return;
                }

                Console.WriteLine("Kritik stok uyarısı gönderiliyor...");

                try
                {
                    SendCriticalStockEmail(
                        warehouse.ManagerEmail,
                        warehouse.ManagerName,
                        stock.Product.ProductName,
                        stock.Quantity
                    );
                    Console.WriteLine("E-posta başarıyla gönderildi.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Mail gönderilirken hata oluştu: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Stok seviyesi kritik değil.");
            }
        }







        private void SendCriticalStockEmail(string managerEmail, string managerName, string productName, int quantity)
        {
            var mimeMessage = new MimeMessage();

          
            mimeMessage.From.Add(new MailboxAddress("Stok Yönetim Sistemi", _emailSettings.SenderEmail));

            
            mimeMessage.To.Add(new MailboxAddress(managerName, managerEmail));

            mimeMessage.Subject = "⚠️ Kritik Stok Uyarısı";

            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Merhaba {managerName},\n\n" +
                           $"Ürün: {productName} kritik stok seviyesine düştü.\n" +
                           $"Mevcut miktar: {quantity}\n\n" +
                           $"İlgili aksiyonları almanız rica olunur.\n\n" +
                           "Saygılarımızla,\nStok Yönetim Sistemi"
            };

            mimeMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    
                    client.Connect(_emailSettings.SmtpHost, 587, SecureSocketOptions.StartTls);

                    
                    client.Authenticate(_emailSettings.Username, _emailSettings.Password);

                    
                    client.Send(mimeMessage);
                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Mail gönderilirken hata oluştu: {ex.Message}");
                    Console.WriteLine($"Hata Detayı: {ex.StackTrace}");  
                }
            }
        }

        public void TAdd(Stock stock)
        {
            try
            {
                _stockDal.Add(stock);
                Console.WriteLine("Stok başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Stok eklerken karşılaşılan hata: {ex.Message}");
            }
        }

        public void TDelete(Stock stock)
        {
            try
            {
                _stockDal.Delete(stock);
                Console.WriteLine($"Stock ID'si {stock.StockID} olan stok silindi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Stok silerken karşılaşılan hata : {ex.Message}");
            }
        }

        public void TUpdate(Stock stock)
        {
            try
            {
                var existingStock = _stockDal.GetById(stock.StockID); 
                if (existingStock != null)
                {
                    existingStock.Quantity = stock.Quantity; 
                    existingStock.WarehouseID = stock.WarehouseID; 
                    existingStock.Date = stock.Date;
                    existingStock.StockMovementType = stock.StockMovementType;
                    _stockDal.Update(existingStock); 
                    Console.WriteLine("Stok başarıyla güncellendi.");
                    CheckStockLevel(existingStock.ProductID);



                }
                else
                {
                    Console.WriteLine("Stok bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Stok güncellerken karşılaşılan hata : {ex.Message}");
            }
        }



        public List<Stock> TGetList()
        {
            return _stockDal.GetAll();
        }

        public Stock TGetByID(int id)
        {
            return _stockDal.GetById(id);
        }

        public List<Stock> TGetListWithDetails()
        {
            return _stockDal.GetQueryable()
                            .Include(s => s.Product)   
                            .Include(s => s.Warehouse) 
                            .ToList();
        }

        public Stock TGetByIDWithDetails(int id)
        {
            return _stockDal.GetQueryable()
                .Include(s => s.Product) 
                .Include(s => s.Warehouse) 
                .FirstOrDefault(s => s.StockID == id);
        }

        public Stock GetStockByIDWithProduct(int id)
        {
            return _stockDal.GetByIDWithProduct(id);
        }
       

        public Stock GetStockByProductAndWarehouse(int productId, int warehouseId)
        {
            return _stockDal.GetQueryable()
                .FirstOrDefault(s => s.ProductID == productId && s.WarehouseID == warehouseId);
        }

        public List<(string WarehouseName, int TotalStock)> GetWarehouseStockDistribution()
        {
            return _stockDal.GetAll()
                            .GroupBy(s => s.Warehouse.WarehouseName)
                            .Select(g => (WarehouseName: g.Key, TotalStock: g.Sum(s => s.Quantity)))
                            .ToList();
        }
    }

}
