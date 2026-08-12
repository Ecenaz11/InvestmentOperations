using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Business.Abstract;
using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using IHttpContextAccessor = Microsoft.AspNetCore.Http.IHttpContextAccessor;
using InvestmentOperations.Entities.Enums;
using InvestmentOperations.Core.DataAccess;
using InvestmentOperations.Core.Caching;

namespace InvestmentOperations.Business.Concrete
{
    public class AssetManager : IAssetService
    {
        private readonly IAssetDal _assetDal;
        private readonly ICacheService _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogService _logService;
        private readonly IUnitOfWork _unitOfWork;
        public AssetManager(IAssetDal assetDal, ICacheService cache, IHttpContextAccessor httpContectAccessor, ILogService logService,IUnitOfWork unitOfWork)
        {
            _assetDal = assetDal;
            _cache = cache;
            _httpContextAccessor = httpContectAccessor;
            _logService = logService;
            _unitOfWork = unitOfWork;
        }
        public async Task<IResult> Add(Asset asset)
        {
            IResult result = ValidateAsset(asset);
            if (!result.Success)
            {
                return result;
            }

            PrepareAsset(asset);


            result = ValidateAssetType(asset);
            if (!result.Success)
            {
                return result;
            }

            result = await CheckDuplicateAssetCode(asset.AssetCode);
            if (!result.Success)
            {
                return result;
            }

            result = await CheckDuplicateAssetName(asset.AssetName, asset.AssetId);
            if (!result.Success)

                return result;


            _assetDal.Add(asset);

            await  _cache.RemoveAsync("assets:all");

            LogAction("AssetAdded", $"AssetCode: {asset.AssetCode}, AssetName: {asset.AssetName}");
            _unitOfWork.SaveChanges();

            return new SuccessResult("Asset added successfully.");
        }

        public  async Task<IResult> Delete(int id)
        {
            var asset = await _assetDal.GetAsync(a => a.AssetId == id);

            if (asset == null)
            {
                return new ErrorResult("Asset not found.");
            }
            _assetDal.Delete(asset);

            await _cache.RemoveAsync($"assets:{id}");
            await _cache.RemoveAsync("assets:all");

            LogAction("AssetDeleted", $"AssetId: {id}");
            _unitOfWork.SaveChanges();


            return new SuccessResult("Asset deleted successfully.");

        }
        public async Task<IDataResult<Asset>> GetById(int id)
        {
            Asset asset = await _cache.GetOrCreateAsync($"asset:{id}",
                async cancellationToken => await _assetDal.GetAsync(a => a.AssetId == id),
                TimeSpan.FromHours(1));

            if (asset == null)
            {
                return new ErrorDataResult<Asset>("Asset not found.");
            }
            LogAction("AssetViewed", $"AssetId: {id}");
            return new SuccessDataResult<Asset>(asset, "Asset found.");
        }
        public async Task<IDataResult<List<Asset>>> GetAll()
        {
            List<Asset> assets = await _cache.GetOrCreateAsync(
                "assets:all",
                async cancellationToken => await _assetDal.GetAllAsync(),
                TimeSpan.FromHours(1));

            LogAction("AssetsListed", $"Count: {assets.Count}");

            return new SuccessDataResult<List<Asset>>(assets, "Assets listed.");
        }

        public  async Task<IResult> Update(Asset asset)
        {
            var existingAsset = await _assetDal.GetAsync(a => a.AssetId == asset.AssetId);
            if (existingAsset == null)
            {
                return new ErrorResult("Asset not found.");
            }

            PrepareAsset(asset);

            IResult result = ValidateAsset(asset);
            if (!result.Success)
            {
                return result;
            }

            result = ValidateAssetType(asset);
            if (!result.Success)
            {
                return result;
            }

            result = await CheckDuplicateAssetCode(asset.AssetCode, asset.AssetId);
            if (!result.Success)
                return result;


            result = await CheckDuplicateAssetName(asset.AssetName, asset.AssetId);
            if (!result.Success)
                return result;

            _assetDal.Update(asset);

            await _cache.RemoveAsync($"asset:{asset.AssetId}");
           await  _cache.RemoveAsync("assets:all");

            LogAction("AssetUpdated", $"AssetId: {asset.AssetId}, AssetCode: {asset.AssetCode}, AssetName: {asset.AssetName}");

             _unitOfWork.SaveChanges();


            return new SuccessResult("Asset updated successfully.");
        }

         private void LogAction (string action, string details)
        {
            var userIdClaim= _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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

        private IResult ValidateAsset(Asset asset)
        {

            if (asset == null)
            {
                return new ErrorResult("Asset cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(asset.AssetName))
            {
                return new ErrorResult("Asset name cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(asset.AssetCode))
            {
                return new ErrorResult("Asset Code cannot be empty.");
            }

            return new SuccessResult();
        }
        private void PrepareAsset(Asset asset)
        {
            asset.AssetName = asset.AssetName.Trim().ToUpperInvariant();
            asset.AssetCode = asset.AssetCode.Trim().ToUpperInvariant();
            asset.AssetType = asset.AssetType.Trim().ToUpperInvariant();
        }
        private async Task<IResult> CheckDuplicateAssetCode(string assetCode, int excludeAssetId = 0)
        {
            if (string.IsNullOrWhiteSpace(assetCode))
            {
                return new ErrorResult("Asset Code cannot be empty");

            }
            var allAssets = await _assetDal.GetAllAsync();
            if (allAssets != null && allAssets.Count > 0)
            {
                bool isDuplicate = allAssets.Any(a => a.AssetId != excludeAssetId &&
                !string.IsNullOrWhiteSpace(a.AssetCode) &&
                string.Equals(a.AssetCode.Trim(), assetCode.Trim(), StringComparison.OrdinalIgnoreCase));

                if (isDuplicate)
                {
                    return new ErrorResult("This AssetCode already exists.");
                }
            }
            return new SuccessResult();
        }
        private async Task<IResult> CheckDuplicateAssetName(string assetName, int excludeAssetId = 0)
        {
            if (string.IsNullOrWhiteSpace(assetName))
            {
                return new ErrorResult("Asset Name cannot be empty");
            }

            var allAssets = await _assetDal.GetAllAsync();
            if (allAssets != null && allAssets.Count > 0)
            {
                bool isDuplicate = allAssets.Any(a => a.AssetId != excludeAssetId &&
                !string.IsNullOrWhiteSpace(a.AssetName) &&
                string.Equals(a.AssetName.Trim(), assetName.Trim(), StringComparison.OrdinalIgnoreCase));

                if (isDuplicate)
                {
                    return new ErrorResult("This Asset Name already exists.");
                }
            }
            return new SuccessResult();
        }
        private IResult ValidateAssetType(Asset asset)
        {
            if (string.IsNullOrWhiteSpace(asset.AssetType))
            {
                return new ErrorResult("Asset Type cannot be empty");
            }

            bool isPreciousMetal = string.Equals(asset.AssetType.Trim(), "PRECIOUSMETAL", StringComparison.OrdinalIgnoreCase);
            bool isCurrency = string.Equals(asset.AssetType.Trim(), "CURRENCY", StringComparison.OrdinalIgnoreCase);

            if (!isPreciousMetal && !isCurrency)
            {
                return new ErrorResult("Invalid Asset Type. Only PRECIOUSMETAL OR CURRENCY are allowed.");
            }

            return new SuccessResult();
        }

        #endregion


    }

}
