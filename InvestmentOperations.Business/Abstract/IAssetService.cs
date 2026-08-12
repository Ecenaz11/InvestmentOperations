using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentOperations.Business.Abstract
{
    public interface IAssetService
    {
        Task<IDataResult<List<Asset>>> GetAll();
        Task<IDataResult<Asset>> GetById(int id);
        Task<IResult> Add(Asset asset);
        Task<IResult> Delete(int id);
        Task<IResult> Update(Asset asset);
    }
}
