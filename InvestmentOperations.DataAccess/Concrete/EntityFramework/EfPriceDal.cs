using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class EfPriceDal : IPriceDal
    {
        public void Add(Price price)
        {
            throw new NotImplementedException();
        }

        public void Delete(Price price)
        {
            throw new NotImplementedException();
        }

        public Price Get(Expression<Func<Price, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public List<Price> GetAll(Expression<Func<Price, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public void Update(Price price)
        {
            throw new NotImplementedException();
        }
    }
}
