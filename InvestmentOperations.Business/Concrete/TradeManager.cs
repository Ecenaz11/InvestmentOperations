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
using System.Reflection.Metadata.Ecma335;
using InvestmentOperations.Entities.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace InvestmentOperations.Business.Concrete
{
    public class TradeManager : ITradeService
    {
        private readonly ITradeDal _tradeDal;
        private readonly IUserService _userService;
        private readonly IAssetService _assetService;
        private readonly IAssetHoldingService _assetHoldingService;
        private readonly IPriceService _priceService;
        private readonly ILogService _logService;
        private readonly IAssetHoldingDal _assetHoldingDal;
        private readonly IUnitOfWork _unitOfWork;

        public TradeManager(ITradeDal tradeDal, IUserService userService, IAssetService assetService, IAssetHoldingService assetHoldingService, IPriceService priceService, ILogService logService, IAssetHoldingDal assetHoldingDal, IUnitOfWork unitOfWork)
        {
            _tradeDal = tradeDal;
            _assetService = assetService;
            _userService = userService;
            _assetHoldingService = assetHoldingService;
            _priceService = priceService;
            _logService = logService;
            _assetHoldingDal = assetHoldingDal;
            _unitOfWork = unitOfWork;
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

            if (trade.TradeType == TradeType.SELL)
            {
                result = CheckSufficientAssetHolding(trade.UserId, trade.AssetId, trade.Quantity);
                if (!result.Success)
                {
                    var asset = _assetService.GetById(trade.AssetId).Data;
                    _logService.Add(new Log
                    {
                        UserId = trade.UserId,
                        Action = "TradeAddFailed",
                        Details = $"Insufficient balance. Asset: {asset?.AssetName}, Quantity : {trade.Quantity}",
                        Status = LogStatus.Failed
                    });
                    return result;
                }
            }

            if (trade.TradeType == TradeType.BUY)
            {
                var tlAsset = GetTLAsset();
                if (tlAsset != null)
                {
                    result = CheckSufficientAssetHolding(trade.UserId, tlAsset.AssetId, trade.TotalPrice);
                    if (!result.Success)
                    {
                        _logService.Add(new Log
                        {
                            UserId = trade.UserId,
                            Action = "TradeAddFailed",
                            Details = $"Insufficient balance. Asset: {tlAsset.AssetName}, Required: {trade.TotalPrice}",
                            Status = LogStatus.Failed

                        });

                        return result;
                    }
                }
            }
            _unitOfWork.BeginTransaction();
            try
            {
                _tradeDal.Add(trade);
                UpdateAssetHoldingsAfterTrade(trade);
                _unitOfWork.SaveChanges();
               
                _logService.Add(new Log
            {
                UserId = trade.UserId,
                Action = "TradeAdded",
                Details = $"{trade.TradeType} - AssetId: {trade.AssetId}, Quantity: {trade.Quantity}, UnitPrice: {trade.UnitPrice}, totalPrice: {trade.TotalPrice}",
                Status = LogStatus.Success
            });
           
            _unitOfWork.Commit();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }

            return new SuccessResult("Trade added successfully.");
        }
        
        public IDataResult<List<TradeDto>> GetAll()
        {
            var trades = _tradeDal.GetAll();
            var dtos = trades.Select(MapToDto).ToList();
            return new SuccessDataResult<List<TradeDto>>(dtos, "Trades listed.");
        }
        public IDataResult<List<TradeDto>> GetByUserId(int userId)
        {
            var trades = _tradeDal.GetAll(t => t.UserId == userId);
            var dtos = trades.Select(MapToDto).ToList();
            return new SuccessDataResult<List<TradeDto>>(dtos, "Trades listed.");
        }

        public IDataResult<TradeDto> GetById(int id)
        {
            var trade = _tradeDal.Get(t => t.TradeId == id);
            if (trade == null)
            {
                return new ErrorDataResult<TradeDto>("Trade not found.");
            }
            return new SuccessDataResult<TradeDto>(MapToDto(trade), "Trade retrieved successfully.");
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
            var priceResult = _priceService.GetByAssetId(trade.AssetId);
            if (!priceResult.Success)
            {
                return new ErrorResult("No current price was found for this asset.");
            }
            trade.UnitPrice = priceResult.Data.CurrentPrice;
            return new SuccessResult();
        }

        private IResult CheckAssetIsNotTL(int assetId)
        {
            var tlAsset = GetTLAsset();
            if (tlAsset != null && assetId == tlAsset.AssetId)
            {
                return new ErrorResult("TL cannot be traded directly.");
            }

            return new SuccessResult();
        }

        private IResult CheckSufficientAssetHolding(int userId, int assetId, decimal requiredAmount)
        {
            var assetHolding = _assetHoldingService.GetByUserId(userId).Data?.FirstOrDefault(a => a.AssetId == assetId);
            decimal currentAmount = assetHolding?.Amount ?? 0;

            if (currentAmount < requiredAmount)
            {
                return new ErrorResult("Insufficient balance. You do not have enough of this asset for this transaction.");
            }

            return new SuccessResult();
        }


        private void ApplyAssetHoldingChange(int userId, int assetId, decimal amountDelta)
        {
            var existingAssetHolding = _assetHoldingService.GetByUserId(userId).Data?.FirstOrDefault(a => a.AssetId == assetId);

            if (existingAssetHolding == null)
            {
                var newAssetHolding = new AssetHolding
                {
                    UserId = userId,
                    AssetId = assetId,
                    Amount = amountDelta
                };
                _assetHoldingDal.Add(newAssetHolding);
            }
            else
            {
                existingAssetHolding.Amount += amountDelta;
                _assetHoldingDal.Update(existingAssetHolding);
            }
        }
        private void UpdateAssetHoldingsAfterTrade(Trade trade)
        {
            var tlAsset = GetTLAsset();
            if (tlAsset == null)
            {
                return;
            }

            if (trade.TradeType == TradeType.BUY)
            {
                ApplyAssetHoldingChange(trade.UserId, tlAsset.AssetId, -trade.TotalPrice);
                ApplyAssetHoldingChange(trade.UserId, trade.AssetId, trade.Quantity);
            }
            else if (trade.TradeType == TradeType.SELL)
            {
                ApplyAssetHoldingChange(trade.UserId, tlAsset.AssetId, trade.TotalPrice);
                ApplyAssetHoldingChange(trade.UserId, trade.AssetId, -trade.Quantity);
            }
        }

        #endregion
    }

}
