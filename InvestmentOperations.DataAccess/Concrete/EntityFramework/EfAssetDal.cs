using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class EfAssetDal : IAssetDal
    {
        public void Add(Asset asset)
        {
            throw new NotImplementedException();
        }

        public void Delete(Asset asset)
        {
            throw new NotImplementedException();
        }

        public Asset Get(Expression<Func<Asset, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public List<Asset> GetAll(Expression<Func<Asset, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public void Update(Asset asset)
        {
            throw new NotImplementedException();
        }
    }
}
