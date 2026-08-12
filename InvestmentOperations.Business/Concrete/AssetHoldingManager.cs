using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Business.Abstract;
using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using InvestmentOperations.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IHttpContextAccessor = Microsoft.AspNetCore.Http.IHttpContextAccessor;
using InvestmentOperations.Entities.Enums;
using InvestmentOperations.Core.DataAccess;
using System.Threading.Tasks;

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

        public async Task<IResult> Add(AssetHolding assetHolding)
        {
            IResult result = ValidateAssetHolding(assetHolding);
            if (!result.Success)
            {
                return result;
            }

            result = await CheckUserExists(assetHolding.UserId);
            if (!result.Success)
            {
                return result;
            }

            result = await CheckAssetExisting(assetHolding.AssetId);
            if (!result.Success)
            {
                return result;
            }

            var existingAssetHolding = await _assetHoldingDal.GetAsync(a => a.UserId == assetHolding.UserId && a.AssetId == assetHolding.AssetId);
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

        public async Task<IResult> Deposit(int userId, decimal amount)
        {
            var tlAsset = (await _assetService.GetAll()).Data?.FirstOrDefault(a => a.AssetCode == "TL");
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

            return await Add(assetHolding);
        }

        public async Task<IResult> Delete(int id)
        {
            var assetHolding = await _assetHoldingDal.GetAsync(a => a.AssetHoldingId == id);
            if (assetHolding == null)
            {
                return new ErrorResult("Asset holding not found.");
            }

            _assetHoldingDal.Delete(assetHolding);
            _unitOfWork.SaveChanges();
            LogAction("AssetHoldingDeleted", $"AssetHoldingId: {id}");
            return new SuccessResult("Asset holding deleted successfully.");
        }

        public async Task<IDataResult<List<AssetHolding>>> GetByUserId(int userId)
        {
            var assetHoldings = await _assetHoldingDal.GetAllAsync(a => a.UserId == userId);
            LogAction("AssetHoldingsListedByUser", $"UserId: {userId}");
            return new SuccessDataResult<List<AssetHolding>>(assetHoldings, "Asset holdings listed.");
        }

        public async Task<IDataResult<List<AssetHoldingDto>>> GetAllDetailed()
        {
            var assetHoldings = await _assetHoldingDal.GetAllAsync();
            var dtos = new List<AssetHoldingDto>();
            foreach (var assetHolding in assetHoldings)
            {
                dtos.Add(await MapToDto(assetHolding));
            }

            LogAction("AssetHoldingsListed", $"Count: {dtos.Count}");

            return new SuccessDataResult<List<AssetHoldingDto>>(dtos, "Asset holdings listed.");
        }

        public async Task<IDataResult<AssetHoldingDto>> GetByIdDetailed(int id)
        {
            var assetHolding = await _assetHoldingDal.GetAsync(a => a.AssetHoldingId == id);
            if (assetHolding == null)
            {
                return new ErrorDataResult<AssetHoldingDto>("Asset holding not found.");
            }
            LogAction("AssetHoldingViewed", $"AssetHoldingId: {id}");

            return new SuccessDataResult<AssetHoldingDto>(await MapToDto(assetHolding), "Asset holding found.");
        }

        public async Task<IDataResult<List<AssetHoldingDto>>> GetByUserIdDetailed(int userId)
        {
            var result = await GetByUserId(userId);
            var dtos = new List<AssetHoldingDto>();
            foreach (var assetHolding in result.Data)
            {
                dtos.Add(await MapToDto(assetHolding));
            }
            return new SuccessDataResult<List<AssetHoldingDto>>(dtos, "Asset holdings listed.");
        }

        public async Task<IResult> Update(AssetHolding assetHolding)
        {
            var existingAssetHolding = await _assetHoldingDal.GetAsync(a => a.AssetHoldingId == assetHolding.AssetHoldingId);
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
        private async Task<AssetHoldingDto> MapToDto(AssetHolding assetHolding)
        {
            var asset = (await _assetService.GetById(assetHolding.AssetId)).Data;
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

        private async Task<IResult> CheckUserExists(int userId)
        {
            var user = await _userService.GetById(userId);
            if (!user.Success)
            {
                return new ErrorResult("User not found.");
            }
            return new SuccessResult();
        }

        private async Task<IResult> CheckAssetExisting(int assetId)
        {
            var asset = await _assetService.GetById(assetId);
            if (!asset.Success)
            {
                return new ErrorResult("Asset not found.");
            }
            return new SuccessResult();
        }

        #endregion

    }
}
