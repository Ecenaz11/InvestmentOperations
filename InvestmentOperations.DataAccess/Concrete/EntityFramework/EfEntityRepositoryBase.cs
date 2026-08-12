using InvestmentOperations.Core.DataAccess;
using InvestmentOperations.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class EfEntityRepositoryBase<TEntity, TContext> : IEntityRepository<TEntity> 
    where TEntity : class, IEntity
    where TContext : DbContext
    {
        protected readonly TContext _context;

        public EfEntityRepositoryBase(TContext context)
        {
            _context = context;
        }

        public void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

       public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter)
        {
            var entity = await _context.Set<TEntity>().FirstOrDefaultAsync(filter);
           return entity;
        }

        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            if(filter==null)
            {
                return await _context.Set<TEntity>().ToListAsync();
            }
            else
            {
                return await _context.Set<TEntity>().Where(filter).ToListAsync();
            }
        }

        public void Update(TEntity entity)
        {
           var entry = _context.Entry(entity);

           if (entry.State == EntityState.Detached)
            {
                var primaryKey = _context.Model.FindEntityType(typeof(TEntity))!.FindPrimaryKey()!;
                var keyValues = primaryKey.Properties.Select(p=> entry.Property(p.Name).CurrentValue).ToArray();

                var trackedEntity = _context.Set<TEntity>().Find(keyValues);
                if(trackedEntity != null)
                {
                    _context.Entry(trackedEntity).State = EntityState.Detached;
                }

                _context.Set<TEntity>().Update(entity);
            }
        }
    }
}