using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;

namespace InvestmentOperations.DataAccess.Concrete.FileContext
{
    public class JsonTradeDal : ITradeDal
    {
        private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "trades.txt");
        private List<Trade> ReadFromFile()
        {
            if (!File.Exists(_filePath))
                return new List<Trade>();
            string jsonText = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Trade>>(jsonText) ?? new List<Trade>();
        }
        private void WriteToFile(List<Trade> tradeList) 
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string updatedJson = JsonSerializer.Serialize(tradeList, options);
            File.WriteAllText(_filePath, updatedJson);
        }
        public void Add(Trade trade)
        {
            var list = ReadFromFile();
            trade.TradeId = list.Count > 0 ? list.Max(t => t.TradeId) + 1 : 1;
            list.Add(trade);
            WriteToFile(list);
        }

        public void Delete(Trade trade)
        {
            var list = ReadFromFile();
            var tradeToDelete = list.FirstOrDefault(t => t.TradeId == trade.TradeId);
            if (tradeToDelete!= null)
            {
                list.Remove(tradeToDelete);
                WriteToFile(list);
            }
        }

        public Trade Get(Expression<Func<Trade, bool>> filter)
        {
            var list = ReadFromFile();
            return list.AsQueryable().FirstOrDefault(filter);
        }

        public List<Trade> GetAll(Expression<Func<Trade, bool>> filter = null)
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

        public void Update(Trade trade)
        {
            var list = ReadFromFile();
            var tradeToUpdate = list.FirstOrDefault(t => t.TradeId == trade.TradeId);
            if(tradeToUpdate!=null)
            {
                tradeToUpdate.TradeType = trade.TradeType;
                tradeToUpdate.TradeDate = trade.TradeDate;
                tradeToUpdate.Quantity = trade.Quantity;
                tradeToUpdate.UnitPrice = trade.UnitPrice;
                tradeToUpdate.TotalPrice = trade.TotalPrice;

                WriteToFile(list);
            }
            
        }
    }
}
