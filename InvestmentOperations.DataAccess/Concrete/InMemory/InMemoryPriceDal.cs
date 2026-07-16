using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace InvestmentOperations.DataAccess.Concrete.InMemory
{
    public class InMemoryPriceDal : IPriceDal
    {
        List<Price> _prices;
        public InMemoryPriceDal()
        {
            _prices = new List<Price>
            {
                new Price {PriceId=1, AssetId=1, CurrentPrice=6281.50m, UpdatedAt=DateTime.Now},
                new Price {PriceId=2, AssetId=2, CurrentPrice=94.05m, UpdatedAt=DateTime.Now},
                new Price {PriceId=3, AssetId=3, CurrentPrice=2521.20m, UpdatedAt=DateTime.Now},
                new Price {PriceId=4, AssetId=4, CurrentPrice=1949.60m, UpdatedAt=DateTime.Now},
                new Price {PriceId=5, AssetId=5, CurrentPrice=46.81m, UpdatedAt=DateTime.Now},
                new Price {PriceId=6, AssetId=6, CurrentPrice=53.61m, UpdatedAt=DateTime.Now},
                new Price {PriceId=7, AssetId=7, CurrentPrice=62.55m, UpdatedAt=DateTime.Now}
            };
        }

        public void Add(Price price)
        {
            _prices.Add(price);
        }

        public void Delete(Price price)
        {
            Price assetToDelete = _prices.SingleOrDefault(p => p.PriceId == price.PriceId);
            _prices.Remove(assetToDelete);
        }

        public Price Get(Expression<Func<Price, bool>> filter)
        {
            return _prices.SingleOrDefault(filter.Compile());
        }

        public List<Price> GetAll(Expression<Func<Price, bool>> filter = null)
        {
            return _prices;
        }

        public void Update(Price price)
        {
            Price priceToUpdate = _prices.SingleOrDefault(p => p.PriceId == price.PriceId);

            priceToUpdate.CurrentPrice = price.CurrentPrice;
            priceToUpdate.UpdatedAt = price.UpdatedAt;
           
        }
    }
}
