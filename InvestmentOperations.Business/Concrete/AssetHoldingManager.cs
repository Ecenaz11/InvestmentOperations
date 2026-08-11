using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Business.Abstract;
using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using InvestmentOperations.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Security.Claims;
using IHttpContextAccessor = Microsoft.AspNetCore.Http.IHttpContextAccessor;
using InvestmentOperations.Entities.Enums;

namespace InvestmentOperations.Business.Concrete
{
    public class AssetHoldingManager : IAssetHoldingService
    {
        private readonly IAssetHoldingDal _assetHoldingDal;
        private readonly IAssetService _assetService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogService _logService;
        public AssetHoldingManager(IAssetHoldingDal assetHoldingDal, IAssetService assetService, IUserService userService, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ILogService logService)
        {
            _assetHoldingDal = assetHoldingDal;
            _assetService = assetService;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logService = logService;
        }

        public IResult Add(AssetHolding assetHolding)
        {
            IResult result = ValidateAssetHolding(assetHolding);
            if (!result.Success)
            {
                return result;
            }

            result = CheckUserExists(assetHolding.UserId);
            if (!result.Success)
            {
                return result;
            }

            result = CheckAssetExisting(assetHolding.AssetId);
            if (!result.Success)
            {
                return result;
            }

            var existingAssetHolding = _assetHoldingDal.Get(a => a.UserId == assetHolding.UserId && a.AssetId == assetHolding.AssetId);
            if (existingAssetHolding != null)
            {
                existingAssetHolding.Amount += assetHolding.Amount;
                PrepareAssetHolding(existingAssetHolding);
                _assetHoldingDal.Update(existingAssetHolding);
                _unitOfWork.SaveChanges();
                LogAction("AssetHoldingIncreased", $"UserId: {existingAssetHolding.UserId}, AssetId: {existingAssetHolding.AssetId}, NewAmount: {existingAssetHolding.Amount}");
                return new SuccessResult("Asset holding updated successfully.");
            }

            PrepareAssetHolding(assetHolding);

            _assetHoldingDal.Add(assetHolding);
            _unitOfWork.SaveChanges();
            LogAction("AssetHoldingAdded", $"UserId: {assetHolding.UserId}, AssetId: {assetHolding.AssetId}, Amount: {assetHolding.Amount}");
            return new SuccessResult("Asset holding added successfully.");
        }

        public IResult Deposit(int userId, decimal amount)
        {
            var tlAsset = _assetService.GetAll().Data?.FirstOrDefault(a => a.AssetCode == "TL");
            if (tlAsset == null)
            {
                return new ErrorResult("TL asset not found.");
            }

            var assetHolding = new AssetHolding
            {
                UserId = userId,
                AssetId = tlAsset.AssetId,
                Amount = amount
            };

            return Add(assetHolding);
        }

        public IResult Delete(int id)
        {
            var assetHolding = _assetHoldingDal.Get(a => a.AssetHoldingId == id);
            if (assetHolding == null)
            {
                return new ErrorResult("Asset holding not found.");
            }

            _assetHoldingDal.Delete(assetHolding);
            _unitOfWork.SaveChanges();
            LogAction("AssetHoldingDeleted", $"AssetHoldingId: {id}");
            return new SuccessResult("Asset holding deleted successfully.");
        }

        public IDataResult<List<AssetHolding>> GetByUserId(int userId)
        {
            var assetHoldings = _assetHoldingDal.GetAll(a => a.UserId == userId);
            LogAction("AssetHoldingsListedByUser", $"UserId: {userId}");
            return new SuccessDataResult<List<AssetHolding>>(assetHoldings, "Asset holdings listed.");
        }

        public IDataResult<List<AssetHoldingDto>> GetAllDetailed()
        {
            var assetHoldings = _assetHoldingDal.GetAll();
            var dtos = assetHoldings.Select(MapToDto).ToList();

            LogAction("AssetHoldingsListed", $"Count: {dtos.Count}");

            return new SuccessDataResult<List<AssetHoldingDto>>(dtos, "Asset holdings listed.");
        }

        public IDataResult<AssetHoldingDto> GetByIdDetailed(int id)
        {
            var assetHolding = _assetHoldingDal.Get(a => a.AssetHoldingId == id);
            if (assetHolding == null)
            {
                return new ErrorDataResult<AssetHoldingDto>("Asset holding not found.");
            }
            LogAction("AssetHoldingViewed", $"AssetHoldingId: {id}");

            return new SuccessDataResult<AssetHoldingDto>(MapToDto(assetHolding), "Asset holding found.");
        }

        public IDataResult<List<AssetHoldingDto>> GetByUserIdDetailed(int userId)
        {
            var result = GetByUserId(userId);
            var dtos = result.Data.Select(MapToDto).ToList();
            return new SuccessDataResult<List<AssetHoldingDto>>(dtos, "Asset holdings listed.");
        }

        public IResult Update(AssetHolding assetHolding)
        {
            var existingAssetHolding = _assetHoldingDal.Get(a => a.AssetHoldingId == assetHolding.AssetHoldingId);
            if (existingAssetHolding == null)
            {
                return new ErrorResult("Asset holding not found.");
            }

            PrepareAssetHolding(assetHolding);

            IResult result = ValidateAssetHolding(assetHolding);
            if (!result.Success)
            {
                return result;
            }

            _assetHoldingDal.Update(assetHolding);
            _unitOfWork.SaveChanges();
            LogAction("AssetHoldingUpdated", $"AssetHoldingId: {assetHolding.AssetHoldingId}, AssetId: {assetHolding.AssetId}, Amount: {assetHolding.Amount}");
            return new SuccessResult("Asset holding updated successfully.");
        }
        private AssetHoldingDto MapToDto(AssetHolding assetHolding)
        {
            var asset = _assetService.GetById(assetHolding.AssetId).Data;
            return new AssetHoldingDto
            {
                AssetHoldingId = assetHolding.AssetHoldingId,
                UserId = assetHolding.UserId,
                AssetId = assetHolding.AssetId,
                AssetName = asset?.AssetName,
                AssetCode = asset?.AssetCode,
                AssetType = asset?.AssetType,
                Amount = assetHolding.Amount
            };
        }
        private void LogAction(string action, string details)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = userIdClaim != null ? int.Parse(userIdClaim) : 0;

            _logService.Add(new Log
            {
                UserId = userId,
                Action = action,
                Details = details,
                Status = LogStatus.Success
            });
        }

        #region Validation Methods
        private IResult ValidateAssetHolding(AssetHolding assetHolding)
        {
            if (assetHolding == null)
            {
                return new ErrorResult("Asset holding cannot be empty.");
            }

            if (assetHolding.UserId <= 0)
            {
                return new ErrorResult("UserId must be greater than zero.");
            }

            if (assetHolding.AssetId <= 0)
            {
                return new ErrorResult("AssetId must be greater than zero.");
            }

            if (assetHolding.Amount < 0)
            {
                return new ErrorResult("Amount cannot be negative.");
            }

            return new SuccessResult();
        }
        private void PrepareAssetHolding(AssetHolding assetHolding)
        {
            assetHolding.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        }

        private IResult CheckUserExists(int userId)
        {
            var user = _userService.GetById(userId);
            if (!user.Success)
            {
                return new ErrorResult("User not found.");
            }
            return new SuccessResult();
        }

        private IResult CheckAssetExisting(int assetId)
        {
            var asset = _assetService.GetById(assetId);
            if (!asset.Success)
            {
                return new ErrorResult("Asset not found.");
            }
            return new SuccessResult();
        }

        #endregion

    }
}
