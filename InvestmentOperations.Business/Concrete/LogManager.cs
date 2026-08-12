using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Business.Abstract;
using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Data;
using InvestmentOperations.Core.DataAccess;
using System.Threading.Tasks;

namespace InvestmentOperations.Business.Concrete
{
    public class LogManager : ILogService
    {
        private readonly ILogDal _logDal;
        private readonly IUnitOfWork _unitOfWork;
        public LogManager(ILogDal logDal, IUnitOfWork unitOfWork)
        {
            _logDal = logDal;
            _unitOfWork = unitOfWork;
        }
        public IResult Add(Log log)
        {
            IResult result = ValidateLog(log);
            if (!result.Success)
            {
                return result;
            }

            PrepareLog(log);

            _logDal.Add(log);
            _unitOfWork.SaveChanges();

            return new SuccessResult("Log added successfully.");
        }
        public async Task<IDataResult<List<Log>>> GetAll()
        {
            return new SuccessDataResult<List<Log>>(await _logDal.GetAllAsync(), "logs listed.");
        }

        public async Task<IDataResult<List<Log>>> GetByUserId(int userId)
        {
            return new SuccessDataResult<List<Log>>(await _logDal.GetByUserIdAsync(userId), "Logs listed.");
        }

        #region Validation Methods

        private IResult ValidateLog(Log log)
        {
            if (log == null)
            {
                return new ErrorResult("Log cannot be empty.");
            }

            if (log.UserId <= 0)
            {
                return new ErrorResult("UserId must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(log.Action))
            {
                return new ErrorResult("Action cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(log.Details))
            {
                return new ErrorResult("Details cannot be empty.");
            }
            return new SuccessResult();
        }
        private void PrepareLog(Log log)
        {
            log.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        }

        #endregion
    }
}