using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace InvestmentOperations.DataAccess.Concrete.InMemory
{
    public class InMemoryUserDal : IUserDal
    {
        List<User> _users;
        public InMemoryUserDal()
        {
            _users = new List<User>
            {
                  new User
            {
                UserId = 1,
                FirstName = "Serap",
                LastName = "Gurer",
                Email = "serapgurer01@gmail.com",
                PasswordHash = "password123",
                CreatedAt = DateTime.Now,
                IsActive = true
            }

            };
          
        }
            
        public void Add(User user)
        {
            _users.Add(user);
        }

        public void Delete(User user)
        {

            User assetToDelete = _users.SingleOrDefault(p => p.UserId == user.UserId);
            _users.Remove(assetToDelete);
        }

        public User Get(Expression<Func<User, bool>> filter)
        {
            return _users.SingleOrDefault(filter.Compile());
        }

        public List<User> GetAll(Expression<Func<User, bool>> filter = null)
        {
            return _users;
        }

        public void Update(User user)
        {
            User userToUpdate = _users.SingleOrDefault(p => p.UserId == user.UserId);

            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.Email = user.Email;
            userToUpdate.PasswordHash = user.PasswordHash;
            userToUpdate.IsActive = user.IsActive;


        }
    }
}
