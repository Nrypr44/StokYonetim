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
    public class CustomerManager : ICustomerService
    {
        private readonly ICustomerDal _customerDal;

        public CustomerManager(ICustomerDal customerDal)
        {
            _customerDal = customerDal;
        }

        public void TAdd(Customer customer)
        {
            _customerDal.Add(customer);
        }

        public void TUpdate(Customer customer)
        {
            _customerDal.Update(customer);
        }

        public void TDelete(Customer customer)
        {
            _customerDal.Delete(customer);
        }

        public Customer TGetByID(int id)
        {
            return _customerDal.GetById(id);
        }

        public List<Customer> TGetList()
        {
            return _customerDal.GetAll();
        }
    }
}
