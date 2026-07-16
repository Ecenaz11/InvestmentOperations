using InvestmentOperations.Business.Abstract;
using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
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

        public void Add(Asset asset)
        {
            ValidateAsset(asset);
            PrepareAsset(asset);
            ValidateAssetType(asset);
            ValidateAssetCodeAndType(asset);
            CheckDuplicateAssetCode(asset.AssetCode);
            _assetDal.Add(asset);
        }

        public void Delete(int id)
        {
            var asset = GetExistingAsset(id);
            _assetDal.Delete(asset);

        }

        public Asset GetById(int id)
        {
            return GetExistingAsset(id);

        }


        public List<Asset> GetAll()
        {
            return _assetDal.GetAll();
        }

        public void Update(Asset asset)
        {

            var existingAsset = GetExistingAsset(asset.AssetId);
           
            PrepareAsset(asset);
            ValidateAsset(asset); 
            ValidateAssetType(asset);
            ValidateAssetCodeAndType(asset);
            ValidateAssetCodeChange(existingAsset,asset);
            _assetDal.Update(asset);
        }


        #region Validation Methods
       
        private void ValidateAsset(Asset asset)
        {
            var englishCharacterPattern = @"^[a-zA-Z0-9\*$";

            if (asset == null)
            {
                throw new Exception("Asset cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(asset.AssetName))
            {
                throw new Exception("Asset name cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(asset.AssetCode))
            {
                throw new Exception("Asset Code cannot be empty.");
            }

            if(!Regex.IsMatch(asset.AssetName,englishCharacterPattern));
            {
                throw new Exception("Please use only English characters for names (No: ı, ş, ğ, ü, ö, ç).");
            }
        }
        private void PrepareAsset(Asset asset)
        {
            asset.AssetName = asset.AssetName.Trim().ToUpperInvariant();
            asset.AssetCode = asset.AssetCode.Trim().ToUpperInvariant();
            asset.AssetType = asset.AssetType.Trim().ToUpperInvariant();
        }

        private void CheckDuplicateAssetCode(string assetCode)
        {
            if (string.IsNullOrWhiteSpace(assetCode))
            {
                throw new Exception("Asset Code cannot be empty");
            }

            var allAsets = _assetDal.GetAll();
            if (allAsets != null && allAsets.Count > 0 )
            {
                bool isDuplicate = allAsets.Any(a =>
                !string.IsNullOrWhiteSpace(a.AssetCode) &&
                string.Equals(a.AssetCode.Trim(), assetCode.Trim(), StringComparison.OrdinalIgnoreCase));

                if (isDuplicate)
                {
                    throw new Exception("This AssetCode already exists.");
                }
            }
        }

        private Asset GetExistingAsset(int id)
        {
            var asset = _assetDal.GetAll()
                .FirstOrDefault(a => a.AssetId == id);
            if (asset==null)
            {
                throw new Exception("Asset not found.");
            }

            return asset;
        }

        private void ValidateAssetCodeChange(Asset existingAsset, Asset updatedAsset)
        {

            if (existingAsset.AssetCode!=updatedAsset.AssetCode)
            {
                CheckDuplicateAssetCode(updatedAsset.AssetCode);
            }
        }

        private void ValidateAssetType(Asset asset)
        {
           if (string.IsNullOrWhiteSpace(asset.AssetType))
            {
                throw new Exception("Asset Type cannot be empty");
            }

            bool isPreciousMetal = string.Equals(asset.AssetType.Trim(), "PRECIOUSMETAL", StringComparison.OrdinalIgnoreCase);
            bool isCurrency = string.Equals(asset.AssetType.Trim(), "CURRENCY", StringComparison.OrdinalIgnoreCase);

            if(!isPreciousMetal&& !isCurrency)
            {
                throw new Exception("Invalid Asset Type. Only PRECIOUSMETAL OR CURRENCY are allowed.");
            }
        }

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
                        asset.AssetCode != "XPD")
                    {
                        throw new Exception("This Asset Code is invalid for the Currency. ");
                    }
                }
            }

        }

        #endregion 


    }

}
