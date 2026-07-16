using InvestmentOperations.DataAccess.Concrete.FileContext;
using InvestmentOperations.Entities.Concrete;

var assetDal = new JsonAssetDal();

assetDal.Add(new Asset
{
    AssetName="bakir2",
    AssetCode="bkr",
    AssetType="bi sey"
});

var assets = assetDal.GetAll();
foreach(var asset in assets)
{
    Console.WriteLine($"{asset.AssetId} - {asset.AssetName}");
}