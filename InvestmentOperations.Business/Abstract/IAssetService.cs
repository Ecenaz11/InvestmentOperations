using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace InvestmentOperations.Business.Abstract
{
    public interface IAssetService 
    {
        IDataResult<List<Asset>> GetAll();
        IDataResult<Asset> GetById(int id);
        IResult Add(Asset asset);
        IResult Delete(int id);
        IResult Update(Asset asset);

    }
}
