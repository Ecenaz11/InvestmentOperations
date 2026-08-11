using InvestmentOperations.Core.Entities;

namespace InvestmentOperations.Entities.Dtos
{
    public class DepositDto : IDto
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
    }
}
