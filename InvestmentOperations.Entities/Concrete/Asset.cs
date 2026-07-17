using InvestmentOperation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace InvestmentOperations.Entities.Concrete
{
    public class Asset : IEntity
    {
        public int AssetId { get; set; }
        public string AssetName { get; set; }
        public string AssetType { get; set; }
        public  string  AssetCode { get; set; }
    }
}
 