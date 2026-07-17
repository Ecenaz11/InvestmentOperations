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

           // ValidateAssetCodeAndType(asset);

             result = CheckDuplicateAssetCode(asset.AssetCode);
            if (!result.Success)
            {
                return result;
            }
            
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
          
            // ValidateAssetCodeAndType(asset);

           result =  ValidateAssetCodeChange(existingAsset,asset);
            if(!result.Success)
            {
                return result;
            }

            result = CheckDuplicateAssetCode(asset.AssetCode);
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

        
        private IResult CheckDuplicateAssetCode(string assetCode)
        {
            if (string.IsNullOrWhiteSpace(assetCode))
            {
                return new ErrorResult("Asset Code cannot be empty") ;
  
            }
            

            var allAssets = _assetDal.GetAll();
            if (allAssets != null && allAssets.Count > 0 )
            {
                bool isDuplicate = allAssets.Any(a =>
                !string.IsNullOrWhiteSpace(a.AssetCode) &&
                string.Equals(a.AssetCode.Trim(), assetCode.Trim(), StringComparison.OrdinalIgnoreCase));

                if (isDuplicate)
                {
                    return new ErrorResult("This AssetCode already exists.");
                }

            }

            return new SuccessResult(); 
        }
/*
        private IResult CheckDuplicateAssetName(string assetName)
        {

        }
*/

        private IResult ValidateAssetCodeChange(Asset existingAsset, Asset updatedAsset)
        {

            if (existingAsset.AssetCode!=updatedAsset.AssetCode)
            {
                CheckDuplicateAssetCode(updatedAsset.AssetCode);

            }
            return new SuccessResult();


            
        }
        public static bool IsNullOrWhiteSpace1([NotNullWhen(false)] string? value)
        {
            if (value == null) return false;

            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i])) return false;
            }

            return true;
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
        /*
        private void ValidateAssetCodeAndType(Asset asset)
        {
            if(asset.AssetType=="PRECIOUSMETAL")
            {
                if (asset.AssetCode != "XAU" &&
                    asset.AssetCode != "XAG" &&
                    asset.AssetCode != "XPT" &&
                    asset.AssetCode != "XPD")
                {
                    throw new Exception("This Asset Code is invalid for the Precious Metal.");
                }

                if (asset.AssetType=="CURRENCY")
                {
                    if (asset.AssetCode != "USD" &&
                        asset.AssetCode != "EUR" &&
                        asset.AssetCode != "GBP")
                    {
                        throw new Exception("This Asset Code is invalid for the Currency. ");
                    }
                }
            }

        }
        */
        #endregion
        

    }

}
