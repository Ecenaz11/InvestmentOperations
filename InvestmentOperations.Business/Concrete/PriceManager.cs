using InvestmentOperation.Core.Utilities.Results;
using InvestmentOperations.Business.Abstract;
using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Text;

namespace InvestmentOperations.Business.Concrete
{
    public class PriceManager : IPriceService
    {
        private readonly IPriceDal _priceDal;
        private readonly IAssetDal _assetDal;
        public PriceManager(IPriceDal priceDal, IAssetDal assetDal)
        {
            _priceDal = priceDal;
            _assetDal = assetDal;

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
           
            return new SuccessResult("Price deleted successfully.");
        }

        public IDataResult<List<Price>> GetAll()
        {
            return new SuccessDataResult<List<Price>>
                (
                _priceDal.GetAll(), "Prices listed."
                );
        }

        public IDataResult<Price> GetById(int id)
        {
            var price = _priceDal.Get(p => p.PriceId == id);
            if (price == null)
            {
                return new ErrorDataResult<Price>("Price not found.");
            }
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
            return new SuccessResult("Price updated successfully.");
        }
       
        
        
            
            
            #region Validation Methods
                private IResult ValidatePrice(Price price)
        {
            if(price ==null)
            {
                return new ErrorResult("Price cannot be empty");
            }

            if(price.AssetId <= 0 )
            {
                return new ErrorResult("Invalid Asset.");
            }
            
            return new SuccessResult();
        }


        private void PreparePrice(Price price)
        {
            price.UpdatedAt = DateTime.Now;
           
        }

        private IResult ValidateCurrentPrice(Price price)
        {
            if(price.CurrentPrice <= 0 )
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
