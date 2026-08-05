using InvestmentOperations.Entities.Concrete;

namespace InvestmentOperations.Business.Abstract
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}