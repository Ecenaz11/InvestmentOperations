using InvestmentOperation.Core.Utilities.Results;
using InvestmentOperations.Business.Abstract;
using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace InvestmentOperations.Business.Concrete
{
    public class AssetManager : IAssetService 
    {
        private readonly IAssetDal _assetDal;
        public AssetManager(IAssetDal assetDal)
        {
            _assetDal = assetDal;
        }

        public IResult Add(Asset asset)
        {
          IResult result  = ValidateAsset(asset);
            if(!result.Success)
            {
                return result;
            }

            PrepareAsset( asset);
           
           
            result = ValidateAssetType(asset);
            if(!result.Success)
            {
                return result;
            }


             result = CheckDuplicateAssetCode(asset.AssetCode);
            if (!result.Success)
            {
                return result;
            }

            result = CheckDuplicateAssetName(asset.AssetName, asset.AssetId);
            if (!result.Success)
                
                return result;
        
            
            _assetDal.Add(asset);
            return new SuccessResult("Asset added successfully.");
        }

        public IResult Delete(int id)
        {
            var asset = _assetDal.Get(a =>a.AssetId == id);
           
           if(asset==null)
            {
                return new ErrorResult("Asset not found.");
            }
           
            _assetDal.Delete(asset);
          
            
            return new SuccessResult("Asset deleted successfully.");

        }

        public IDataResult<Asset> GetById(int id)
        {
            var asset = _assetDal.Get(a => a.AssetId == id);
            if(asset==null)
            {
                return new ErrorDataResult<Asset>("Asset not found.");
            }
            return new SuccessDataResult<Asset>(asset);
        }


        public IDataResult<List<Asset>> GetAll()
        {
            return new SuccessDataResult<List<Asset>>
                (
                _assetDal.GetAll(), "Assets listed."
                );
        }

        public IResult Update(Asset asset)
        {

            var existingAsset = _assetDal.Get(a => a.AssetId == asset.AssetId);
            if(existingAsset==null)
            {
                return new ErrorResult("Asset not found.");
            }
           
             PrepareAsset(asset);
           
            IResult result = ValidateAsset(asset); 
            if(!result.Success )
            {
                return result;
            }
           
            result = ValidateAssetType(asset);
            if(!result.Success)
            {
                return result;
            }
          
            result = CheckDuplicateAssetCode(asset.AssetCode, asset.AssetId);
            if (!result.Success)
                return result;
           
            
           
           result = CheckDuplicateAssetName(asset.AssetName, asset.AssetId);
            if (!result.Success)
                return result;
           
            _assetDal.Update(asset);
            return new SuccessResult("Asset updated successfully.");
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

        
        private IResult CheckDuplicateAssetCode(string assetCode, int excludeAssetId = 0)
        {
            if (string.IsNullOrWhiteSpace(assetCode))
            {
                return new ErrorResult("Asset Code cannot be empty") ;
  
            }
            

            var allAssets = _assetDal.GetAll();
            if (allAssets != null && allAssets.Count > 0 )
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

        private IResult CheckDuplicateAssetName(string assetName, int excludeAssetId = 0)
        {
            if (string.IsNullOrWhiteSpace(assetName))
            {
                return new ErrorResult("Asset Name cannot be empty");
            }

            var allAssets = _assetDal.GetAll();
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

            if(!isPreciousMetal && !isCurrency)
            {
               return new ErrorResult("Invalid Asset Type. Only PRECIOUSMETAL OR CURRENCY are allowed.");
            }

            return new SuccessResult();
        }
        
        #endregion
        

    }

}
