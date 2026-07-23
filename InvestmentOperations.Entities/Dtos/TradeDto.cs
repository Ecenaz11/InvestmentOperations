using System;
using InvestmentOperations.Core.Entities;

namespace InvestmentOperations.Entities.Dtos
{
    public class TradeDto : IDto
    {
        public int TradeId {get; set;}
        public int UserId {get; set;}
        public string AssetName {get; set;}
        public string AssetCode {get; set;}
        public string AssetType {get; set;}
        public string TradeType {get; set;}
        public decimal Quantity {get; set;}
        public decimal UnitPrice {get; set;}
        public decimal TotalPrice {get; set;}
        public DateTime TradeDate {get; set;}
    }
}