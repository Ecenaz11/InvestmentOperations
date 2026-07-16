using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class EfTradeDal : ITradeDal
    {
        public void Add(Trade trade)
        {
            throw new NotImplementedException();
        }

        public void Delete(Trade trade)
        {
            throw new NotImplementedException();
        }

        public Trade Get(Expression<Func<Trade, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public List<Trade> GetAll(Expression<Func<Trade, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public void Update(Trade trade)
        {
            throw new NotImplementedException();
        }
    }
}
