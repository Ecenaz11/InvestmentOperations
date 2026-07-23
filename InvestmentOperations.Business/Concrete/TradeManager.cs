using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Business.Abstract;
using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using InvestmentOperations.Entities.Dtos;
using System.Security.Cryptography;

namespace InvestmentOperations.Business.Concrete
{
    public class TradeManager : ITradeService
    {
        private readonly ITradeDal _tradeDal;
        private readonly IUserService _userService;
        private readonly IAssetService _assetService;
        private readonly IBalanceService _balanceService;
        private readonly IPriceService _priceService;
        public TradeManager(ITradeDal tradeDal, IUserService userService, IAssetService assetService, IBalanceService balanceService, IPriceService priceService)
        {
            _tradeDal = tradeDal;
            _assetService = assetService;
            _userService = userService;
            _balanceService = balanceService;
            _priceService = priceService;
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

            result = CheckAssetIsNotTL(trade.AssetId);
            if (!result.Success)
            {
                return result;
            }

            result = SetCurrentUnitPrice(trade);
            if(!result.Success)
            {
                return result;
            }

            PrepareTrade(trade);

            result = ValidateTrade(trade);
            if (!result.Success)
            {
                return result;
            }

            if (trade.TradeType == "SELL")
            {
                result = CheckSufficientBalance(trade.UserId, trade.AssetId, trade.Quantity);
                if (!result.Success)
                {
                    return result;
                }
            }

            _tradeDal.Add(trade);

            UpdateBalancesAfterTrade(trade);

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

        public IDataResult<List<TradeDto>>GetAll()
        {
            var trades = _tradeDal.GetAll();
            var dtos = trades.Select(MapToDto).ToList();
            return new SuccessDataResult<List<TradeDto>>(dtos, "Trades listed.");
        }

        public IDataResult<TradeDto> GetById(int id)
        {
            var trade = _tradeDal.Get(t=> t.TradeId ==id);
            if(trade ==null)
            {
                return new ErrorDataResult<TradeDto>("Trade not found.");
            }
            return new SuccessDataResult<TradeDto>(MapToDto(trade), "Trade retrieved successfully.");
        }

        

        public IResult Update(Trade trade)
        {
            var existingTrade = _tradeDal.Get(t => t.TradeId == trade.TradeId);
            if (existingTrade == null)
            {
                return new ErrorResult($"Trade with ID {trade.TradeId} does not exist.");
            }

             trade.UnitPrice = existingTrade.UnitPrice;


            IResult result = CheckRelations(trade);
            if (!result.Success)
            {
                return result;
            }

            result = CheckAssetIsNotTL(trade.AssetId);
            if (!result.Success)
            {
                return result;
            }

            PrepareTrade(trade);

            result = ValidateTrade(trade);
            if (!result.Success)
            {
                return result;
            }

            if (trade.TradeType == "SELL")
            {
                result = CheckSufficientBalance(trade.UserId, trade.AssetId, trade.Quantity);
                if (!result.Success)
                {
                    return result;
                }
            }

            _tradeDal.Update(trade);

            UpdateBalancesAfterTrade(trade);

            return new SuccessResult("Trade updated successfully.");
        }

        private TradeDto MapToDto(Trade trade)
        {
            var asset = _assetService.GetById(trade.AssetId).Data;
            return new TradeDto
            {
                TradeId = trade.TradeId,
                UserId = trade.UserId,
                AssetName = asset?.AssetName,
                AssetCode = asset?.AssetCode,
                AssetType = asset?.AssetType,
                TradeType = trade.TradeType,
                Quantity = trade.Quantity,
                UnitPrice = trade.UnitPrice,
                TotalPrice = trade.TotalPrice,
                TradeDate = trade.TradeDate

            };
        }




        #region Validation Methods

        private void PrepareTrade(Trade trade)
        {
            trade.TradeDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            trade.TotalPrice = trade.Quantity * trade.UnitPrice;
            trade.TradeType = trade.TradeType.ToUpper();
        }


        private IResult ValidateTrade(Trade trade)
        {
            if (trade == null)
                return new ErrorResult("Trade data cannot be empty.");

            if (trade.Quantity <= 00)
            {
                return new ErrorResult("Trade quantity must be greater than zero.");
            }

            if (trade.UnitPrice <= 0)
            {
                return new ErrorResult("Trade price must be greater than zero.");
            }

            if (trade.TradeType != "BUY" && trade.TradeType != "SELL")
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
                return new ErrorResult($"Transaction failed. User with ID {trade.UserId} does not exist.");
            }

            var asset = _assetService.GetById(trade.AssetId);
            if (!asset.Success)
            {
                return new ErrorResult($"Transaction failed. Asset with ID {trade.AssetId} does not exist.");
            }

            return new SuccessResult();
        }


        private IResult CheckDuplicateTradeId(int tradeId)
        {
            var existingTrade = _tradeDal.Get(t => t.TradeId == tradeId);
            if (existingTrade != null)
            {
                return new ErrorResult($"A trade record with ID {tradeId} already exists.");
            }

            return new SuccessResult();
        }


        private Asset GetTLAsset()
        {
            return _assetService.GetAll().Data?.FirstOrDefault(a => a.AssetCode == "TL");
        }

        private IResult SetCurrentUnitPrice(Trade trade)
        {
            var priceResult = _priceService.GeyByAssetId(trade.AssetId);
            if (!priceResult.Success)
            {
                return new ErrorResult("No current price was found for this asset.");
            }
            trade.UnitPrice= priceResult.Data.CurrentPrice;
            return new SuccessResult();
        }


        private IResult CheckAssetIsNotTL(int assetId)
        {
            var tlAsset = GetTLAsset();
            if (tlAsset != null && assetId == tlAsset.AssetId)
            {
                return new ErrorResult("TL doğrudan trade edilemez.");
            }

            return new SuccessResult();
        }


        private IResult CheckSufficientBalance(int userId, int assetId, decimal requiredAmount)
        {
            var balance = _balanceService.GetByUserId(userId).Data?.FirstOrDefault(b => b.AssetId == assetId);
            decimal currentAmount = balance?.Amount ?? 0;

            if (currentAmount < requiredAmount)
            {
                return new ErrorResult("Yetersiz bakiye. Bu işlem için yeterli miktarda varlığınız bulunmuyor.");
            }

            return new SuccessResult();
        }


        private void ApplyBalanceChange(int userId, int assetId, decimal amountDelta)
        {
            var existingBalance = _balanceService.GetByUserId(userId).Data?.FirstOrDefault(b => b.AssetId == assetId);

            if (existingBalance == null)
            {
                var newBalance = new Balance
                {
                    UserId = userId,
                    AssetId = assetId,
                    Amount = amountDelta
                };
                _balanceService.Add(newBalance);
            }
            else
            {
                existingBalance.Amount += amountDelta;
                _balanceService.Update(existingBalance);
            }
        }


        private void UpdateBalancesAfterTrade(Trade trade)
        {
            var tlAsset = GetTLAsset();
            if (tlAsset == null)
            {
                return;
            }

            if (trade.TradeType == "BUY")
            {
                ApplyBalanceChange(trade.UserId, tlAsset.AssetId, -trade.TotalPrice);
                ApplyBalanceChange(trade.UserId, trade.AssetId, trade.Quantity);
            }
            else if (trade.TradeType == "SELL")
            {
                ApplyBalanceChange(trade.UserId, tlAsset.AssetId, trade.TotalPrice);
                ApplyBalanceChange(trade.UserId, trade.AssetId, -trade.Quantity);
            }
        }

        #endregion
    }

}
