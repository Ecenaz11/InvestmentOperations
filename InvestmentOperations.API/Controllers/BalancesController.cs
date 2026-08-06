using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InvestmentOperations.Entities.Dtos;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace InvestmentOperations.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalancesController : ControllerBase
    {
        private readonly IBalanceService _balanceService;
        public BalancesController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        [HttpPost("get")]
        public IActionResult Get(BalanceQueryDto dto)
        {
            var callerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            bool isAdmin = User.IsInRole("Admin");

            if (dto != null && dto.Id != null)
            {
                var result = _balanceService.GetByIdDetailed(dto.Id.Value);
                if (!result.Success)
                {
                    return BadRequest(result.Message);
                }
                if (!isAdmin && result.Data.UserId != callerId)
                {
                    return Forbid();
                }
                return Ok(result);
            }

            if (dto != null && dto.UserId != null)
            {
                if (!isAdmin && dto.UserId != callerId)
                {
                    return Forbid();
                }
                var userResult = _balanceService.GetByUserIdDetailed(dto.UserId.Value);
                if (!userResult.Success)
                {
                    return BadRequest(userResult.Message);
                }
                return Ok(userResult);
            }

            var allResult = isAdmin ? _balanceService.GetAllDetailed() : _balanceService.GetByUserIdDetailed(callerId);
            if (!allResult.Success)
            {
                return BadRequest(allResult.Message);
            }
            return Ok(allResult);
        }

        [HttpPost]
        public IActionResult Add(BalanceForAddDto dto)
        {
            if (!User.IsInRole("Admin"))
            {
                var callerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                if (dto.UserId != callerId)
                {
                    return Forbid();
                }
            }
            var balance = new Balance
            {
                UserId = dto.UserId,
                AssetId = dto.AssetId,
                Amount = dto.Amount
            };

            var result = _balanceService.Add(balance);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }

        [HttpPost("deposit")]
        public IActionResult Deposit(BalanceForDepositDto dto)
        {
            if (!User.IsInRole("Admin"))
            {
                var callerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                if (dto.UserId != callerId)
                {
                    return Forbid();
                }
            }
            var result = _balanceService.Deposit(dto.UserId, dto.Amount);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }
    }
}
