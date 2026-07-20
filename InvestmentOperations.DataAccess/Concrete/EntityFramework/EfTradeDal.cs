using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class EfTradeDal : ITradeDal
    {
        private readonly InvestmentContext _context;
        public EfTradeDal(InvestmentContext context)
        {
            _context = context;
        }
        public void Add(Trade trade)
        {
           _context.Trades.Add(trade);
           _context.SaveChanges();    
        }

        public void Delete(Trade trade)
        {
           _context.Trades.Remove(trade);
           _context.SaveChanges();
        }

        public Trade Get(Expression<Func<Trade, bool>> filter)
        {
            var trade = _context.Trades.FirstOrDefault(filter);
            return trade;
        }

        public List<Trade> GetAll(Expression<Func<Trade, bool>> filter = null)
        {
            if (filter==null)
            {
                return _context.Trades.ToList();
            }
            else
            {
                return _context.Trades.Where(filter).ToList();
            }
        }

        public void Update(Trade trade)
        {
            var tracked = _context.ChangeTracker.Entries<Trade>().FirstOrDefault(e => e.Entity.TradeId == trade.TradeId);
            if (tracked != null)
            {
                tracked.State = EntityState.Detached;
            }
            _context.Trades.Update(trade);
            _context.SaveChanges();
        }
    }
}
