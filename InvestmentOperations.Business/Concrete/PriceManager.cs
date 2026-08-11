using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Business.Abstract;
using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Text;
using InvestmentOperations.Entities.Dtos;
using Microsoft.VisualBasic;
using System.Net.Http.Headers;
using System.Security;
using System.Security.Claims;
using IHttpContextAccessor = Microsoft.AspNetCore.Http.IHttpContextAccessor;
using InvestmentOperations.Entities.Enums;

namespace InvestmentOperations.Business.Concrete
{
    public class PriceManager : IPriceService
    {
        private readonly IPriceDal _priceDal;
        private readonly IAssetDal _assetDal;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogService _logService;
        public PriceManager(IPriceDal priceDal, IAssetDal assetDal, IHttpContextAccessor httpContextAccessor, ILogService logService)
        {
            _priceDal = priceDal;
            _assetDal = assetDal;
            _httpContextAccessor = httpContextAccessor;
            _logService = logService;

        }
        public IResult Add(Price price)
        {
            PreparePrice(price);
            IResult result = ValidatePrice(price);
            if (!result.Success)
            {
                return result;
            }
            result = ValidateCurrentPrice(price);
            if (!result.Success)
            {
                return result;
            }

            result = CheckExistingAsset(price.AssetId);
            if (!result.Success)
            {
                return result;
            }

            result = CheckDuplicatePrice(price.AssetId);
            if (!result.Success)
            {
                return result;
            }
            _priceDal.Add(price);

            LogAction("PriceAdded", $"AssetId: {price.AssetId}, CurrentPrice: {price.CurrentPrice}");

            return new SuccessResult("Price added successfully.");
        }
        public IResult Delete(int id)
        {
            var price = _priceDal.Get(p => p.PriceId == id);
            if (price == null)
            {
                return new ErrorResult("Price not found.");
            }
            _priceDal.Delete(price);

            LogAction("PriceDeleted", $"PriceId: {id}");

            return new SuccessResult("Price deleted successfully.");
        }
        public IDataResult<PriceDto> GetById(int id)
        {
            var price = _priceDal.Get(p => p.PriceId == id);
            if (price == null)
            {
                return new ErrorDataResult<PriceDto>("Price not found");
            }
            LogAction("PriceViewed", $"PriceId: {id}");
            
            return new SuccessDataResult<PriceDto>(MapToDto(price), "Price found.");
        }
        public IDataResult<List<PriceDto>> GetAll()
        {
            var prices = _priceDal.GetAll();
            var dtos = prices.Select(MapToDto).ToList();

            LogAction("PricesListed", $"Count: {dtos.Count}");

            return new SuccessDataResult<List<PriceDto>>(dtos, "Prices listed.");
        }
        public IDataResult<Price> GetByAssetId(int assetId)
        {
            var price = _priceDal.Get(p => p.AssetId == assetId);
            if (price == null)
            {
                return new ErrorDataResult<Price>("Price not found for this asset.");
            }
            LogAction("PriceViewedByAsset", $"AssetId: {assetId}");

            return new SuccessDataResult<Price>(price, "Price found.");
        }
        public IResult Update(Price price)
        {
            var existingPrice = _priceDal.Get(p => p.PriceId == price.PriceId);
            if (existingPrice == null)
            {
                return new ErrorResult("Price not found.");
            }
            IResult result = ValidatePrice(price);
            if (!result.Success)
            {
                return result;
            }

            result = ValidateCurrentPrice(price);
            if (!result.Success)
            {
                return result;
            }

            result = CheckExistingAsset(price.AssetId);
            if (!result.Success)
            {
                return result;
            }

            PreparePrice(price);

            _priceDal.Update(price);

            LogAction("PriceUpdated", $"PriceId: {price.PriceId}, AssetId: {price.AssetId}, CurrentPrice: {price.CurrentPrice}");

            return new SuccessResult("Price updated successfully.");
        }

        private PriceDto MapToDto(Price price)
        {
            var asset = _assetDal.Get(a => a.AssetId == price.AssetId);
            return new PriceDto
            {
                PriceId = price.PriceId,
                AssetName = asset?.AssetName,
                AssetCode = asset?.AssetCode,
                AssetType = asset?.AssetType,
                CurrentPrice = price.CurrentPrice,
                UpdatedAt = price.UpdatedAt
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
        private IResult ValidatePrice(Price price)
        {
            if (price == null)
            {
                return new ErrorResult("Price cannot be empty");
            }

            if (price.AssetId <= 0)
            {
                return new ErrorResult("Invalid Asset.");
            }

            return new SuccessResult();
        }


        private void PreparePrice(Price price)
        {
            price.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        }

        private IResult ValidateCurrentPrice(Price price)
        {
            if (price.CurrentPrice <= 0)
            {
                return new ErrorResult(" Current Price must be greater than zero.");
            }
            return new SuccessResult();
        }


        private IResult CheckDuplicatePrice(int assetId)
        {
            var price = _priceDal.Get(p => p.AssetId == assetId);
            if (price != null)
            {
                return new ErrorResult("This Asset already has a price.");
            }

            return new SuccessResult();
        }

        private IResult CheckExistingAsset(int assetId)
        {
            var asset = _assetDal.Get(a => a.AssetId == assetId);
            if (asset == null)
            {
                return new ErrorResult("Asset not found.");
            }
            return new SuccessResult();
        }

        #endregion

    }
}
