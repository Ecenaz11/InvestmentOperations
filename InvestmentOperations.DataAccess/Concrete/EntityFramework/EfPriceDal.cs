using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class EfPriceDal : IPriceDal
    {
        private readonly InvestmentContext _context;
        public EfPriceDal(InvestmentContext context)
        {
            _context=context;
        }
        public void Add(Price price)
        {
            _context.Prices.Add(price);
            _context.SaveChanges();
        }

        public void Delete(Price price)
        {
            _context.Prices.Remove(price);
            _context.SaveChanges();
        }

        public Price Get(Expression<Func<Price, bool>> filter)
        {
            var price = _context.Prices.FirstOrDefault(filter);
            return price;
        }

        public List<Price> GetAll(Expression<Func<Price, bool>> filter = null)
        {
            if (filter == null)
            {
                return _context.Prices.ToList();
            }
            else
            {
                return _context.Prices.Where(filter).ToList();
            }
        }

        public void Update(Price price)
        {
            var tracked = _context.ChangeTracker.Entries<Price>().FirstOrDefault(e => e.Entity.PriceId == price.PriceId);
            if (tracked != null)
            {
                tracked.State = EntityState.Detached;
            }
            _context.Prices.Update(price);
            _context.SaveChanges();
        }
    }
}
