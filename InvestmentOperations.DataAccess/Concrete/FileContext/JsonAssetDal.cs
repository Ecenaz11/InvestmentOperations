using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;
using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System.Linq.Expressions;
using System.ComponentModel.Design;
using System.Linq;



namespace InvestmentOperations.DataAccess.Concrete.FileContext
{
    public class JsonAssetDal : IAssetDal
    {
        private readonly string _filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "assets.txt");
        private List<Asset> ReadFromFile() 
        {
            if (!File.Exists(_filepath))  
              
                return new List<Asset>();

            string jsonText = File.ReadAllText(_filepath);
           
            if(string.IsNullOrWhiteSpace(jsonText))
            {
                return new List<Asset>();
            }

            var deserializeOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<Asset>>(jsonText) ?? new List<Asset>();
        }
        private void WriteToFile(List<Asset> assetList)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string updatedJson = JsonSerializer.Serialize(assetList, options);
            File.WriteAllText(_filepath, updatedJson);
        }

        public void Add(Asset asset)
        {
            var list = ReadFromFile();
            asset.AssetId = list.Count >0?list.Max(a => a.AssetId) + 1 : 1;  
            list.Add(asset);
            WriteToFile(list);
        }

        public void Delete(Asset asset)
        {
            var list = ReadFromFile();
            var assetToDelete = list.FirstOrDefault(a => a.AssetId == asset.AssetId);
            if (assetToDelete != null)
            {
                list.Remove(assetToDelete);
                WriteToFile(list);
            }
        }

        public Asset Get(Expression<Func<Asset, bool>> filter)
        {
            var list = ReadFromFile();
            return list.AsQueryable().FirstOrDefault(filter);
        }

        public List<Asset> GetAll(Expression<Func<Asset, bool>> filter = null)
        {
            var list = ReadFromFile();

            if (filter == null)
            {
                return list;
            }
          
            else
            {
                return list.AsQueryable().Where(filter).ToList();

            }

        }

        public void Update(Asset asset)
        {
            var list = ReadFromFile();
            var assetToUpdate = list.FirstOrDefault(a => a.AssetId == asset.AssetId);
            if (assetToUpdate != null)
            {
                assetToUpdate.AssetName = asset.AssetName;
                assetToUpdate.AssetCode = asset.AssetCode;
                assetToUpdate.AssetType = asset.AssetType;
                WriteToFile(list);
            }
        }
    }
}
