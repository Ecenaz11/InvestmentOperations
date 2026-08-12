using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Mvc;
using InvestmentOperations.Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpPost("get")]
        public async Task<IActionResult> Get(LogsQueryDto dto)
        {
            if (dto == null || dto.Id == null)
            {
                var result = await _logService.GetAll();
                if (!result.Success)
                {
                    return BadRequest(result.Message);
                }
                var logDtos = result.Data.Select(MapToDto).ToList();
                return Ok(logDtos);
            }
            var userResult = await _logService.GetByUserId(dto.Id.Value);
            if (!userResult.Success)
            {
                return BadRequest(userResult.Message);
            }
            var userLogDtos = userResult.Data.Select(MapToDto).ToList();
            return Ok(userLogDtos);
        }

        private LogDto MapToDto(Log log)
        {
            return new LogDto
            {
                LogId = log.LogId,
                UserId = log.UserId,
                Action = log.Action,
                Details = log.Details,
                CreatedAt = log.CreatedAt,
                Status = log.Status
            };
        }

    }
}