using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Business.Abstract;
using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using InvestmentOperations.Entities.Enums;
using InvestmentOperations.Entities.Dtos;
using InvestmentOperations.Core.DataAccess;
using System.Threading.Tasks;

namespace InvestmentOperations.Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IUserDal _userDal;
        private readonly ILogService _logService;
        private readonly IUnitOfWork _unitOfWork;

        public UserManager(IUserDal userDal, ILogService logService, IUnitOfWork unitOfWork)
        {
            _userDal = userDal;
            _logService = logService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> Add(User user)
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

            result = await CheckDuplicateEmail(user.Email);
            if (!result.Success)
            {
                return result;
            }
            _userDal.Add(user);

            _logService.Add(new Log
            {
                UserId = user.UserId,
                Action = "UserRegistered",
                Details = $"{user.FirstName} {user.LastName} ({user.Email}) registered.",
                Status = LogStatus.Success
            });
            _unitOfWork.SaveChanges();

            return new SuccessResult("User added successfully.");
        }

        public async Task<IResult> Delete(int id)
        {
            var user = await _userDal.GetAsync(u => u.UserId == id);
            if (user == null)
            {
                return new ErrorResult("User not found.");
            }
            _userDal.Delete(user);

            _logService.Add(new Log
            {
                UserId = user.UserId,
                Action = "UserDeleted",
                Details = $"{user.FirstName} {user.LastName} ({user.Email}) deleted.",
                Status = LogStatus.Success
            });
            _unitOfWork.SaveChanges();

            return new SuccessResult("User deleted successfully.");
        }

        public async Task<IDataResult<User>> GetById(int id)
        {
            var user = await _userDal.GetAsync(u => u.UserId == id);
            if (user == null)
            {
                return new ErrorDataResult<User>("User not found.");
            }
            return new SuccessDataResult<User>(user, "User found.");
        }


        public async Task<IDataResult<List<User>>> GetAll()
        {
            return new SuccessDataResult<List<User>>
              (
                await _userDal.GetAllAsync(), "Users listed."
              );
        }

        public async Task<IResult> Update(User user)
        {
            var existingUser = await _userDal.GetAsync(u => u.UserId == user.UserId);
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

            result = await CheckDuplicateEmail(user.Email, user.UserId);
            if (!result.Success)
            {
                return result;
            }

            _userDal.Update(user);
            _logService.Add(new Log
            {
                UserId = user.UserId,
                Action = "UserUpdated",
                Details = $"{user.FirstName} {user.LastName} ({user.Email}) updated.",
                Status = LogStatus.Success
            });
            _unitOfWork.SaveChanges();

            return new SuccessResult("User updated successfully.");
        }

        public async Task<IDataResult<User>> Login(UserForLoginDto dto)
        {
            var user = await _userDal.GetAsync(u => u.Email == dto.Email.Trim().ToLowerInvariant());
            if (user == null)
            {
                _logService.Add(new Log
                {
                    UserId = 0,
                    Action = "UserLoginFailed",
                    Details = $"Login attempt failed for email: {dto.Email}. User not found.",
                    Status = LogStatus.Failed
                });
                return new ErrorDataResult<User>("Email or password is incorrect.");
            }

            bool passwordcorrect = VerifyPassword(dto.Password, user.PasswordHash);
            if (!passwordcorrect)
            {
                _logService.Add(new Log
                {
                    UserId = user.UserId,
                    Action = "UserLoginFailed",
                    Details = $"Incorrect password for {user.Email}.",
                    Status = LogStatus.Failed
                });
                return new ErrorDataResult<User>("Email or password is incorrect.");
            }
            if (!user.IsActive)
            {
                _logService.Add(new Log
                {
                    UserId = user.UserId,
                    Action = "UserLoginFailed",
                    Details = $"Login attempt for inactive account : {user.Email}.",
                    Status = LogStatus.Failed
                });
                return new ErrorDataResult<User>("User account is inactive.");
            }

            _logService.Add(new Log
            {
                UserId = user.UserId,
                Action = "UserLoggedIn",
                Details = $"{user.FirstName} {user.LastName} ({user.Email}) logged in",
                Status = LogStatus.Success
            });

            return new SuccessDataResult<User>(user, "Login successful.");
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
                return new ErrorResult("Last Name Cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return new ErrorResult("Email cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                return new ErrorResult("Password cannot be empty.");
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
                return new ErrorResult("The password must be at least 8 characters long.");
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
        private async Task<IResult> CheckDuplicateEmail(string email, int excludeUserId = 0)
        {
            var user = await _userDal.GetAsync(u => u.Email == email && u.UserId != excludeUserId);
            if (user != null)
            {
                return new ErrorResult("This email address is already registered.");
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
