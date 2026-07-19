using InvestmentOperations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InvestmentOperations.Entities.Concrete
{
    public class Price : IEntity
    {
        public int PriceId { get; set; }
        public int AssetId { get; set; }
        public  decimal  CurrentPrice { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
