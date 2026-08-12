using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;
using InvestmentOperations.Core.DataAccess;

namespace InvestmentOperations.DataAccess.Abstract
{
    public interface IUserDal : IEntityRepository<User>
    {
    }
}
