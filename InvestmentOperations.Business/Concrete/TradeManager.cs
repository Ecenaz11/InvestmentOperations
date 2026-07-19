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
            IResult result = CheckDuplicateTradeId(trade.TradeId);
            if (!result.Success)
            {
                return result;
            }


            result = CheckRelations(trade);
            if (!result.Success)
            {
                return result;
            }


            result = ValidateTrade(trade);
            if (!result.Success)
            {
                return result;
            }

            PrepareTrade(trade);

            _tradeDal.Add(trade);
            return new SuccessResult("Trade added successfully.");
        }

      

        public IResult Delete(int id)
        {
            var trade = _tradeDal.Get(t => t.TradeId == id);
            if (trade == null)
            {
                return new ErrorResult($"Trade with ID {id} does not exist.");
            }
           
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
            var trade = _tradeDal.Get(t => t.TradeId == id);
            if (trade == null)
            {
                return new ErrorDataResult<Trade>($"Trade with ID {id} does not exists.");

            }
            return new SuccessDataResult<Trade>(trade, "Trade retrieved successfully.");
        }


        public IResult Update(Trade trade)
        {
            var existingTrade = _tradeDal.Get(t => t.TradeId == trade.TradeId);
            if (existingTrade == null)
            {
                return new ErrorResult($"Trade with ID {trade.TradeId} does not exist.");
            }
          

          
           IResult result =  CheckRelations(trade);
            if (!result.Success)
            {
                return result;
            }
           
             
            
            result = ValidateTrade(trade);
            if (!result.Success)
            {
                return result;
            }   
            

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


        private IResult ValidateTrade (Trade trade)
        {
            if (trade == null)
            {
                return new ErrorResult("Trade data cannot be empty.");
            }
            
            if (trade.Amount <= 00)
            {
                return new ErrorResult("Trade amount must be greater than zero.");
            }

            if(trade.UnitPrice <=0)
            {
                return new ErrorResult("Trade price must be greater than zero.");
            }

            if ( trade.TradeType != "BUY" && trade.TradeType != "SELL")
            {
                return new ErrorResult("Invalid trade type. Only 'BUY' or 'SELL' transaction are allowed.");
            }

            return new SuccessResult();
        }


        private IResult CheckRelations(Trade trade)
        {
            var user = _userService.GetById(trade.UserId);
            if (!user.Success)
            {
                return new ErrorResult ($"Transaction failed. User with ID {trade.UserId} does not exist.");
            }

            var asset = _assetService.GetById(trade.AssetId);
            if (!asset.Success)
            {
                return new ErrorResult ($"Transaction failed. Asset with ID {trade.AssetId} does not exist.");
            }

            return new SuccessResult();
        }


        private IResult CheckDuplicateTradeId(int tradeId)
        {
            var existingTrade = _tradeDal.Get(t => t.TradeId == tradeId);
            if (existingTrade != null)
            {
                return new ErrorResult ($"A trade record with ID {tradeId} already exists.");
            }

            return new SuccessResult();
        }
        
        #endregion
    }

}
