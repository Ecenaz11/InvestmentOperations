using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Core.Utilities.Results;
using InvestmentOperations.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InvestmentOperations.Entities.Dtos;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace InvestmentOperations.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalancesController : ControllerBase
    {
        private readonly IBalanceService _balanceService;
        private readonly IAuthorizationService _authorizationService;
        public BalancesController(IBalanceService balanceService, IAuthorizationService authorizationService)
        {
            _balanceService = balanceService;
            _authorizationService = authorizationService;
        }

        [HttpPost("get")]
        public async Task<IActionResult> Get(BalanceQueryDto dto)
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
                var authResult = await _authorizationService.AuthorizeAsync(User, result.Data.UserId, "SameUserOrAdmin");
                if(!authResult.Succeeded)
                {
                    return Forbid();
                }
                return Ok(result);
            }

            if (dto != null && dto.UserId != null)
            {
                var authResult = await _authorizationService.AuthorizeAsync(User, dto.UserId.Value, "SameUserOrAdmin");
                if(!authResult.Succeeded)
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
            int targetUserId = dto.UserId;
            if(!User.IsInRole("Admin"))
            {
                targetUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
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
            int targetUserId = dto.UserId;
            if(!User.IsInRole("Admin"))
            {
                targetUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
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
