using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class EfBalanceDal : IBalanceDal
    {
        private readonly InvestmentContext _context;
        public EfBalanceDal(InvestmentContext context)
        {
            _context = context;
        }
        public void Add(Balance balance)
        {
            _context.Balances.Add(balance);
            _context.SaveChanges();
        }

        public void Delete(Balance balance)
        {
            _context.Balances.Remove(balance);
            _context.SaveChanges();
        }

        public Balance Get(Expression<Func<Balance, bool>> filter)
        {
            var balance = _context.Balances.FirstOrDefault(filter);
            return balance;
        }

        public List<Balance> GetAll(Expression<Func<Balance, bool>> filter = null)
        {
            if (filter == null)
            {
                return _context.Balances.ToList();
            }
            else
            {
                return _context.Balances.Where(filter).ToList();
            }
        }

        public void Update(Balance balance)
        {
            var tracked = _context.ChangeTracker.Entries<Balance>().FirstOrDefault(e => e.Entity.BalanceId == balance.BalanceId);
            if (tracked != null)
            {
                tracked.State = EntityState.Detached;
            }
            _context.Balances.Update(balance);
            _context.SaveChanges();
        }
    }
}
