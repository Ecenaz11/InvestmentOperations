using InvestmentOperations.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using InvestmentOperations.Entities;
using InvestmentOperations.Entities.Concrete;


namespace InvestmentOperations.DataAccess.Concrete.InMemory
{
    public class InMemoryTradeDal : ITradeDal
    {
        List<Trade> _trades;
        public InMemoryTradeDal()
        {
            _trades = new List<Trade>
            {
                new Trade{TradeId = 1, UserId = 1, AssetId = 4, Quantity =2m, UnitPrice = 1949.60m, TotalPrice =3899.20m , TradeType = "BUY", TradeDate =DateTime.Now},
                new Trade{TradeId = 2, UserId = 1, AssetId = 7, Quantity =10m, UnitPrice = 62.55m, TotalPrice = 6255.00m, TradeType = "BUY", TradeDate = DateTime.Now.AddDays(-2)},
                new Trade{TradeId = 3, UserId = 1, AssetId = 3, Quantity =5m, UnitPrice = 2521.20m, TotalPrice = 12606.00m, TradeType = "SELL", TradeDate =DateTime.Now.AddDays(-2) },
                new Trade{TradeId = 4, UserId = 1, AssetId = 1, Quantity =3m, UnitPrice = 6281.50m, TotalPrice = 18844.50m, TradeType = "BUY", TradeDate =DateTime.Now},
                new Trade{TradeId = 5, UserId = 1, AssetId = 6, Quantity =200m, UnitPrice =53.61m , TotalPrice =10722.00m , TradeType = "SELL", TradeDate = DateTime.Now.AddDays(-5)},
                new Trade{TradeId = 6, UserId = 1, AssetId = 5, Quantity =500m, UnitPrice = 46.81m, TotalPrice = 23405.00m, TradeType = "BUY", TradeDate =DateTime.Now.AddDays(-4)},
                new Trade{TradeId = 7, UserId = 1, AssetId = 2, Quantity =50m, UnitPrice =94.05m, TotalPrice = 4702.50m, TradeType = "BUY", TradeDate = DateTime.Now}


            };
        }
        public void Add(Trade trade)
        {
            _trades.Add(trade);
        }

        public void Delete(Trade trade)
        {
            Trade tradeToDelete = _trades.SingleOrDefault(p => p.TradeId == trade.TradeId);
            _trades.Remove(tradeToDelete);
        }

        public Trade Get(Expression<Func<Trade, bool>> filter)
        {
            return _trades.SingleOrDefault(filter.Compile());
        }

        public List<Trade> GetAll(Expression<Func<Trade, bool>> filter = null)
        {
            return _trades;
        }
                

        public void Update(Trade trade)
        {
            Trade tradeToUpdate = _trades.SingleOrDefault(p => p.AssetId == trade.AssetId);
            tradeToUpdate.Quantity = trade.Quantity;
            tradeToUpdate.UnitPrice = trade.UnitPrice;
            tradeToUpdate.TotalPrice = trade.TotalPrice;
            tradeToUpdate.TradeType = trade.TradeType;
            tradeToUpdate.TradeDate = trade.TradeDate;

        }
    }
}
