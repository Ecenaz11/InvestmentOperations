using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class EfUserDal : IUserDal
    {
        private readonly InvestmentContext _context;
        public EfUserDal(InvestmentContext context)
        {
            _context = context;
        }
        public void Add(User user)
        {
           _context.Users.Add(user);
           _context.SaveChanges();
        }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        public User Get(Expression<Func<User, bool>> filter)
        {
            var user = _context.Users.FirstOrDefault(filter);
            return user;
        }

        public List<User> GetAll(Expression<Func<User, bool>> filter = null)
        {
            if (filter == null)
            {
                return _context.Users.ToList();
            }
            else
            {
                return _context.Users.Where(filter).ToList();
            }
            
        }
    

        public void Update(User user)
        {
            var tracked = _context.ChangeTracker.Entries<User>().FirstOrDefault(e => e.Entity.UserId == user.UserId);
            if (tracked != null)
            {
                tracked.State = EntityState.Detached;
            }
            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
}
