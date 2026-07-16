using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace InvestmentOperations.DataAccess.Concrete.InMemory
{
    public class InMemoryAssetDal : IAssetDal
    {
        List<Asset> _assets;
        public InMemoryAssetDal()
        {
            _assets = new List<Asset>
            {
                new Asset {AssetId=1, AssetName = "Gold", AssetType = "PreciousMetal", AssetCode="XAU" },
                new Asset {AssetId=2, AssetName = "Silver", AssetType = "PreciousMetal", AssetCode="XAG" },
                new Asset {AssetId=3, AssetName = "Platinum", AssetType = "PreciousMetal", AssetCode="XPT" },
                new Asset {AssetId=4, AssetName = "Palladium", AssetType = "PreciousMetal", AssetCode="XPD" },
                new Asset {AssetId=5, AssetName = "Dollar", AssetType = "Currency", AssetCode="USD" },
                new Asset {AssetId=6, AssetName = "Euro", AssetType = "Currency", AssetCode="EUR" },
                new Asset {AssetId=7, AssetName = "Sterling", AssetType = "Currency", AssetCode="GBP" }
            }; 
        }

        public void Add(Asset asset)
        {
            _assets.Add(asset);
        }

        public void Delete(Asset asset)
        {
           Asset assetToDelete = _assets.SingleOrDefault(p => p.AssetId == asset.AssetId);
            _assets.Remove(assetToDelete);
        }

        public Asset Get(Expression<Func<Asset, bool>> filter)
        {
           return _assets.SingleOrDefault(filter.Compile());
        }

        public List<Asset> GetAll(Expression<Func<Asset, bool>> filter = null)
        {
            return _assets;
        }

        public void Update(Asset asset)
        {
            Asset assetToUpdate = _assets.SingleOrDefault(p => p.AssetId == asset.AssetId);
            assetToUpdate.AssetName = asset.AssetName;
            assetToUpdate.AssetType = asset.AssetType;
            assetToUpdate.AssetCode = asset.AssetCode;
        }
    }
}
