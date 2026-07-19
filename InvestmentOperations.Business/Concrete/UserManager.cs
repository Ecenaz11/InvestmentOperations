using InvestmentOperation.Core.Utilities.Results;
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

        public IResult Add(User user)
        {
            IResult result = ValidateUser(user);
            if (!result.Success)
            {
                return result;
            }

            PrepareUser(user);

            result = ValidateEmail(user.Email);
            if (!result.Success)
            {
                return result;
            }
            
            result = CheckDuplicateEmail(user.Email);
            if (!result.Success)
            {
                return result;
            }
            result = ValidatePassword(user);
            if (!result.Success)
            {
                return result;
            }

            _userDal.Add(user);

            return new SuccessResult("User added successfully.");
        }

        public IResult Delete(int id)
        {
            var user = _userDal.Get(u => u.UserId == id);
            if (user == null)
            {
                return new ErrorResult("User not found.");
            }
            _userDal.Delete(user);

            return new SuccessResult("User deleted successfully.");
        }

        public IDataResult<User> GetById(int id)
        {
            var user = _userDal.Get(u => u.UserId == id);
            if (user == null)
            {
                return new ErrorDataResult<User>("User not found.");
            }
            return new SuccessDataResult<User>(user, "User found.");
        }


        public IDataResult<List<User>> GetAll()
        {
            return new SuccessDataResult<List<User>>
              (
                _userDal.GetAll(), "Users listed."
              );
        }

        public IResult Update(User user)
        {
            var existingUser = _userDal.Get(u => u.UserId == user.UserId);
            if (existingUser == null)
            {
                return new ErrorResult("User not found.");
            }

            PrepareUser(user);

            IResult result = ValidateUser(user);
            if (!result.Success)
            {
                return result;
            }

            result = ValidateEmail(user.Email);
            if (!result.Success)
            {
                return result;
            }

            result = CheckDuplicateEmail(user.Email , user.UserId);
            if (!result.Success)
            {
                return result;
            }

            result = ValidatePassword(user);
            if (!result.Success)
            {
                return result;
            }

            _userDal.Update(user);
            return new SuccessResult("User updated successfully.");
        }
      
      
        
        #region Validation Methods

        private IResult ValidateUser(User user)
        {
            if (user == null)
            {
                return new ErrorResult("User cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(user.FirstName))
            {
                return new ErrorResult("First Name cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(user.LastName))
            {
                return new ErrorResult ("Last Name Cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return new ErrorResult ("Email cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                return new ErrorResult ("Password cannot be empty.");
            }
            return new SuccessResult();
        }

        private void PrepareUser(User user)
        {
            user.FirstName = user.FirstName.Trim().ToLowerInvariant();
            user.LastName = user.LastName.Trim().ToLowerInvariant();
            user.Email = user.Email.Trim().ToLowerInvariant();
            user.PasswordHash = user.PasswordHash.ToLowerInvariant();
        }

        private IResult ValidatePassword(User user)
        {
            if (user.PasswordHash.Length < 8)
            {
                return new ErrorResult ("The password must be at least 8 characters long.");
            }
            return new SuccessResult();
        }

        private IResult ValidateEmail(string email)
        {
            if (!email.Contains("@") || !email.Contains("."))
            {
                return new ErrorResult("Invalid email format.");
            }

            if (email.StartsWith("@") || email.EndsWith("@"))
            {
                return new ErrorResult("Invalid email format.");
            }
            return new SuccessResult();
        }
        private IResult CheckDuplicateEmail(string email, int excludeUserId = 0)
        {
            var user = _userDal.Get(u => u.Email == email && u.UserId != excludeUserId);
            if (user != null)
            {
                return new ErrorResult ("This email address is already registered.");
            }
            return new SuccessResult();
        }

       

        #endregion






    }

}
