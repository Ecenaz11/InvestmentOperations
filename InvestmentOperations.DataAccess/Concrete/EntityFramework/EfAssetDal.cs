using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class EfAssetDal : IAssetDal
    {
        private readonly InvestmentContext _context;
        public EfAssetDal(InvestmentContext context)
        {
            _context = context;
        }
        public void Add(Asset asset)
        {
            _context.Assets.Add(asset);
            _context.SaveChanges();
        }

        public void Delete(Asset asset)
        {
            _context.Assets.Remove(asset);
            _context.SaveChanges();
        }

        public Asset Get(Expression<Func<Asset, bool>> filter)
        {
            var asset = _context.Assets.FirstOrDefault(filter);
            return asset;
        }

        public List<Asset> GetAll(Expression<Func<Asset, bool>> filter = null)
        {
            if (filter == null)
            {
                return _context.Assets.ToList();
            }
            else
            {
                return _context.Assets.Where(filter).ToList();
            }
        }

        public void Update(Asset asset)
        {
            var tracked = _context.ChangeTracker.Entries<Asset>().FirstOrDefault(e => e.Entity.AssetId == asset.AssetId);
            if (tracked != null)
            {
                tracked.State = EntityState.Detached;
            }
            _context.Assets.Update(asset);
            _context.SaveChanges();
        }
    }
}
