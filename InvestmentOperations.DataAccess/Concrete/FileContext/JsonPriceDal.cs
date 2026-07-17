using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;

namespace InvestmentOperations.DataAccess.Concrete.FileContext
{
    public class JsonPriceDal : IPriceDal
    {
        private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "prices.txt");
        private List<Price> ReadFromFile()
        {
            if (!File.Exists(_filePath))
                return new List<Price>();
            string jsonText = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Price>>(jsonText) ?? new List<Price>();
        }
        private void WriteToFile(List<Price> priceList)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string updatedJson = JsonSerializer.Serialize(priceList, options);
            File.WriteAllText(_filePath, updatedJson);
        }
        public void Add(Price price)
        {
            var List = ReadFromFile();
            price.PriceId = List.Count > 0 ? List.Max(p => p.PriceId) + 1 : 1;
            List.Add(price);
            WriteToFile(List);
        }

        public void Delete(Price price)
        {
            var list = ReadFromFile();
            var priceToDelete = list.FirstOrDefault(p => p.PriceId == price.PriceId);
            if (priceToDelete != null)
            {
                list.Remove(priceToDelete);
                WriteToFile(list);
            }
                
            
        }

        public Price Get(Expression<Func<Price, bool>> filter)
        {
            var list = ReadFromFile();
            return list.AsQueryable().FirstOrDefault(filter);
        }

        public List<Price> GetAll(Expression<Func<Price, bool>> filter = null)
        {
            var list = ReadFromFile();
            if(filter==null)
            {
                return list;
            }
            else
            {
                return list.AsQueryable().Where(filter).ToList();
            }
        }

        public void Update(Price price)
        {
            var list = ReadFromFile();
            var priceToUpdate = list.FirstOrDefault(p => p.PriceId == price.PriceId);
            if (priceToUpdate != null)
            {
                priceToUpdate.CurrentPrice = price.CurrentPrice;
                priceToUpdate.UpdatedAt = price.UpdatedAt;

                WriteToFile(list);
            }
        }
    }
}
