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

namespace InvestmentOperations.Business.Concrete
{
    public class BalanceManager : IBalanceService
    {
        private readonly IBalanceDal _balanceDal;
        private readonly IAssetService _assetService;
        private readonly IUserService _userService;
        public BalanceManager(IBalanceDal balanceDal, IAssetService assetService, IUserService userService)
        {
            _balanceDal = balanceDal;
            _assetService = assetService;
            _userService = userService;
        }

        public IResult Add(Balance balance)
        {
            IResult result = ValidateBalance(balance);
            if (!result.Success)
            {
                return result;
            }

            result = CheckUserExists(balance.UserId);
            if (!result.Success)
            {
                return result;
            }

            result = CheckAssetExisting(balance.AssetId);
            if (!result.Success)
            {
                return result;  
            }

            var existingBalance = _balanceDal.Get(b => b.UserId == balance.UserId && b.AssetId == balance.AssetId);
            if (existingBalance != null)
            {
               existingBalance.Amount += balance.Amount;
                PrepareBalance(existingBalance);
                _balanceDal.Update(existingBalance);    
                return new SuccessResult("Balance updated successfully.");
               
            }

            PrepareBalance(balance);

            _balanceDal.Add(balance);
            return new SuccessResult("Balance added successfully.");
        }

        public IResult Deposit(int userId, decimal amount)
        {
            var tlAsset = _assetService.GetAll().Data?.FirstOrDefault(a => a.AssetCode == "TL");
            if (tlAsset == null)
            {
                return new ErrorResult("TL asset not found.");
            }

            var balance = new Balance
            {
                UserId = userId,
                AssetId = tlAsset.AssetId,
                Amount = amount
            };

            return Add(balance);
        }

        public IResult Delete(int id)
        {
            var balance = _balanceDal.Get(b => b.BalanceId == id);
            if (balance == null)
            {
                return new ErrorResult("Balance not found.");
            }

            _balanceDal.Delete(balance);
            return new SuccessResult("Balance deleted successfully.");
        }

        public IDataResult<List<Balance>> GetByUserId(int userId)
        {
            return new SuccessDataResult<List<Balance>>
                (
                _balanceDal.GetAll(b => b.UserId == userId), "Balances listed."
                );
        }

        public IDataResult<List<BalanceDto>> GetAllDetailed()
        {
            var balance = _balanceDal.GetAll();
            var dtos = balance.Select(MapToDto).ToList();
           
            return new SuccessDataResult<List<BalanceDto>>(dtos, "Balances listed.");
        }

        public IDataResult<BalanceDto> GetByIdDetailed(int id)
        {
            var balance = _balanceDal.Get(b => b.BalanceId == id);
            if(balance==null)
            {
                return new ErrorDataResult<BalanceDto>("Balance not found.");
            }
           
            return new SuccessDataResult<BalanceDto>(MapToDto(balance), "Balance found.");
        }

        public IDataResult<List<BalanceDto>> GetByUserIdDetailed(int userId)
        {
            var result = GetByUserId(userId);
            var dtos = result.Data.Select(MapToDto).ToList();
            return new SuccessDataResult<List<BalanceDto>>(dtos, "Balances listed.");
        }

        public IResult Update(Balance balance)
        {
            var existingBalance = _balanceDal.Get(b => b.BalanceId == balance.BalanceId);
            if (existingBalance == null)
            {
                return new ErrorResult("Balance not found.");
            }

            PrepareBalance(balance);

            IResult result = ValidateBalance(balance);
            if (!result.Success)
            {
                return result;
            }

            _balanceDal.Update(balance);
            return new SuccessResult("Balance updated successfully.");
        }


        private BalanceDto MapToDto(Balance balance)
        {
            var asset = _assetService.GetById(balance.AssetId).Data;
            return new BalanceDto
            {
                BalanceId = balance.BalanceId,
                UserId = balance.UserId,
                AssetId = balance.AssetId,
                AssetName = asset?.AssetName,
                AssetCode = asset?.AssetCode,
                AssetType = asset?.AssetType,
                Amount = balance.Amount
            };
        }


        #region Validation Methods

        private IResult ValidateBalance(Balance balance)
        {
            if (balance == null)
            {
                return new ErrorResult("Balance cannot be empty.");
            }

            if (balance.UserId <= 0)
            {
                return new ErrorResult("UserId must be greater than zero.");
            }

            if (balance.AssetId <= 0)
            {
                return new ErrorResult("AssetId must be greater than zero.");
            }

            if (balance.Amount < 0)
            {
                return new ErrorResult("Amount cannot be negative.");
            }

            return new SuccessResult();
        }

        private void PrepareBalance(Balance balance)
        {
            balance.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
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
