using InvestmentOperations.Business.Abstract;
using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace InvestmentOperations.Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IUserDal _userDal;
        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }

        public void Add(User user)
        {
            ValidateUser(user);
            PrepareUser(user);
            CheckDuplicateEmail(user.Email);
            ValidatePassword(user);

            _userDal.Add(user);
        }

        public void Delete(int id)
        {
            var user = GetExistingUser(id);
            _userDal.Delete(user);
        }

        public User GetById(int id)
        {
            return GetExistingUser(id);
        }


        public List<User> GetAll()
        {
            return _userDal.GetAll();
        }

        public void Update(User user)
        {
            var existingUser = GetExistingUser(user.UserId);
            PrepareUser(user);
            ValidateUser(user);
            ValidateEmail(user.Email);
            CheckDuplicateEmail(user.Email);
            ValidatePassword(user);

            _userDal.Update(user);
        }
      
        #region Validation Methods

        private void ValidateUser(User user)
        {
            if (user == null)
            {
                throw new Exception("User cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(user.FirstName))
            {
                throw new Exception("First Name cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(user.LastName))
            {
                throw new Exception("Last Name Cannot be empty.");
            }

            var englishCharacterPattern = @"^[a-zA-Z0-9\s]*$";
            if(!Regex.IsMatch(user.FirstName, englishCharacterPattern) ||
                !Regex.IsMatch(user.LastName, englishCharacterPattern))
            {
                throw new Exception("Please use only English characters for names (No: ı, ş, ğ, ü, ö, ç).");
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new Exception("Email cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                throw new Exception("Password cannot be empty.");
            }
        }

        private void PrepareUser(User user)
        {
            user.FirstName = user.FirstName.Trim().ToLowerInvariant();
            user.LastName = user.LastName.Trim().ToLowerInvariant();
            user.Email = user.Email.Trim().ToLowerInvariant();
            user.PasswordHash = user.PasswordHash.ToLowerInvariant();
        }
        private void ValidatePassword(User user)
        {
            if (user.PasswordHash.Length < 8)
            {
                throw new Exception("The password must be at least 8 characters long.");
            }
        }

        private void ValidateEmail(string email)
        {
            if (!email.Contains("@") || !email.Contains("."))
            {
                throw new Exception("Invalid email format.");
            }

            if (email.StartsWith("@") || email.EndsWith("@"))
            {
                throw new Exception("Invalid email format.");
            }
        }
        private void CheckDuplicateEmail(string email)
        {
            var user = _userDal.Get(u => u.Email == email);
            if (user != null)
            {
                throw new Exception("This email address is already registered.");
            }

        }

        private User GetExistingUser(int id)
        {
            var user = _userDal.Get(u => u.UserId == id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            return user;
        }
        #endregion






    }

}
