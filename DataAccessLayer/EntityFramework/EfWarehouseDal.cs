using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.EntityFramework
{
    public class EfWarehouseDal : GenericDal<Warehouse>, IWarehouseDal
    {
        private readonly MyDepoContext _context;

        public EfWarehouseDal(MyDepoContext context) : base(context)
        {
            _context = context;
        }
    }
}