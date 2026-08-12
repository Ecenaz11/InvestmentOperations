using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using System.Collections.Generic;
using InvestmentOperations.Entities.Dtos;
using System.Threading.Tasks;

namespace InvestmentOperations.Business.Abstract
{
    public interface IUserService
    {
        Task<IResult> Add(User user);
        Task<IResult> Delete(int id);
        Task<IResult> Update(User user);
        Task<IDataResult<List<User>>> GetAll();
        Task<IDataResult<User>> GetById(int id);
        Task<IDataResult<User>> Login(UserForLoginDto dto);
    }
}
