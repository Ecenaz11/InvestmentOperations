using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class EfAssetHoldingDal : IAssetHoldingDal
    {
        private readonly InvestmentContext _context;
        public EfAssetHoldingDal(InvestmentContext context)
        {
            _context = context;
        }
        public void Add(AssetHolding assetHolding)
        {
            _context.AssetHoldings.Add(assetHolding);
        }

        public void Delete(AssetHolding assetHolding)
        {
            _context.AssetHoldings.Remove(assetHolding);
        }

        public AssetHolding Get(Expression<Func<AssetHolding, bool>> filter)
        {
            var assetHolding = _context.AssetHoldings.FirstOrDefault(filter);
            return assetHolding;
        }

        public List<AssetHolding> GetAll(Expression<Func<AssetHolding, bool>> filter = null)
        {
            if (filter == null)
            {
                return _context.AssetHoldings.ToList();
            }
            else
            {
                return _context.AssetHoldings.Where(filter).ToList();
            }
        }

        public void Update(AssetHolding assetHolding)
        {
            var tracked = _context.ChangeTracker.Entries<AssetHolding>().FirstOrDefault(e => e.Entity.AssetHoldingId == assetHolding.AssetHoldingId);
            if (tracked != null)
            {
                tracked.State = EntityState.Detached;
            }
            _context.AssetHoldings.Update(assetHolding);
        }
    }
}