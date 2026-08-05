using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Mvc;
using InvestmentOperations.Entities.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace InvestmentOperations.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;
        public LogsController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _logService.GetAll();
            if(!result.Success)
            {
                return BadRequest(result.Message);
            }

            var logDtos = new List<LogDto>();
            foreach ( var log in result.Data)
            {
                logDtos.Add(MapToDto(log));
            }
            return Ok(logDtos);
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetByUserId(int userId)
        {
            var result = _logService.GetByUserId(userId);
            if(!result.Success)
            {
                return BadRequest(result.Message);
            }
            var logDtos = new List<LogDto>();
            foreach (var log in result.Data)
            {
                logDtos.Add(MapToDto(log));
            }
            return Ok(logDtos);
        }

        private LogDto MapToDto(Log log)
        {
            return new LogDto
            {
                LogId = log.LogId,
                UserId = log.UserId,
                Action=log.Action,
                Details=log.Details,
                CreatedAt=log.CreatedAt,
                Status = log.Status
            };
        }

    }
}