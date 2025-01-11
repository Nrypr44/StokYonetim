using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class MyDepoContext : DbContext
    {
       
        public MyDepoContext(DbContextOptions<MyDepoContext> options) : base(options) 
        {
        
        }

      
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBarcode> ProductBarcodes { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Customer> Customers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Barcodes)
                .WithOne(b => b.Product)
                .HasForeignKey(b => b.ProductID)
                .OnDelete(DeleteBehavior.Cascade);
            

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Warehouse)
                .WithMany(w => w.Stocks)
                .HasForeignKey(s => s.WarehouseID);

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Product)
                .WithMany(p => p.Stocks)
                .HasForeignKey(s => s.ProductID);

            modelBuilder.Entity<Sale>()
              .HasOne(s => s.Product)
              .WithMany(p => p.Sales)
              .HasForeignKey(s => s.ProductID)
              .OnDelete(DeleteBehavior.Restrict); 

           
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Warehouse)
                .WithMany(w => w.Sales)
                .HasForeignKey(s => s.WarehouseID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sale>()
             .HasOne(s => s.Customer)
             .WithMany(c => c.Sales)
             .HasForeignKey(s => s.CustomerID)
             .OnDelete(DeleteBehavior.Restrict);

           
        }
    }
}