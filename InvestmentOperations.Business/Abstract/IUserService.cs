using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace InvestmentOperations.Business.Abstract
{
    public interface IUserService
    {
        void Add(User user);
        void Delete(int id);
        void Update(User user);
        List<User> GetAll();
        User GetById(int id);
    }
}
