using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Business.Abstract;
using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Data;

namespace InvestmentOperations.Business.Concrete
{
    public class LogManager : ILogService
    {
        private readonly ILogDal _logDal;
        public LogManager(ILogDal logDal)
        {
            _logDal = logDal;
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
            return new SuccessResult("Log added successfully.");
        }

        public IDataResult<List<Log>> GetAll()
        {
            return new SuccessDataResult<List<Log>>(_logDal.GetAll(), "logs listed.");
        }

        public IDataResult<List<Log>> GetByUserId(int userId)
        {
            return new SuccessDataResult<List<Log>>(_logDal.GetByUserId(userId), "Logs listed.");
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