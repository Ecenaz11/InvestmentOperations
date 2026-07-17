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
            ValidatePrice(price);
            ValidateCurrentPrice(price);
            CheckExistingAsset(price.AssetId);
            //CheckDuplicatePrice(price.AssetId);
            _priceDal.Add(price);
            return new SuccessResult("Price added successfully.");
        }

        public IResult Delete(int id)
        {
            var price = GetExistingPrice(id);
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
            return new SuccessDataResult<Price>
                (
                GetExistingPrice(id), "Price found."
                );

        }

        public IResult Update(Price price)
        {
            var existingPrice = GetExistingPrice(price.PriceId);
            ValidatePrice(price);
            ValidateCurrentPrice(price);
            CheckExistingAsset(price.AssetId);
            PreparePrice(price);
            _priceDal.Update(price);
            return new SuccessResult("Price updated successfully.");
        }
       
        
        #region Validation Methods
        private void ValidatePrice(Price price)
        {
            if(price ==null)
            {
                throw new Exception("Price cannot be empty");
            }

            if(price.AssetId <= 0 )
            {
                throw new Exception("Invalid Asset.");
            }
            
        }


        private void PreparePrice(Price price)
        {
            price.UpdatedAt = DateTime.Now;
        }

        private void ValidateCurrentPrice(Price price)
        {
            if(price.CurrentPrice <= 0 )
            {
                throw new Exception("");
            }
        }

        private void CheckExistingAsset(int assetId)
        {
            var asset = _assetDal.Get(asset => asset.AssetId == assetId);
            if (asset ==null)
            {
                throw new Exception("Asset not found.");
            }
        }

        

        private void CheckDuplicatePrice(int assetId)
        {
            var price = _priceDal.Get(p => p.AssetId == assetId);
            if (price != null)
            {
                throw new Exception("This Asset already has a price.");
            }
                
        }

        #endregion

    }
}
