using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace InvestmentOperations.Business.Abstract
{
    public interface IAssetService 
    {
        List<Asset> GetAll();
        Asset GetById(int id);
        void Add(Asset asset);
        void Delete(int id);
        void Update(Asset asset);

    }
}
