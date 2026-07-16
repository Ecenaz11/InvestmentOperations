using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace InvestmentOperations.DataAccess.Abstract
{
    public interface IAssetDal : IEntityRepository<Asset> 
    {

    }
}
