using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.EntityFramework
{
    public class EfProductDal : GenericDal<Product>, IProductDal
    {
        private readonly MyDepoContext _context;

        public EfProductDal(MyDepoContext context) : base(context)
        {
            _context = context;
        }

        public List<Product> GetAllWithBarcodes()
        {

            return _context.Products
                          .Include(p => p.Barcodes) 
                          .ToList();
        }


    }
}