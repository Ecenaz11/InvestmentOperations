using InvestmentOperations.Core.Entities;

namespace InvestmentOperations.Entities.Dtos
{
     public class PriceDto : IDto
    {
        public int PriceId{get;set;}
        public string AssetName{get;set;}
        public string AssetCode{get;set;}
        public string AssetType{get;set;}
        public decimal CurrentPrice{get;set;}
        public DateTime UpdatedAt{get;set;}

    }
}