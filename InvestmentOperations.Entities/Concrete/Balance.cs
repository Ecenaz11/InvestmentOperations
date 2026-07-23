using InvestmentOperations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InvestmentOperations.Entities.Concrete
{
    public class Balance : IEntity
    {
        public int BalanceId { get; set; }
        public int UserId { get; set; }
        public int AssetId { get; set; }
        public decimal Amount { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
