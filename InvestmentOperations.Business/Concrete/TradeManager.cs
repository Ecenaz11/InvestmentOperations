using InvestmentOperation.Core.Utilities.Results;
using InvestmentOperations.Business.Abstract;
using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace InvestmentOperations.Business.Concrete
{
    public class TradeManager : ITradeService
    {
        private readonly ITradeDal _tradeDal;
        private readonly IUserService _userService;
        private readonly IAssetService _assetService;
        public TradeManager(ITradeDal tradeDal, IUserService userService, IAssetService assetService)
        {
            _tradeDal = tradeDal;
            _assetService = assetService;
            _userService = userService;
        }

        public IResult Add(Trade trade)
        {
            CheckDuplicateTradeId(trade.TradeId);
            CheckRelations(trade);
            ValidateTrade(trade);
            PrepareTrade(trade);
            _tradeDal.Add(trade);
            return new SuccessResult("Trade added successfully.");
        }

        public IResult Delete(int id)
        {
            var trade = GetExistingTrade(id);
            _tradeDal.Delete(trade);
            return new SuccessResult("Trade deleted successfully.");
        }
        public IDataResult<List<Trade>> GetAll()
        {
            return new SuccessDataResult<List<Trade>>
                (
                _tradeDal.GetAll(), "Trades listed."
                );
        }

        public IDataResult<Trade> GetById(int id)
        {
            return new SuccessDataResult<Trade>
                (
                GetExistingTrade(id), "Trade found."
                );
        }

        public IResult Update(Trade trade)
        {
            var existingTrade = GetExistingTrade(trade.TradeId);
            CheckRelations(trade);
            ValidateTrade(trade);
            PrepareTrade(trade);
            
            _tradeDal.Update(trade);
            return new SuccessResult("Trade updated successfully.");
        }


        #region Validation Methods
       
        private void PrepareTrade(Trade trade)
        {
            trade.TradeDate = DateTime.Now;
            trade.TotalPrice = trade.Amount * trade.UnitPrice;
            trade.TradeType = trade.TradeType.ToUpper();
        }
        private void ValidateTrade (Trade trade)
        {
            if (trade == null)
            {
                throw new Exception("Trade data cannot be empty.");
            }
            
            if (trade.Amount <= 00)
            {
                throw new Exception("Trade amount must be greater than zero.");
            }

            if(trade.UnitPrice <=0)
            {
                throw new Exception("Trade price must be greater than zero.");
            }

            if ( trade.TradeType != "BUY" && trade.TradeType != "SELL")
            {
                throw new Exception("Invalid trade type. Only 'BUY' or 'SELL' transaction are allowed.");
            }
        }

        private void CheckRelations(Trade trade)
        {
            var user = _userService.GetById(trade.UserId);
            if (user == null)
            {
                throw new Exception($"Transaction failed. User with ID {trade.UserId} does not exist.");
            }

            var asset = _assetService.GetById(trade.AssetId);
            if (asset == null)
            {
                throw new Exception($"Transaction failed. User with ID {trade.AssetId} does not exist.");
            }
        }

        private void CheckDuplicateTradeId(int tradeId)
        {
            var existingTrade = _tradeDal.Get(t => t.TradeId == tradeId);
            if (existingTrade != null)
            {
                throw new Exception($"A trade record with ID {tradeId} already exists.");
            }
        }
        
        private Trade GetExistingTrade(int tradeId)
        {
            var existingTrade = _tradeDal.Get(t => t.TradeId == tradeId);
            if (existingTrade == null)
            {
                throw new Exception($"Trade record with ID {tradeId} was not found.");
            }
            return existingTrade;
                
        }

        public void Delete(Trade trade)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
