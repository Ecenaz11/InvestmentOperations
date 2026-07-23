using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Business.Abstract;
using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

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

            result = ValidateEmail(user.Email);
            if (!result.Success)
            {
                return result;
            }

            result = ValidatePassword(user);
            if (!result.Success)
            {
                return result;
            }

            PrepareUser(user);

            user.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            user.IsActive = true;

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

            user.CreatedAt = existingUser.CreatedAt;  

            IResult result = ValidatePassword(user);
            if (!result.Success)
            {
                return result;
            }  

            PrepareUser(user);

             result = ValidateUser(user);
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
            user.PasswordHash = HashPassword(user.PasswordHash.Trim());
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

        private string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100000, HashAlgorithmName.SHA256, 32);
            byte[] combined = new byte[salt.Length + hash.Length];
            Array.Copy(salt, 0, combined, 0, salt.Length);
            Array.Copy(hash, 0, combined, salt.Length, hash.Length);

            return Convert.ToBase64String(combined);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            byte[] combined = Convert.FromBase64String(storedHash);

            byte[] salt = new byte[16];
            Array.Copy(combined, 0, salt, 0, 16);

            byte[] originalHash = new byte[combined.Length - 16];
            Array.Copy(combined, 16, originalHash, 0, originalHash.Length);

            byte[] testHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100000, HashAlgorithmName.SHA256, 32);

            return CryptographicOperations.FixedTimeEquals(testHash, originalHash);
        }

        #endregion






    }

}
