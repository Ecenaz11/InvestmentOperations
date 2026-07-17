using InvestmentOperation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InvestmentOperations.Entities.Concrete
{
    public class Trade : IEntity
    {
        public int TradeId { get; set; }
        public int  UserId { get; set; }
        public int  AssetId { get; set; }
        public string TradeType { get; set; }
        public decimal Amount  { get; set; }
        public  decimal  UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime TradeDate { get; set; }


    }
}
