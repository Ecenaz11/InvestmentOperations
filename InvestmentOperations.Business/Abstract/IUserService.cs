using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using InvestmentOperations.Entities.Dtos;
using System.Text;

namespace InvestmentOperations.Business.Abstract
{
    public interface IUserService
    {
        IResult Add(User user);
        IResult Delete(int id);
        IResult Update(User user);
        IDataResult<List<User>> GetAll();
        IDataResult<User> GetById(int id);
        IDataResult<User> Login(UserForLoginDto dto);
    }
}
